using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class NIN
{
    public const byte ClassID = 29;
    public const byte JobID = 30;

    public const uint
        SpinningEdge = 2240,
        GustSlash = 2242,
        Hide = 2245,
        Assassinate = 8814,
        Mug = 2248,
        DeathBlossom = 2254,
        AeolianEdge = 2255,
        TrickAttack = 2258,
        Ninjutsu = 2260,
        Chi = 2261,
        JinNormal = 2263,
        Kassatsu = 2264,
        Suiton = 2271,
        ArmorCrush = 3563,
        DreamWithinADream = 3566,
        TenChiJin = 7403,
        HakkeMujinsatsu = 16488,
        Meisui = 16489,
        Jin = 18807,
        Bunshin = 16493,
        Huraijin = 25876,
        PhantomKamaitachi = 25774,
        ForkedRaiju = 25777,
        FleetingRaiju = 25778,
        Dokumori = 36957;

    public static class Buffs
    {
        public const ushort
            Mudra = 496,
            Kassatsu = 497,
            Suiton = 507,
            Hidden = 614,
            Bunshin = 1954,
            RaijuReady = 2690,
            ShadowWalker = 3848,
            Higi = 3850;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            GustSlash = 4,
            Hide = 10,
            Mug = 15,
            AeolianEdge = 26,
            Ninjutsu = 30,
            Suiton = 45,
            HakkeMujinsatsu = 52,
            ArmorCrush = 54,
            Huraijin = 60,
            Dokumori = 66,
            TenChiJin = 70,
            Meisui = 72,
            EnhancedKassatsu = 76,
            Bunshin = 80,
            PhantomKamaitachi = 82,
            Raiju = 90,
            KunaisBane = 92;
    }
}

internal class NinjaAeolianEdge : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.AeolianEdge)
        {
            var gauge = GetJobGauge<NINGauge>();

            if (IsEnabled(CustomComboPreset.NinjaAeolianNinjutsuFeature))
            {
                if (level >= NIN.Levels.Ninjutsu && HasEffect(NIN.Buffs.Mudra))
                    return OriginalHook(NIN.Ninjutsu);
            }

            if (IsEnabled(CustomComboPreset.NinjaAeolianEdgeRaijuFeature))
            {
                if (level >= NIN.Levels.Raiju && HasEffect(NIN.Buffs.RaijuReady))
                    return NIN.FleetingRaiju;
            }

            if (IsEnabled(CustomComboPreset.NinjaAeolianEdgeCombo))
            {
                if (comboTime > 0)
                {
                    if (IsEnabled(CustomComboPreset.NinjaKazematoiFeature) && lastComboMove == NIN.GustSlash && level >= NIN.Levels.ArmorCrush && gauge.Kazematoi > 0)
                        return NIN.AeolianEdge;

                    if (lastComboMove == NIN.GustSlash && level >= NIN.Levels.AeolianEdge)
                        return NIN.AeolianEdge;

                    if (lastComboMove == NIN.SpinningEdge && level >= NIN.Levels.GustSlash)
                        return NIN.GustSlash;
                }

                return NIN.SpinningEdge;
            }
        }

        return actionID;
    }
}

internal class NinjaArmorCrush : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.ArmorCrush)
        {
            if (IsEnabled(CustomComboPreset.NinjaArmorCrushRaijuFeature))
            {
                if (level >= NIN.Levels.Raiju && HasEffect(NIN.Buffs.RaijuReady))
                    return NIN.ForkedRaiju;
            }

            if (IsEnabled(CustomComboPreset.NinjaArmorCrushNinjutsuFeature))
            {
                if (level >= NIN.Levels.Ninjutsu && HasEffect(NIN.Buffs.Mudra))
                    return OriginalHook(NIN.Ninjutsu);
            }

            if (IsEnabled(CustomComboPreset.NinjaArmorCrushCombo))
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == NIN.GustSlash && level >= NIN.Levels.ArmorCrush)
                        return NIN.ArmorCrush;

                    if (lastComboMove == NIN.SpinningEdge && level >= NIN.Levels.GustSlash)
                        return NIN.GustSlash;
                }

                return NIN.SpinningEdge;
            }
        }

        return actionID;
    }
}

internal class NinjaHakkeMujinsatsu : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.HakkeMujinsatsu)
        {
            if (IsEnabled(CustomComboPreset.NinjaHakkeMujinsatsuNinjutsuFeature))
            {
                if (level >= NIN.Levels.Ninjutsu && HasEffect(NIN.Buffs.Mudra))
                    return OriginalHook(NIN.Ninjutsu);
            }

            if (IsEnabled(CustomComboPreset.NinjaHakkeMujinsatsuCombo))
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == NIN.DeathBlossom && level >= NIN.Levels.HakkeMujinsatsu)
                        return NIN.HakkeMujinsatsu;
                }

                return NIN.DeathBlossom;
            }
        }

        return actionID;
    }
}

internal class NinjaKassatsu : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaKassatsuTrickFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.Kassatsu)
        {
            if ((level >= NIN.Levels.Hide && HasEffect(NIN.Buffs.Hidden)) ||
                (level >= NIN.Levels.Suiton && HasEffect(NIN.Buffs.ShadowWalker)))
                return OriginalHook(NIN.TrickAttack);
        }

        return actionID;
    }
}

internal class NinjaHide : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.Hide)
        {
            if (IsEnabled(CustomComboPreset.NinjaHideNinjutsuFeature))
            {
                if (level >= NIN.Levels.Ninjutsu && HasEffect(NIN.Buffs.Mudra))
                    return OriginalHook(NIN.Ninjutsu);
            }

            if (IsEnabled(CustomComboPreset.NinjaHideMugFeature) && (InCombat() || HasEffect(NIN.Buffs.Hidden)))
            {
                if (level >= NIN.Levels.Mug)
                {
                    if (level >= NIN.Levels.Dokumori)
                        return NIN.Dokumori;
                }

                return NIN.Mug;
            }
        }

        return actionID;
    }
}

internal class NinjaChi : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaKassatsuChiJinFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.Chi)
        {
            if (level >= NIN.Levels.EnhancedKassatsu && HasEffect(NIN.Buffs.Kassatsu))
                return NIN.Jin;
        }

        return actionID;
    }
}

internal class NinjaTenChiJin : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaTCJMeisuiFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.TenChiJin)
        {
            if (level >= NIN.Levels.Meisui && HasEffect(NIN.Buffs.ShadowWalker))
                return NIN.Meisui;
        }

        return actionID;
    }
}

internal class NinjaNinjutsu : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
    {
        if (actionID == NIN.Ninjutsu)
        {
            if (level >= NIN.Levels.Raiju && HasEffect(NIN.Buffs.RaijuReady) && !HasEffect(NIN.Buffs.Mudra))
            {
                if (IsEnabled(CustomComboPreset.NinjaNinjutsuForkedRaijuFeature))
                    return NIN.ForkedRaiju;

                if (IsEnabled(CustomComboPreset.NinjaNinjutsuFleetingRaijuFeature))
                    return NIN.FleetingRaiju;
            }
        }

        return actionID;
    }
}
