using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using XIVComboExpandedPlugin;
using XIVComboExpandedPlugin.Attributes;
using static System.Net.Mime.MediaTypeNames;

namespace XIVComboExpandedPlugin.Interface;

/// <summary>
/// Plugin configuration window.
/// </summary>
internal class ConfigWindow : Window
{
    private readonly Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> groupedPresets;
    private readonly Dictionary<CustomComboPreset, (CustomComboPreset Preset, CustomComboInfoAttribute Info)[]> presetChildren;
    private readonly Vector4 shadedColor = new(0.68f, 0.68f, 0.68f, 1.0f);

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigWindow"/> class.
    /// </summary>
    public ConfigWindow()
        : base("XIV Combo Expanded")
    {
        this.RespectCloseHotkey = true;

        this.groupedPresets = Enum
            .GetValues<CustomComboPreset>()
            .Where(preset => (int)preset > 100 && preset != CustomComboPreset.Disabled)
            .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
            .Where(tpl => tpl.Info != null && Service.Configuration.GetParent(tpl.Preset) == null)
            .OrderBy(tpl => CustomComboInfoAttribute.RoleIDToOrder(tpl.Info.RoleName))
            .ThenBy(tpl => tpl.Info.JobID)
            .GroupBy(tpl => tpl.Info.JobName)
            .ToDictionary(
                tpl => tpl.Key,
                tpl => tpl.ToList());

        var childCombos = Enum.GetValues<CustomComboPreset>().ToDictionary(
            tpl => tpl,
            tpl => new List<CustomComboPreset>());

        foreach (var preset in Enum.GetValues<CustomComboPreset>())
        {
            var parent = preset.GetAttribute<ParentComboAttribute>()?.ParentPreset;
            if (parent != null)
                childCombos[parent.Value].Add(preset);
        }

        this.presetChildren = childCombos.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value
                .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
                .OrderBy(tpl => tpl.Info.Order).ToArray());

        this.SizeCondition = ImGuiCond.FirstUseEver;
        this.Size = new Vector2(740, 490);
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        if (ImGui.BeginTabBar("Tabs"))
        {
            if (ImGui.BeginTabItem("Combos"))
            {
                if (ImGui.BeginChild("TabButtons", new System.Numerics.Vector2(40f, 0f), false, ImGuiWindowFlags.NoScrollbar))
                {
                    ImGui.SameLine(2f);

                    if (ImGui.BeginTable("TabButtonsTable", 1, ImGuiTableFlags.None, new System.Numerics.Vector2(400f, 36f), 5f))
                    {
                        foreach (var jobName in this.groupedPresets.Keys)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID($"EditorTab{CustomComboInfoAttribute.NameToJobID(jobName)}");
                            bool selected = Service.Configuration.CurrentTab == jobName ? true : false;
                            if (selected)
                            {
                                ImGui.PushStyleColor(ImGuiCol.Button, ImGuiColors.DalamudGrey2);
                                ImGui.PushStyleColor(ImGuiCol.Border, ImGuiColors.DalamudGrey3);
                            }
                            else
                            {
                                ImGui.PushStyleColor(ImGuiCol.Button, 0);

                                ImGui.PushStyleColor(ImGuiCol.Border, 0);
                            }

                            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0);
                            var image = GetJobIcon(CustomComboInfoAttribute.NameToJobID(jobName));

                            if (ImGui.ImageButton(image.GetWrapOrDefault().ImGuiHandle, new System.Numerics.Vector2(28f, 28f)))
                            {
                                Service.Configuration.CurrentTab = jobName;
                            }

                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(jobName);
                                ImGui.EndTooltip();
                            }

                            ImGui.PopStyleVar();
                            ImGui.PopStyleColor(2);
                            ImGui.PopID();
                        }

                        ImGui.EndTable();
                    }

                    ImGui.EndChild();
                }

                ImGui.SameLine();

                ImGui.BeginGroup();

                ImGui.PushStyleColor(ImGuiCol.ChildBg, ImGuiColors.DalamudWhite);
                ImGui.PushStyleColor(ImGuiCol.Border, ImGuiColors.DalamudWhite2);

                ImGui.Indent();
                if (ImGui.BeginChild("TabContent", new Vector2(0, -1), true, ImGuiWindowFlags.NoBackground))
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 5));
                    ImGui.PopStyleColor(2);
                    int i = 1;
                    foreach (var (preset, info) in this.groupedPresets[Service.Configuration.CurrentTab])
                    {
                        this.DrawPreset(preset, info, ref i);
                    }

                    ImGui.EndChild();
                }

                ImGui.Unindent();

                ImGui.EndGroup();

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Settings"))
            {
                var showSecrets = Service.Configuration.EnableSecretCombos;
                if (ImGui.Checkbox("Enable secret forbidden knowledge", ref showSecrets))
                {
                    Service.Configuration.EnableSecretCombos = showSecrets;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Optimized, potentially unintuitive combos for the common folk");
                    ImGui.EndTooltip();
                }

                var hideChildren = Service.Configuration.HideChildren;
                if (ImGui.Checkbox("Hide children of disabled combos and features", ref hideChildren))
                {
                    Service.Configuration.HideChildren = hideChildren;
                    Service.Configuration.Save();
                }

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Changelog"))
            {
                ImGui.BeginChild("scrolling", new Vector2(0, -1), true);

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 5));

                var changelog = new Dictionary<string, string[]>()
                {
                    { "v3", ["dummy (like me)"] },
                    { "v2", ["data"] },
                    { "v1", ["text", "can be on 2 lines I think", "and even three wooooo"] },
                };


                foreach (var (version, info) in changelog)
                {
                    if (ImGui.CollapsingHeader(version))
                    {
                        ImGui.PushItemWidth(200);

                        ImGui.PopItemWidth();

                        ImGui.PushStyleColor(ImGuiCol.Text, this.shadedColor);
                        foreach (var text in info)
                        {
                            ImGui.BulletText(text);
                        }

                        ImGui.PopStyleColor();
                        ImGui.Spacing();
                    }
                }

                ImGui.EndChild();
                ImGui.EndTabItem();
            }


            if (ImGui.BeginTabItem("About"))
            {

                ImGui.Text("Project's GitHub link :");
                if (ImGui.Button("Repository link"))
                {
                    Process.Start(new ProcessStartInfo { FileName = "https://github.com/MKhayle/XIVComboExpanded", UseShellExecute = true });
                }

                ImGui.Separator();
                ImGui.Spacing();

                ImGui.Text("Contributors and special thanks:");
                ImGui.BulletText("goat");
                ImGui.BulletText("meli");
                ImGui.BulletText("attick");
                ImGui.BulletText("daemitus");
                ImGui.BulletText("Grammernatzi");
                ImGui.BulletText("khayle");
                ImGui.BulletText("kaedys");
                ImGui.BulletText("aldros-ffxi");
                ImGui.BulletText("lhn1703");
                ImGui.BulletText("pliv-dev");
                ImGui.BulletText("bfabe8");
                ImGui.BulletText("mikel-gh");
                ImGui.BulletText("diwo");
                ImGui.BulletText("MayakoAelys");
                ImGui.BulletText("andyvorld");
                ImGui.BulletText("rz-1");
                ImGui.BulletText("AkiraChisaka");
                ImGui.BulletText("Aelexe");
                ImGui.BulletText("perks");
                ImGui.Text("And many others who contributed through issues, bug reporting or feature requests!");

                ImGui.Separator();
                ImGui.Spacing();
                ImGui.Text("If you want to personally support me for maintaining this project (♥):");
                if (ImGui.Button("My Ko-Fi link"))
                {
                    Process.Start(new ProcessStartInfo { FileName = "https://ko-fi.com/khayle", UseShellExecute = true });
                }

                ImGui.EndTabItem();
                }
            }

        ImGui.EndTabBar();
    }


    private void DrawPreset(CustomComboPreset preset, CustomComboInfoAttribute info, ref int i)
    {
        var enabled = Service.Configuration.IsEnabled(preset);
        var secret = Service.Configuration.IsSecret(preset);
        var showSecrets = Service.Configuration.EnableSecretCombos;
        var conflicts = Service.Configuration.GetConflicts(preset);
        var parent = Service.Configuration.GetParent(preset);

        if (secret && !showSecrets)
            return;

        ImGui.PushItemWidth(200);

        if (ImGui.Checkbox(info.FancyName, ref enabled))
        {
            if (enabled)
            {
                this.EnableParentPresets(preset);
                Service.Configuration.EnabledActions.Add(preset);
                foreach (var conflict in conflicts)
                {
                    Service.Configuration.EnabledActions.Remove(conflict);
                }
            }
            else
            {
                Service.Configuration.EnabledActions.Remove(preset);
            }

            Service.Configuration.Save();
        }

        if (secret)
        {
            ImGui.SameLine();
            ImGui.Text("  ");
            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
            ImGui.Text(FontAwesomeIcon.Star.ToIconString());
            ImGui.PopStyleColor();
            ImGui.PopFont();

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Secret");
                ImGui.EndTooltip();
            }
        }

        ImGui.PopItemWidth();

        ImGui.PushStyleColor(ImGuiCol.Text, this.shadedColor);
        ImGui.TextWrapped($"{info.Description}");
        ImGui.PopStyleColor();
        ImGui.Spacing();

        if (conflicts.Length > 0)
        {
            var conflictText = conflicts.Select(conflict =>
            {
                if (!showSecrets && Service.Configuration.IsSecret(conflict))
                    return string.Empty;

                var conflictInfo = conflict.GetAttribute<CustomComboInfoAttribute>();
                return $"\n - {conflictInfo.FancyName}";
            }).Aggregate((t1, t2) => $"{t1}{t2}");

            if (conflictText.Length > 0)
            {
                ImGui.TextColored(this.shadedColor, $"Conflicts with: {conflictText}");
                ImGui.Spacing();
            }
        }

        if (preset == CustomComboPreset.DancerDanceComboCompatibility && enabled)
        {
            var actions = Service.Configuration.DancerDanceCompatActionIDs.Cast<int>().ToArray();

            var inputChanged = false;
            inputChanged |= ImGui.InputInt("Emboite (Red) ActionID", ref actions[0], 0);
            inputChanged |= ImGui.InputInt("Entrechat (Blue) ActionID", ref actions[1], 0);
            inputChanged |= ImGui.InputInt("Jete (Green) ActionID", ref actions[2], 0);
            inputChanged |= ImGui.InputInt("Pirouette (Yellow) ActionID", ref actions[3], 0);

            if (inputChanged)
            {
                Service.Configuration.DancerDanceCompatActionIDs = actions.Cast<uint>().ToArray();
                Service.Configuration.Save();
            }

            ImGui.Spacing();
        }

        i++;

        var hideChildren = Service.Configuration.HideChildren;
        if (enabled || !hideChildren)
        {
            var children = this.presetChildren[preset];
            if (children.Length > 0)
            {
                ImGui.Indent();

                foreach (var (childPreset, childInfo) in children)
                    this.DrawPreset(childPreset, childInfo, ref i);

                ImGui.Unindent();
            }
        }
    }

    /// <summary>
    /// Iterates up a preset's parent tree, enabling each of them.
    /// </summary>
    /// <param name="preset">Combo preset to enabled.</param>
    private void EnableParentPresets(CustomComboPreset preset)
    {
        var parentMaybe = Service.Configuration.GetParent(preset);
        while (parentMaybe != null)
        {
            var parent = parentMaybe.Value;

            if (!Service.Configuration.EnabledActions.Contains(parent))
            {
                Service.Configuration.EnabledActions.Add(parent);
                foreach (var conflict in Service.Configuration.GetConflicts(parent))
                {
                    Service.Configuration.EnabledActions.Remove(conflict);
                }
            }

            parentMaybe = Service.Configuration.GetParent(parent);
        }
    }



    /// <summary>
    /// Returns a ISharedImmediateTexture for the appropriate job
    /// </summary>
    /// <param name="jobID">ID of the job.</param>
    private static ISharedImmediateTexture GetJobIcon(int jobID)
    {
        var iconID = 62100 + jobID;

        if (iconID < 62101 || iconID > 62142)
            iconID = 62145;
        if (jobID == 0)
            iconID = 62146;

        return Service.TextureProvider.GetFromGameIcon(new GameIconLookup((uint)(iconID), true));
    }
}
