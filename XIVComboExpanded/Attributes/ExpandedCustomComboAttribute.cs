using System;

namespace XIVComboExpandedPlugin.Attributes;

/// <summary>
/// Attribute designating easy, non-optimal combos.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class ExpandedCustomComboAttribute : Attribute
{
}
