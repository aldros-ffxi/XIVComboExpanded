using System;
using System.Collections.Generic;

namespace XIVComboExpandedPlugin.Attributes;

/// <summary>
/// Attribute designating icons to display in action presets.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class IconsComboAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IconsComboAttribute"/> class with a single icon.
    /// </summary>
    /// <param name="icon">Icon that should be displayed next to the action preset.</param>
    internal IconsComboAttribute(uint icon)
    {
        this.Icons = [icon];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IconsComboAttribute"/> class with up to 3 icons.
    /// </summary>
    /// <param name="icons">Array of icon that should be displayed next to the action preset.</param>
    internal IconsComboAttribute(uint[] icons)
    {
            this.Icons = icons;
    }

    /// <summary>
    /// Gets the icon ID.
    /// </summary>
    public uint[] Icons { get; }

    public const uint
        // Combo-Specific
        Opo = 70123,
        PartnerChocobo = 70104,

        // General utility icons
        Party = 61522,
        Duty = 61585,
        Blank = 61699,
        Blank2 = 61034,
        Ally = 61701,
        Enemy = 61710,
        ArrowLeft = 66301,
        ArrowRight = 66302,
        ArrowUp = 66303,
        ArrowDown = 66304,
        Cross = 66308,
        Forbidden = 66309,
        Danger = 66310,
        Plus = 66315,
        Minus = 66316,
        Clock = 66317,
        Idea = 66318,
        Checkmark = 66311,
        Chatbubble = 66321,
        ChatbubbleCute = 63933,
        AoE = 60495,
        ST = 60429,
        InBattle = 61510,
        OutOfBattle = 61822,
        Cycle = 66329;

}