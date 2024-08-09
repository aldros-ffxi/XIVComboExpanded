using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using XIVComboExpandedPlugin.Combos;

namespace XIVComboExpandedPlugin;

/// <summary>
/// This class facilitates the icon replacing.
/// </summary>
internal sealed partial class IconReplacer : IDisposable
{
    private readonly unsafe ActionManager* clientStructActionManager;
    private readonly List<CustomCombo> customCombos;
    private readonly Hook<IsIconReplaceableDelegate> isIconReplaceableHook;
    private readonly Hook<GetIconDelegate> getIconHook;

    private IntPtr actionManager = IntPtr.Zero;

    /// <summary>
    /// Initializes a new instance of the <see cref="IconReplacer"/> class.
    /// </summary>
    public unsafe IconReplacer(IGameInteropProvider gameInteropProvider)
    {
        this.clientStructActionManager = ActionManager.Instance();

        this.customCombos = Assembly.GetAssembly(typeof(CustomCombo))!.GetTypes()
            .Where(t => !t.IsAbstract && IsDescendant(t, typeof(CustomCombo)))
            .Select(t => Activator.CreateInstance(t))
            .Cast<CustomCombo>()
            .ToList();

        this.getIconHook = gameInteropProvider.HookFromAddress<GetIconDelegate>(FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Addresses.GetAdjustedActionId.Value, this.GetIconDetour);
        this.isIconReplaceableHook = gameInteropProvider.HookFromAddress<IsIconReplaceableDelegate>(Service.Address.IsActionIdReplaceable, this.IsIconReplaceableDetour);

        this.getIconHook.Enable();
        this.isIconReplaceableHook.Enable();
    }

    /// <summary>
    /// Gets bool determining if action is greyed out or not.
    /// </summary>
    /// <param name="actionID">Action ID.</param>
    /// <param name="targetID">Target ID.</param>
    /// <returns>A bool value of whether the action can be used or not.</returns>
    internal unsafe bool CanUseAction(uint actionID, uint targetID = 0xE000_0000)
    {
        return clientStructActionManager->GetActionStatus(ActionType.Action, actionID, targetID, false, true) == 0;
    }

    private static bool IsDescendant(Type clazz, Type ancestor)
    {
        if (clazz.BaseType == null) return false;
        if (clazz.BaseType == ancestor) return true;
        return IsDescendant(clazz.BaseType, ancestor);
    }

    private delegate ulong IsIconReplaceableDelegate(uint actionID);

    private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);

    /// <inheritdoc/>
    public void Dispose()
    {
        this.getIconHook?.Dispose();
        this.isIconReplaceableHook?.Dispose();
    }

    /// <summary>
    /// Calls the original hook.
    /// </summary>
    /// <param name="actionID">Action ID.</param>
    /// <returns>The result from the hook.</returns>
    internal uint OriginalHook(uint actionID)
        => this.getIconHook.Original(this.actionManager, actionID);

    private unsafe uint GetIconDetour(IntPtr actionManager, uint actionID)
    {
        this.actionManager = actionManager;

        try
        {
            if (Service.ClientState.LocalPlayer == null)
                return this.OriginalHook(actionID);

            var lastComboMove = *(uint*)Service.Address.LastComboMove;
            var comboTime = *(float*)Service.Address.ComboTimer;
            var level = Service.ClientState.LocalPlayer?.Level ?? 0;

            foreach (var combo in this.customCombos)
            {
                if (combo.TryInvoke(actionID, level, lastComboMove, comboTime, out var newActionID))
                    return newActionID;
            }

            return this.OriginalHook(actionID);
        }
        catch (Exception ex)
        {
            Service.PluginLog.Error(ex, "Don't crash the game");
            return this.OriginalHook(actionID);
        }
    }

    private ulong IsIconReplaceableDetour(uint actionID) => 1;
}
