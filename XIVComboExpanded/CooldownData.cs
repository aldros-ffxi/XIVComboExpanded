using System.Runtime.InteropServices;

using FFXIVClientStructs.FFXIV.Client.Game;

namespace XIVComboExpandedPlugin;

/// <summary>
/// Internal cooldown data.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
internal struct CooldownData
{
    [FieldOffset(0x0)]
    private readonly bool isCooldown;

    [FieldOffset(0x4)]
    private readonly uint actionID;

    [FieldOffset(0x8)]
    private readonly float cooldownElapsed;

    [FieldOffset(0xC)]
    private readonly float cooldownTotal;

    /// <summary>
    /// Gets the action ID on cooldown.
    /// </summary>
    public uint ActionID => this.actionID;

    /// <summary>
    /// Gets the cast time in seconds, adjusted by spell cast time modifiers (ex. spell speed/skill speed).
    /// </summary>
    public unsafe float CastTime => ActionManager.GetAdjustedCastTime(ActionType.Action, this.ActionID) / 1000f;

    /// <summary>
    /// Gets the resource cost of the action.
    /// </summary>
    public unsafe float Cost => ActionManager.GetActionCost(ActionType.Action, this.ActionID, 1, 0, 0, 0);

    /// <summary>
    /// Gets the base cooldown time of an action in seconds, adjusted for spell recast modifiers
    /// (ex. spell speed, if relevant).
    /// </summary>
    public float BaseCooldown => ActionManager.GetAdjustedRecastTime(ActionType.Action, this.ActionID) / 1000f;

    /// <summary>
    /// Gets the total cooldown of an action across all charges, which is equivalent to the BaseCooldown multiplied
    /// by the MaxCharges.
    /// </summary>
    public float TotalBaseCooldown => this.BaseCooldown * this.MaxCharges;

    /// <summary>
    /// Gets the maximum number of charges for an action at the current level.
    /// </summary>
    /// <returns>Number of charges.</returns>
    public ushort MaxCharges => Service.ComboCache.GetMaxCharges(this.ActionID).Current;

    /// <summary>
    /// Gets a value indicating whether an action utilizes charges, not whether charges are currently available.
    /// </summary>
    public bool UsesCharges => this.MaxCharges > 1;

    /// <summary>
    /// Gets the currently remaining (ie. usable) number of charges for an action.
    /// </summary>
    public ushort RemainingCharges => !this.isCooldown ? this.MaxCharges : (ushort)(this.TotalCooldownElapsed / this.BaseCooldown);

    /// <summary>
    /// Gets a value indicating whether this action is off cooldown, or for charge-based actions, if the action
    /// has at least one usable charge available.
    /// </summary>
    public bool Available => !this.isCooldown || this.RemainingCharges > 0;

    /// <summary>
    /// Gets a value indicating whether the action is on cooldown, or for charge-based actions, if any charges
    /// are currently recharging.  IsCooldown being true is NOT the same as the action being unavailable, as a
    /// charged-based action can be both currently recovering a charge and also available for use.
    /// </summary>
    public bool IsCooldown => this.isCooldown;

    /// <summary>
    /// Gets the cooldown time remaining until all charges are replenished.
    /// </summary>
    public float TotalCooldownRemaining => !this.isCooldown ? 0 : this.TotalBaseCooldown - this.cooldownElapsed;

    /// <summary>
    /// Gets the cooldown time remaining until the currently recharging charge is replenished.  For actions that are
    /// not charge-based, this is mechanically equivalent to TotalCooldownRemaining.
    /// </summary>
    public float CooldownRemaining => this.TotalCooldownRemaining % this.BaseCooldown;

    /// <summary>
    /// Gets the overall elapsed cooldown.  The value will range from 0, immediately after all charges are used,
    /// up to the TotalBaseCooldown.  It is not known at this time if a return value of exactly 0 is possible.
    /// For abilities with charges, this will equal the time elapsed on the current charge's recharge, plus the
    /// BaseCooldown multiplied by the number of charges currently available.
    /// As an example, if an ability with 2 charges and a 20s recharge had 1 charge used 5 seconds ago
    /// (so it has 1 charge available, and 15s remaining until another charge is available), this field would
    /// return 25s (20 + 5).  If another charge were used at that exact moment, it would then return 5.
    /// </summary>
    public float TotalCooldownElapsed => !this.isCooldown ? this.TotalBaseCooldown : this.cooldownElapsed;

    /// <summary>
    /// Gets the elapsed time on the recharge of only the currently recharging charge.  For actions that are not
    /// charge-based, this is mechanically equivalent to TotalCooldownElapsed.
    /// </summary>
    public float CooldownElapsed => this.TotalCooldownElapsed % this.BaseCooldown;
}
