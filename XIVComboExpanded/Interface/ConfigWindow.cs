using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using XIVComboExpandedPlugin.Attributes;
using Action = Lumina.Excel.GeneratedSheets.Action;
using Language = Lumina.Data.Language;

namespace XIVComboExpandedPlugin.Interface;

/// <summary>
/// Plugin configuration window.
/// </summary>
internal class ConfigWindow : Window
{
    private enum Tabs
    {
        Classic = 1,
        Accessibility = 2,
        Expanded = 3,
        Secret = 4,
    }

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
            .ThenBy(tpl => tpl.Info.Order)
            .ThenBy(tpl => tpl.Preset.GetAttribute<SectionComboAttribute>()?.Section)
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
        this.Size = new Vector2(750, 500);
        WindowSizeConstraints windowSizeConstraints = new WindowSizeConstraints();
        if (Service.Configuration.BigComboIcons || Service.Configuration.BigJobIcons)
            windowSizeConstraints.MinimumSize = new Vector2(900, 700);
        else
        windowSizeConstraints.MinimumSize = new Vector2(750, 500);
        this.SizeConstraints = windowSizeConstraints;
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        if (ImGui.BeginTabBar("Tabs"))
        {
            #region COMBOS TAB

            if (ImGui.BeginTabItem("Combos"))
            {
                float scale = 1f;
                if (Service.Configuration.BigJobIcons)
                    scale = 1.5f;

                if (ImGui.BeginChild("TabButtons", new System.Numerics.Vector2(36f*scale, 0f), false, ImGuiWindowFlags.NoScrollbar))
                {
                    ImGui.SameLine(1f);

                    if (ImGui.BeginTable("TabButtonsTable", 1, ImGuiTableFlags.None, new System.Numerics.Vector2(40f*scale, 36f*scale), 4f*scale))
                    {
                        if ((Service.Configuration.CurrentTab == "Adventurer"
                            || Service.Configuration.CurrentTab == "Disciples of the Land"
                            || Service.Configuration.CurrentTab == "Sage") && !Service.Configuration.EnableExpandedCombos)
                        {
                            Service.Configuration.CurrentTab = "Paladin";
                        }

                        foreach (var jobName in this.groupedPresets.Keys)
                        {
                            if ((jobName == "Adventurer"
                                || jobName == "Disciples of the Land"
                                || jobName == "Sage") && !Service.Configuration.EnableExpandedCombos)
                            {
                            }
                            else
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

                                ISharedImmediateTexture image = GetJobIcon(CustomComboInfoAttribute.NameToJobID(jobName));

                                if (image != null)
                                {
                                    if (ImGui.ImageButton(image.GetWrapOrEmpty().ImGuiHandle, new System.Numerics.Vector2(28f*scale, 28f*scale)))
                                    {
                                        Service.Configuration.CurrentTab = jobName;
                                    }

                                    if (ImGui.IsItemHovered())
                                    {
                                        ImGui.BeginTooltip();
                                        ImGui.TextUnformatted(jobName);
                                        ImGui.EndTooltip();
                                    }
                                }

                                ImGui.PopStyleVar();
                                ImGui.PopStyleColor(2);
                                ImGui.PopID();
                            }
                        }

                        ImGui.EndTable();
                    }

                    ImGui.EndChild();
                }

                ImGui.SameLine();

                ImGui.BeginGroup();

                ImGui.PushStyleColor(ImGuiCol.ChildBg, ImGuiColors.DalamudWhite);
                ImGui.PushStyleColor(ImGuiCol.Border, ImGuiColors.DalamudWhite2);

                ImGui.Indent(4f);
                if (ImGui.BeginChild("TabContent", new Vector2(0, -1), true, ImGuiWindowFlags.NoBackground))
                {
                    #region COMBOS TAB HEADER
                    var jobID = CustomComboInfoAttribute.NameToJobID(Service.Configuration.CurrentTab);
                    var image = GetJobIcon(jobID);
                    ImGui.Image(image.GetWrapOrEmpty().ImGuiHandle, new System.Numerics.Vector2(36f, 36f));
                    ImGui.SameLine();
                    ImGui.PushFont(UiBuilder.MonoFont);
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedGold);
                    ImGui.Text($" " + Service.Configuration.CurrentTab + "\n " + (CustomComboInfoAttribute.JobIDToRole(jobID) != "Adventurer" ? CustomComboInfoAttribute.JobIDToRole(jobID) : "Warrior of Light"));
                    ImGui.PopStyleColor();
                    ImGui.PopFont();
                    ImGui.Separator();
                    ImGui.PopStyleColor(2);
                    #endregion

                    if (ImGui.BeginTabBar("ComboTabs"))
                    {
                        if(Service.Configuration.CurrentTab != "Adventurer" && Service.Configuration.CurrentTab != "Disciples of the Land" && Service.Configuration.CurrentTab != "Sage")
                        {
                            if (ImGui.BeginTabItem("Classic"))
                            {
                                if ((ImGui.IsItemHovered() && !ImGui.IsItemActive()) || ImGui.IsItemHovered())
                                {
                                    ImGui.BeginTooltip();
                                    ImGui.TextUnformatted("Classic Hover Tooltip");
                                    ImGui.EndTooltip();
                                }

                                ImGui.BeginChild("scrolling", new Vector2(0, -1), true);

                                int i = 1;
                                string previousSection = string.Empty;
                                foreach (var (preset, info) in this.groupedPresets[Service.Configuration.CurrentTab])
                                {
                                    previousSection = this.DrawPreset(Tabs.Classic, preset, info, previousSection, ref i);
                                }

                                ImGui.EndChild();
                                ImGui.EndTabItem();
                            }
                        }

                        if (Service.Configuration.EnableExpandedCombos)
                        {
                            if (ImGui.BeginTabItem("Expanded"))
                            {
                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.BeginTooltip();
                                    ImGui.TextUnformatted("Expanded hover tooltip");
                                    ImGui.EndTooltip();
                                }

                                ImGui.BeginChild("scrolling", new Vector2(0, -1), true);

                                int i = 1;
                                string previousSection = string.Empty;
                                foreach (var (preset, info) in this.groupedPresets[Service.Configuration.CurrentTab])
                                {
                                    previousSection = this.DrawPreset(Tabs.Expanded, preset, info, previousSection, ref i);
                                }


                                ImGui.EndChild();
                                ImGui.EndTabItem();
                            }
                        }


                        if (Service.Configuration.EnableAccessibilityCombos)
                        {
                            if (ImGui.BeginTabItem("Accessibility"))
                            {
                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.BeginTooltip();
                                    ImGui.TextUnformatted("Accessibility");
                                    ImGui.EndTooltip();
                                }

                                ImGui.BeginChild("scrolling", new Vector2(0, -1), true);

                                int i = 1;
                                string previousSection = string.Empty;
                                foreach (var (preset, info) in this.groupedPresets[Service.Configuration.CurrentTab])
                                {
                                    previousSection = this.DrawPreset(Tabs.Accessibility, preset, info, previousSection, ref i);
                                }


                                ImGui.EndChild();
                                ImGui.EndTabItem();
                            }
                        }

                        if (Service.Configuration.EnableSecretCombos)
                        {
                            if (ImGui.BeginTabItem("Secret"))
                            {
                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.BeginTooltip();
                                    ImGui.TextUnformatted("Secret hover tooltip");
                                    ImGui.EndTooltip();
                                }

                                ImGui.BeginChild("scrolling", new Vector2(0, -1), true);

                                int i = 1;
                                string previousSection = string.Empty;
                                foreach (var (preset, info) in this.groupedPresets[Service.Configuration.CurrentTab])
                                {
                                    previousSection = this.DrawPreset(Tabs.Secret, preset, info, previousSection, ref i);
                                }


                                ImGui.EndChild();
                                ImGui.EndTabItem();
                            }
                        }

                        ImGui.EndTabBar();
                    }

                    ImGui.EndChild();
                }

                ImGui.Unindent();

                ImGui.EndGroup();

                ImGui.EndTabItem();
            }
            #endregion

            #region SETTINGS TAB

            if (ImGui.BeginTabItem("Settings"))
            {

                var enablePlugin = Service.Configuration.EnablePlugin;
                if (ImGui.Checkbox("Enables this plugin.", ref enablePlugin))
                {
                    Service.Configuration.EnablePlugin = enablePlugin;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Completely disables every combo when unchecked.");
                    ImGui.EndTooltip();
                }

                var showExpanded = Service.Configuration.EnableExpandedCombos;
                if (ImGui.Checkbox("Enables the expanded features for XIVCombo.", ref showExpanded))
                {
                    Service.Configuration.EnableExpandedCombos = showExpanded;
                    Service.Configuration.EnableAccessibilityCombos = showExpanded;
                    Service.Configuration.EnableSecretCombos = showExpanded;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Optimized, potentially unintuitive combos.");
                    ImGui.EndTooltip();
                }

                if (showExpanded)

                {
                    var showAccessibility = Service.Configuration.EnableAccessibilityCombos;
                    if (ImGui.Checkbox("Enable accessibility combos.", ref showAccessibility))
                    {
                        Service.Configuration.EnableAccessibilityCombos = showAccessibility;
                        Service.Configuration.Save();
                    }

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted("Unoptimized, easy-to-use combos.");
                        ImGui.EndTooltip();
                    }

                    var showSecrets = Service.Configuration.EnableSecretCombos;
                    if (ImGui.Checkbox("Enable secret forbidden knowledge.", ref showSecrets))
                    {
                        Service.Configuration.EnableSecretCombos = showSecrets;
                        Service.Configuration.Save();
                    }

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted("Optimized, potentially unintuitive combos.");
                        ImGui.EndTooltip();
                    }
                }

                var bigComboIcons = Service.Configuration.BigComboIcons;
                if (ImGui.Checkbox("Increase the size of icons for combos and features.", ref bigComboIcons))
                {
                    Service.Configuration.BigComboIcons = bigComboIcons;
                    Service.Configuration.Save();
                }

                var bigJobIcons = Service.Configuration.BigJobIcons;
                if (ImGui.Checkbox("Increase the size of icons for the jobs on the side bar.", ref bigJobIcons))
                {
                    Service.Configuration.BigJobIcons = bigJobIcons;
                    Service.Configuration.Save();
                }

                var hideIcons = Service.Configuration.HideIcons;
                if (ImGui.Checkbox("Hide icons for combos and features.", ref hideIcons))
                {
                    Service.Configuration.HideIcons = hideIcons;
                    Service.Configuration.Save();
                }

                var hideChildren = Service.Configuration.HideChildren;
                if (ImGui.Checkbox("Hide children of disabled combos and features.", ref hideChildren))
                {
                    Service.Configuration.HideChildren = hideChildren;
                    Service.Configuration.Save();
                }

                ImGui.EndTabItem();
            }
            #endregion

            #region CHANGELOG TAB

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

                ImGui.PopStyleVar();

                ImGui.EndChild();
                ImGui.EndTabItem();
            }
            #endregion

            #region ABOUT TAB
            if (ImGui.BeginTabItem("About"))
            {


                ImGui.Separator();
                ImGui.Spacing();

                ImGui.Spacing();
                ImGui.Spacing();
                ImGui.PushFont(UiBuilder.MonoFont);
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudWhite2);
                ImGui.Text("Statistics");
                ImGui.PopStyleColor();
                ImGui.PopFont();
                ImGui.Separator();
                ImGui.Spacing();

                ImGui.Text($"{Enum.GetValues<CustomComboPreset>().Where(preset => (int)preset > 100 && preset != CustomComboPreset.Disabled && Service.Configuration.IsEnabled(preset)).Count()} combos currently enabled.");
                ImGui.Text($"{Enum.GetValues<CustomComboPreset>().Where(preset => (int)preset > 100 && preset != CustomComboPreset.Disabled).Count()} available in total.");

                ImGui.Separator();
                ImGui.Spacing();
                ImGui.Spacing();
                ImGui.PushFont(UiBuilder.MonoFont);
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudWhite2);
                ImGui.Text("GitHub Repository");
                ImGui.PopStyleColor();
                ImGui.PopFont();
                ImGui.Separator();
                ImGui.Spacing();

                if (ImGui.Button("Open the GitHub Repository"))
                {
                    Process.Start(new ProcessStartInfo { FileName = "https://github.com/MKhayle/XIVComboExpanded", UseShellExecute = true });
                }

                ImGui.Text("Dalamud Repository URL");
                var url = "https://github.com/daemitus/MyDalamudPlugins/raw/master/pluginmaster.json";
                ImGui.InputText("", ref url, 100, ImGuiInputTextFlags.ReadOnly);

                ImGui.Spacing();
                ImGui.Spacing();
                ImGui.PushFont(UiBuilder.MonoFont);
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudWhite2);
                ImGui.Text("Contributors and special thanks");
                ImGui.PopStyleColor();
                ImGui.PopFont();
                ImGui.Separator();
                ImGui.Spacing();

                ImGui.BulletText("goat and the whole Dalamud team");
                ImGui.BulletText("ff-meli for the initial concept");
                ImGui.BulletText("attick for XIVCombo");
                ImGui.BulletText("daemitus for creating XIVCombo Expanded");
                ImGui.BulletText("Grammernatzi for supporting the project");
                ImGui.BulletText("kaedys for considerably contributing to the repository");
                ImGui.Spacing();
                ImGui.Text("Additional thanks to all those contributors");
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
                ImGui.Spacing();
                ImGui.Text("And many others who contributed through issues, bug reporting or feature requests!");

                ImGui.EndTabItem();
            }
            #endregion
        }

        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - 100f - ImGui.GetScrollX()
                               - 2 * ImGui.GetStyle().ItemSpacing.X);
        if (ImGui.Button("My Ko-Fi link ♥"))
        {
            Process.Start(new ProcessStartInfo { FileName = "https://ko-fi.com/khayle", UseShellExecute = true });
        }

        ImGui.EndTabBar();
    }

    private void DrawSection(Tabs tab, CustomComboPreset preset, CustomComboInfoAttribute info, ref int i)
    {
        var enabled = Service.Configuration.IsEnabled(preset);
        var secret = Service.Configuration.IsSecret(preset);
        var expanded = Service.Configuration.IsExpanded(preset);
        var accessibility = Service.Configuration.IsAccessible(preset);
        var conflicts = Service.Configuration.GetConflicts(preset);
        var parent = Service.Configuration.GetParent(preset);
        string section = preset.GetAttribute<SectionComboAttribute>()?.Section;
        uint[] icons = [];

        switch (tab)
        {
            case Tabs.Classic:
                if (accessibility || expanded || secret)
                    return;
                break;
            case Tabs.Expanded:
                if (accessibility || secret)
                    return;
                break;
            case Tabs.Accessibility:
                if (secret)
                    return;
                break;
            case Tabs.Secret:
                if (accessibility && !Service.Configuration.EnableAccessibilityCombos)
                    return;
                break;
            default:
                break;
        }

        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.PushFont(UiBuilder.MonoFont);
        ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedOrange);
        ImGui.Text(section);
        ImGui.PopStyleColor();
        ImGui.PopFont();
        ImGui.Separator();
        ImGui.Spacing();
    }

    private string DrawPreset(Tabs tab, CustomComboPreset preset, CustomComboInfoAttribute info, string previousSection, ref int i)
    {
        var enabled = Service.Configuration.IsEnabled(preset);
        var secret = Service.Configuration.IsSecret(preset);
        var expanded = Service.Configuration.IsExpanded(preset);
        var accessibility = Service.Configuration.IsAccessible(preset);
        var conflicts = Service.Configuration.GetConflicts(preset);
        var parent = Service.Configuration.GetParent(preset);
        uint[] icons = [];
        string section = string.Empty;

        if (preset.GetAttribute<IconsComboAttribute>()?.Icons.Length > 0)
            icons = preset.GetAttribute<IconsComboAttribute>().Icons;
        if (preset.GetAttribute<SectionComboAttribute>()?.Section != null)
            section = preset.GetAttribute<SectionComboAttribute>().Section.ToString();

        switch (tab)
        {
            case Tabs.Classic:
                if (accessibility || expanded || secret)
                    return previousSection;
                break;
            case Tabs.Expanded:
                if (accessibility || secret)
                    return previousSection;
                break;
            case Tabs.Accessibility:
                if (secret)
                    return previousSection;
                break;
            case Tabs.Secret:
                if (accessibility && !Service.Configuration.EnableAccessibilityCombos)
                    return previousSection;
                break;
            default:
                break;
        }

        if (preset.GetAttribute<SectionComboAttribute>()?.Section != null)
        {
            if (previousSection != preset.GetAttribute<SectionComboAttribute>()?.Section && previousSection != "child")
            {
                this.DrawSection(tab, preset, info, ref i);
                previousSection = preset.GetAttribute<SectionComboAttribute>()?.Section;
            }
        }


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

        if (expanded)
        {
            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.TankBlue);
            ImGui.Text(FontAwesomeIcon.Star.ToIconString());
            ImGui.PopStyleColor();
            ImGui.PopFont();

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Expanded combo");
                ImGui.EndTooltip();
            }
        }

        if (accessibility)
        {
            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
            ImGui.Text(FontAwesomeIcon.Star.ToIconString());
            ImGui.PopStyleColor();
            ImGui.PopFont();

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Accessibility combo");
                ImGui.EndTooltip();
            }
        }

        if (secret)
        {
            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DPSRed);
            ImGui.Text(FontAwesomeIcon.Star.ToIconString());
            ImGui.PopStyleColor();
            ImGui.PopFont();

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Secret combo");
                ImGui.EndTooltip();
            }
        }


        float scale = 1;
        if (Service.Configuration.BigComboIcons)
            scale = 1.3f;


        if (icons.Length > 0 && !Service.Configuration.HideIcons)
        {
            ImGui.SameLine();
            ImGui.SetCursorPosX(
              ImGui.GetCursorPosX()
              + ImGui.GetColumnWidth()
              - (icons.Length * ((24f * scale) + (float)ImGui.GetStyle().ItemSpacing.X))
              + ImGui.GetScrollX());


            int it = 0;
            foreach (var iconId in icons)
            {
                bool isStatus = false;
                bool isUTL = false;
                string hoverName = string.Empty;
                ISharedImmediateTexture icon;

                // Workaround which will work until it won't work anymore
                if (iconId > 60000)
                {
                    icon = GetIcon(iconId);
                    isUTL = true;
                }
                else
                {
                    icon = GetSkillIcon(iconId);
                    if (icon == null)
                    {
                        isStatus = true;
                        icon = GetStatusIcon(iconId);
                    }
                }

                if (isStatus)
                {
                    ImGui.Image(GetIcon(IconsComboAttribute.Blank).GetWrapOrEmpty().ImGuiHandle, new System.Numerics.Vector2(3f * scale, 24f * scale));
                    ImGui.SameLine(0, 0);
                    ImGui.Image(icon.GetWrapOrEmpty().ImGuiHandle, new System.Numerics.Vector2(18f * scale, 24f * scale));
                    hoverName = GetStatusName(iconId);
                }
                else if (isUTL)
                {
                    ImGui.Image(GetIcon(IconsComboAttribute.Blank).GetWrapOrEmpty().ImGuiHandle, new System.Numerics.Vector2(2f * scale, 24f * scale));
                    ImGui.SameLine(0, 0);
                    ImGui.Image(icon.GetWrapOrEmpty().ImGuiHandle, new System.Numerics.Vector2(20f * scale, 20f * scale));
                }
                else
                {
                    ImGui.Image(icon.GetWrapOrEmpty().ImGuiHandle, new System.Numerics.Vector2(24f*scale, 24f*scale));
                    hoverName = GetSkillName(iconId);
                }

                if (hoverName != string.Empty)
                {
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(hoverName);
                        ImGui.EndTooltip();
                    }
                }

                if (isStatus)
                {
                    ImGui.SameLine(0, 0);
                    ImGui.Image(GetIcon(IconsComboAttribute.Blank).GetWrapOrEmpty().ImGuiHandle, new System.Numerics.Vector2(3f * scale, 24f * scale));
                }

                if (isUTL)
                {
                    ImGui.SameLine(0, 0);
                    ImGui.Image(GetIcon(IconsComboAttribute.Blank).GetWrapOrEmpty().ImGuiHandle, new System.Numerics.Vector2(2f * scale, 24f * scale));
                }

                it++;

                if (icons.Count() != it)
                {
                    ImGui.SameLine();
                }
                else
                    it = 0;
            }

        }

        ImGui.PopItemWidth();

        ImGui.PushStyleColor(ImGuiCol.Text, this.shadedColor);
        ImGui.TextWrapped($"{info.Description}");
        ImGui.PopStyleColor();
        ImGui.Spacing();

        if (conflicts.Length > 0 && enabled)
        {
            var conflictText = conflicts.Select(conflict =>
            {
                switch (tab)
                {
                    case Tabs.Classic:
                        if ((Service.Configuration.IsSecret(conflict) && !Service.Configuration.EnableSecretCombos)
                        || (Service.Configuration.IsAccessible(conflict) && !Service.Configuration.EnableAccessibilityCombos)
                        || (Service.Configuration.IsExpanded(conflict) && !Service.Configuration.EnableExpandedCombos))
                            return string.Empty;
                        break;
                    case Tabs.Expanded:
                        if ((Service.Configuration.IsSecret(conflict) && !Service.Configuration.EnableSecretCombos)
                        || (Service.Configuration.IsAccessible(conflict) && !Service.Configuration.EnableAccessibilityCombos))
                            return string.Empty;
                        break;
                    case Tabs.Accessibility:
                        if (Service.Configuration.IsSecret(conflict) && !Service.Configuration.EnableSecretCombos)
                            return string.Empty;
                        break;
                    case Tabs.Secret:
                        if (Service.Configuration.IsAccessible(conflict) && !Service.Configuration.EnableAccessibilityCombos)
                            return string.Empty;
                        break;
                    default:
                        break;
                }

                var conflictInfo = conflict.GetAttribute<CustomComboInfoAttribute>();
                return $" · {conflictInfo.FancyName}";
            }).Aggregate((t1, t2) => $"{t1}{t2}");

            if (conflictText.Length > 0)
            {
                ImGui.TextColored(ImGuiColors.DPSRed, $"Conflicts with {conflictText}");
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
                    this.DrawPreset(tab, childPreset, childInfo, "child", ref i);

                ImGui.Unindent();
            }
        }
        return section;
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
    /// Returns a ISharedImmediateTexture for the appropriate job.
    /// </summary>
    /// <param name="jobID">ID of the job.</param>
    private static ISharedImmediateTexture GetJobIcon(byte jobID)
    {
        var iconID = 62100 + jobID;

        // Outside of bounds, either DoL, DoH, or we messed up.
        if (iconID < 62101 || iconID > 62142)
            iconID = 62145;
        // Adventurer
        if (jobID == 0)
            iconID = 62146;

        return GetIcon((uint)iconID);
    }

    /// <summary>
    /// Returns a ISharedImmediateTexture for the appropriate skill.
    /// </summary>
    /// <param name="skillID">ID of the skill.</param>
    private static ISharedImmediateTexture GetSkillIcon(uint skillID)
    {
        var actionList = Service.DataManager.GameData.Excel.GetSheet<Action>();
        var skill = actionList.GetRow(skillID);
        // Check if the icon isn't Cure's AND isn't actually Cure
        if (skill.Icon == 405 && skill.RowId != 120)
            return null;
        return GetIcon((uint)skill.Icon);
    }

    /// <summary>
    /// Returns a ISharedImmediateTexture for the appropriate status.
    /// </summary>
    /// <param name="statusID">ID of the status.</param>
    private static ISharedImmediateTexture GetStatusIcon(uint statusID)
    {
        var statusList = Service.DataManager.GameData.Excel.GetSheet<Status>();
        var status = statusList.GetRow(statusID);

        if (status.ClassJobCategory.Value.Name.RawString.Length == 3)
            return GetIcon((uint)status.Icon);
        else
            return GetIcon((uint)statusID);
    }

    /// <summary>
    /// Returns the localized string name for the appropriate skill/status.
    /// </summary>
    /// <param name="skillID">ID of the skill.</param>
    private static string GetSkillName(uint skillID)
    {
        if (skillID > 60000)
            return String.Empty;

        Language language = (Language)Service.ClientState.ClientLanguage + 1;
        var actionList = Service.DataManager.GameData.Excel.GetSheet<Action>(language);
        var skill = actionList.GetRow(skillID);
        return skill.Name;

    }

    /// <summary>
    /// Returns the localized string name for the appropriate skill/status.
    /// </summary>
    /// <param name="skillID">ID of the skill.</param>
    private static string GetStatusName(uint skillID)
    {
        if (skillID > 60000)
            return String.Empty;

        Language language = (Language)Service.ClientState.ClientLanguage + 1;
        var statusList = Service.DataManager.GameData.Excel.GetSheet<Status>(language);
        var status = statusList.GetRow(skillID);
        return status.Name;

    }

    /// <summary>
    /// Returns a ISharedImmediateTexture for the appropriate icon.
    /// </summary>
    /// <param name="iconID">ID of the icon.</param>
    private static ISharedImmediateTexture GetIcon(uint iconID)
        => Service.TextureProvider.GetFromGameIcon(new GameIconLookup(iconID, false, true));
}
