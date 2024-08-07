using System;
using System.Collections.Generic;
using System.Linq;

using Dalamud.Configuration;
using Dalamud.Utility;
using Newtonsoft.Json;
using XIVComboExpandedPlugin.Attributes;
using XIVComboExpandedPlugin.Combos;
using XIVComboExpandedPlugin.Interface;

namespace XIVComboExpandedPlugin;

/// <summary>
/// Plugin configuration.
/// </summary>
[Serializable]
public class PluginConfiguration : IPluginConfiguration
{
    private static readonly HashSet<CustomComboPreset> SecretCombos;
    private static readonly HashSet<CustomComboPreset> AccessibilityCombos;
    private static readonly HashSet<CustomComboPreset> ExpandedCombos;
    private static readonly Dictionary<CustomComboPreset, CustomComboPreset[]> ConflictingCombos;
    private static readonly Dictionary<CustomComboPreset, CustomComboPreset?> ParentCombos;  // child: parent

    static PluginConfiguration()
    {
        SecretCombos = Enum.GetValues<CustomComboPreset>()
            .Where(preset => preset.GetAttribute<SecretCustomComboAttribute>() != default)
            .ToHashSet();

        AccessibilityCombos = Enum.GetValues<CustomComboPreset>()
            .Where(preset => preset.GetAttribute<AccessibilityCustomComboAttribute>() != default)
            .ToHashSet();

        ExpandedCombos = Enum.GetValues<CustomComboPreset>()
            .Where(preset => preset.GetAttribute<ExpandedCustomComboAttribute>() != default)
            .ToHashSet();

        ConflictingCombos = Enum.GetValues<CustomComboPreset>()
            .Distinct() // Prevent ArgumentExceptions from adding the same int twice, should not be seen anymore
            .ToDictionary(
                preset => preset,
                preset => preset.GetAttribute<ConflictingCombosAttribute>()?.ConflictingPresets ?? Array.Empty<CustomComboPreset>());

        ParentCombos = Enum.GetValues<CustomComboPreset>()
            .Distinct() // Prevent ArgumentExceptions from adding the same int twice, should not be seen anymore
            .ToDictionary(
                preset => preset,
                preset => preset.GetAttribute<ParentComboAttribute>()?.ParentPreset);
    }

    /// <summary>
    /// Gets or sets the configuration version.
    /// </summary>
    public int Version { get; set; } = 5;

    /// <summary>
    /// Gets or sets the collection of enabled combos.
    /// </summary>
    [JsonProperty("EnabledActionsV5")]
    public HashSet<CustomComboPreset> EnabledActions { get; set; } = new();

    /// <summary>
    /// Gets or sets the collection of enabled combos.
    /// </summary>
    [JsonProperty("EnabledActionsV4")]
    public HashSet<CustomComboPreset> EnabledActions4 { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether to enable the plugin or not.
    /// </summary>
    [JsonProperty("Plugin")]
    public bool EnablePlugin { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to allow and display expanded combos.
    /// </summary>
    [JsonProperty("Expanded")]
    public bool EnableExpandedCombos { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to allow and display accessibility combos.
    /// </summary>
    [JsonProperty("Accessibility")]
    public bool EnableAccessibilityCombos { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to allow and display secret combos.
    /// </summary>
    [JsonProperty("Secret")]
    public bool EnableSecretCombos { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to allow and display secret combos.
    /// </summary>
    [JsonProperty("SecretUnlock")]
    public bool UnlockSecretCombos { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating which is the current tab.
    /// </summary>
    [JsonProperty("Tab")]
    public string CurrentJobTab { get; set; } = "Adventurer";

    /// <summary>
    /// Gets or sets a value indicating whether the plugin automatically changes to the current job upon opening the GUI.
    /// </summary>
    public bool AutoJobChange { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to hide the Ko-Fi link.
    /// </summary>
    public bool HideKofi { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to hide the children of a feature if it is disabled.
    /// </summary>
    public bool HideChildren { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to hide the icons of a feature.
    /// </summary>
    public bool HideIcons { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to increase the icons of the jobs on the sidebar or not.
    /// </summary>
    public bool BigJobIcons { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether increase the icons featured in the combo lists or not.
    /// </summary>
    public bool BigComboIcons { get; set; } = false;

    /// <summary>
    /// Gets or sets an array of 4 ability IDs to interact with the <see cref="CustomComboPreset.DancerDanceComboCompatibility"/> combo.
    /// </summary>
    public uint[] DancerDanceCompatActionIDs { get; set; } = new uint[]
    {
        DNC.Cascade,
        DNC.Flourish,
        DNC.FanDance1,
        DNC.FanDance2,
    };

    /// <summary>
    /// Save the configuration to disk.
    /// </summary>
    public void Save()
        => Service.Interface.SavePluginConfig(this);

    /// <summary>
    /// Gets a value indicating whether a preset is enabled.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The boolean representation.</returns>
    public bool IsEnabled(CustomComboPreset preset)
        => this.EnabledActions.Contains(preset)
        && (this.EnableSecretCombos || !this.IsSecret(preset))
        && (this.EnableExpandedCombos || !this.IsExpanded(preset))
        && (this.EnableAccessibilityCombos || !this.IsAccessible(preset));

    /// <summary>
    /// Gets a value indicating whether a preset is secret.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The boolean representation.</returns>
    public bool IsSecret(CustomComboPreset preset)
        => SecretCombos.Contains(preset);

    /// <summary>
    /// Gets a value indicating whether a preset is an expanded one.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The boolean representation.</returns>
    public bool IsExpanded(CustomComboPreset preset)
        => ExpandedCombos.Contains(preset);

    /// <summary>
    /// Gets a value indicating whether a preset is easy.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The boolean representation.</returns>
    public bool IsAccessible(CustomComboPreset preset)
        => AccessibilityCombos.Contains(preset);

    /// <summary>
    /// Gets an array of conflicting combo presets.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The conflicting presets.</returns>
    public CustomComboPreset[] GetConflicts(CustomComboPreset preset)
        => ConflictingCombos[preset];

    /// <summary>
    /// Gets the parent combo preset if it exists, or null.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The parent preset.</returns>
    public CustomComboPreset? GetParent(CustomComboPreset preset)
        => ParentCombos[preset];
}
