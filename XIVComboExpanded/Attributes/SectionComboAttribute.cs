using System;

namespace XIVComboExpandedPlugin.Attributes;

/// <summary>
/// Attribute designating easy, non-optimal combos.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class SectionComboAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SectionComboAttribute"/> class.
    /// </summary>
    /// <param name="section">Presets that should be contained in a specific section.</param>
    internal SectionComboAttribute(string section)
    {
        this.Section = section;
    }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public string Section { get; }
}