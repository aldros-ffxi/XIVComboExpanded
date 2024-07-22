using System;
using System.Linq;

using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class MNK
{
    public const byte ClassID = 2;
    public const byte JobID = 20;

    public const uint
        Bootshine = 53,
        TrueStrike = 54,
        SnapPunch = 56,
        TwinSnakes = 61,
        ArmOfTheDestroyer = 62,
        Demolish = 66,
        PerfectBalance = 69,
        Rockbreaker = 70,
        DragonKick = 74,
        Meditation = 3546,
        FormShift = 4262,
        RiddleOfFire = 7395,
        Brotherhood = 7396,
        FourPointFury = 16473,
        Enlightenment = 16474,
        SteeledPeak = 25761,
        HowlingFist = 25763,
        MasterfulBlitz = 25764,
        RiddleOfWind = 25766,
        ShadowOfTheDestroyer = 25767,
        SteeledMeditation = 36942,
        EnlightenedMeditation = 36943,
        LeapingOpo = 36945,
        RisingRaptor = 36946,
        PouncingCoeurl = 36947;

    public static class Buffs
    {
        public const ushort
            OpoOpoForm = 107,
            RaptorForm = 108,
            CoerlForm = 109,
            PerfectBalance = 110,
            LeadenFist = 1861,
            FormlessFist = 2513,
            DisciplinedFist = 3001;
    }

    public static class Debuffs
    {
        public const ushort
            Demolish = 246;
    }

    public static class Levels
    {
        public const byte
            Bootshine = 1,
            TrueStrike = 4,
            SnapPunch = 6,
            Meditation = 15,
            SteeledMeditation = 15,
            TwinSnakes = 18,
            ArmOfTheDestroyer = 26,
            Rockbreaker = 30,
            Demolish = 30,
            FourPointFury = 45,
            HowlingFist = 40,
            DragonKick = 50,
            PerfectBalance = 50,
            FormShift = 52,
            EnhancedPerfectBalance = 60,
            MasterfulBlitz = 60,
            RiddleOfFire = 68,
            Brotherhood = 70,
            Enlightenment = 70,
            RiddleOfWind = 72,
            EnlightenedMeditation = 74,
            ShadowOfTheDestroyer = 82;
    }
}

internal class MonkOpoCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkOpoFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.Bootshine || actionID == MNK.LeapingOpo)
        {
            var gauge = GetJobGauge<MNKGauge>();

            // Opo Chakra
            if (IsEnabled(CustomComboPreset.MonkOpoChakraFeature))
            {
                if (gauge.Chakra > 4 && InCombat())
                    return OriginalHook(MNK.SteeledPeak);
            }

            if (IsEnabled(CustomComboPreset.MonkBootshineMeditationFeature))
            {
                if (level >= MNK.Levels.SteeledMeditation && gauge.Chakra < 5 && !InCombat())
                    return MNK.SteeledMeditation;
            }

            if (IsEnabled(CustomComboPreset.MonkBootshineFormShiftFeature))
            {
                if (level >= MNK.Levels.FormShift && !HasEffect(MNK.Buffs.FormlessFist) && !InCombat())
                    return MNK.FormShift;
            }

            if (IsEnabled(CustomComboPreset.MonkSTBalanceFeature))
            {
                if (!gauge.BeastChakra.Contains(BeastChakra.NONE) && level >= MNK.Levels.MasterfulBlitz)
                    // Chakra actions
                    return OriginalHook(MNK.MasterfulBlitz);
            }

            if (gauge.OpoOpoFury == 0 && level >= MNK.Levels.DragonKick)
                return MNK.DragonKick;
        }

        return actionID;
    }
}

internal class MonkRaptorCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkRaptorFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.TrueStrike || actionID == MNK.RisingRaptor)
        {
            var gauge = GetJobGauge<MNKGauge>();

            if (IsEnabled(CustomComboPreset.MonkSTBalanceFeature))
            {
                if (!gauge.BeastChakra.Contains(BeastChakra.NONE) && level >= MNK.Levels.MasterfulBlitz)
                    // Chakra actions
                    return OriginalHook(MNK.MasterfulBlitz);
            }

            if (gauge.RaptorFury == 0 && level > MNK.Levels.TwinSnakes)
                return MNK.TwinSnakes;
        }

        return actionID;
    }
}

internal class MonkCoeurlCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkCoeurlFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.SnapPunch || actionID == MNK.PouncingCoeurl)
        {
            var gauge = GetJobGauge<MNKGauge>();

            if (IsEnabled(CustomComboPreset.MonkSTBalanceFeature))
            {
                if (!gauge.BeastChakra.Contains(BeastChakra.NONE) && level >= MNK.Levels.MasterfulBlitz)
                    // Chakra actions
                    return OriginalHook(MNK.MasterfulBlitz);
            }

            if (gauge.CoeurlFury == 0 && level > MNK.Levels.Demolish)
                return MNK.Demolish;
        }

        return actionID;
    }
}

internal class MonkMonkeyMode : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkMonkeyMode;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.Bootshine || actionID == MNK.LeapingOpo)
        {
            var gauge = GetJobGauge<MNKGauge>();

            // Auto Chakra
            if (IsEnabled(CustomComboPreset.MonkMonkeyAutoChakraFeature))
            {
                    if (gauge.Chakra > 4 && InCombat())
                        return OriginalHook(MNK.SteeledPeak);
            }

            // Masterful Blitz
            if (IsEnabled(CustomComboPreset.MonkSTBalanceFeature))
            {
                    if (!gauge.BeastChakra.Contains(BeastChakra.NONE) && level >= MNK.Levels.MasterfulBlitz)
                        return OriginalHook(MNK.MasterfulBlitz);
            }

            if (level >= MNK.Levels.PerfectBalance && HasEffect(MNK.Buffs.PerfectBalance))
            {
                // Solar Nadi
                if (level >= MNK.Levels.EnhancedPerfectBalance && !gauge.Nadi.HasFlag(Nadi.SOLAR))
                {
                    if (level >= MNK.Levels.TwinSnakes && !gauge.BeastChakra.Contains(BeastChakra.RAPTOR))
                    {
                        if (gauge.RaptorFury == 0 && level >= MNK.Levels.TwinSnakes)
                            return MNK.TwinSnakes;
                        else
                            return OriginalHook(MNK.TrueStrike);
                    }

                    if (level >= MNK.Levels.Demolish && !gauge.BeastChakra.Contains(BeastChakra.COEURL))
                    {
                        if (gauge.CoeurlFury == 0 && level >= MNK.Levels.Demolish)
                            return MNK.Demolish;
                        else
                            return OriginalHook(MNK.SnapPunch);
                    }

                    if (level >= MNK.Levels.DragonKick && !gauge.BeastChakra.Contains(BeastChakra.OPOOPO))
                       {
                        if (gauge.OpoOpoFury == 0 && level >= MNK.Levels.DragonKick)
                            return MNK.DragonKick;
                        else
                            return OriginalHook(MNK.Bootshine);
                    }
                }

                // Lunar Nadi or both
                else if (level >= MNK.Levels.EnhancedPerfectBalance && gauge.Nadi.HasFlag(Nadi.SOLAR))
                {
                    if (gauge.OpoOpoFury == 0 && level >= MNK.Levels.DragonKick)
                        return MNK.DragonKick;
                    else
                        return OriginalHook(MNK.Bootshine);
                }
            }

            if (IsEnabled(CustomComboPreset.MonkMonkeyMeditationFeature))
            {
                if (level >= MNK.Levels.SteeledMeditation && gauge.Chakra < 5 && !InCombat())
                    return MNK.SteeledMeditation;
            }

            if (IsEnabled(CustomComboPreset.MonkMonkeyFormShiftFeature))
            {
                if (level >= MNK.Levels.FormShift && !HasEffect(MNK.Buffs.FormlessFist) && !InCombat())
                    return MNK.FormShift;
            }

            if (HasEffect(MNK.Buffs.RaptorForm))
            {
                if (gauge.RaptorFury == 0 && level >= MNK.Levels.TwinSnakes)
                    return MNK.TwinSnakes;
                else
                    return OriginalHook(MNK.TrueStrike);
            }

            if (HasEffect(MNK.Buffs.CoerlForm))
            {
                if (gauge.CoeurlFury == 0 && level >= MNK.Levels.Demolish)
                    return MNK.Demolish;
                else
                    return OriginalHook(MNK.SnapPunch);
            }

            if (HasEffect(MNK.Buffs.OpoOpoForm) || (!HasEffect(MNK.Buffs.OpoOpoForm) || !HasEffect(MNK.Buffs.CoerlForm) || !HasEffect(MNK.Buffs.RaptorForm)))
            {
                if (gauge.OpoOpoFury == 0 && level >= MNK.Levels.DragonKick)
                    return MNK.DragonKick;
                else
                    return OriginalHook(MNK.Bootshine);
            }
        }

        return actionID;
    }
}

internal class MonkAoECombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkAoECombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.MasterfulBlitz)
        {
            var gauge = GetJobGauge<MNKGauge>();

            // Auto Chakra
            if (IsEnabled(CustomComboPreset.MonkAoEAutoChakraFeature))
            {
                if (gauge.Chakra > 4 && level >= MNK.Levels.HowlingFist && InCombat())
                    return OriginalHook(MNK.HowlingFist);
            }

            if (IsEnabled(CustomComboPreset.MonkAoEMeditationFeature))
            {
                if (level >= MNK.Levels.EnlightenedMeditation && gauge.Chakra < 5 && !InCombat())
                    return MNK.EnlightenedMeditation;
            }

            if (IsEnabled(CustomComboPreset.MonkAoEFormShiftFeature))
            {
                if (level >= MNK.Levels.FormShift && !HasEffect(MNK.Buffs.FormlessFist) && !InCombat())
                    return MNK.FormShift;
            }

            // Blitz
            if (level >= MNK.Levels.MasterfulBlitz && !gauge.BeastChakra.Contains(BeastChakra.NONE))
                return OriginalHook(MNK.MasterfulBlitz);

            if (level >= MNK.Levels.PerfectBalance && HasEffect(MNK.Buffs.PerfectBalance))
            {
                // Solar
                if (level >= MNK.Levels.EnhancedPerfectBalance && !gauge.Nadi.HasFlag(Nadi.SOLAR))
                {
                    if (level >= MNK.Levels.FourPointFury && !gauge.BeastChakra.Contains(BeastChakra.RAPTOR))
                        return MNK.FourPointFury;

                    if (level >= MNK.Levels.Rockbreaker && !gauge.BeastChakra.Contains(BeastChakra.COEURL))
                        return MNK.Rockbreaker;

                    if (level >= MNK.Levels.ArmOfTheDestroyer && !gauge.BeastChakra.Contains(BeastChakra.OPOOPO))
                        // Shadow of the Destroyer
                        return OriginalHook(MNK.ArmOfTheDestroyer);

                    return level >= MNK.Levels.ShadowOfTheDestroyer
                        ? MNK.ShadowOfTheDestroyer
                        : MNK.Rockbreaker;
                }

                // Lunar.  Also used if we have both Nadi as Tornado Kick/Phantom Rush isn't picky, or under 60.
                return level >= MNK.Levels.ShadowOfTheDestroyer
                    ? MNK.ShadowOfTheDestroyer
                    : MNK.Rockbreaker;
            }

            // FPF with FormShift
            if (level >= MNK.Levels.FormShift && HasEffect(MNK.Buffs.FormlessFist))
            {
                return level >= MNK.Levels.ShadowOfTheDestroyer
                    ? MNK.ShadowOfTheDestroyer
                    : MNK.Rockbreaker;
            }

            // 1-2-3 combo
            if (level >= MNK.Levels.FourPointFury && HasEffect(MNK.Buffs.RaptorForm))
                return MNK.FourPointFury;

            if (level >= MNK.Levels.ArmOfTheDestroyer && HasEffect(MNK.Buffs.OpoOpoForm))
                // Shadow of the Destroyer
                return OriginalHook(MNK.ArmOfTheDestroyer);

            if (level >= MNK.Levels.Rockbreaker && HasEffect(MNK.Buffs.CoerlForm))
                return MNK.Rockbreaker;

            // Shadow of the Destroyer
            return OriginalHook(MNK.ArmOfTheDestroyer);
        }

        return actionID;
    }
}

internal class MonkPerfectBalance : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkPerfectBalanceFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.PerfectBalance)
        {
            var gauge = GetJobGauge<MNKGauge>();

            if (!gauge.BeastChakra.Contains(BeastChakra.NONE) && level >= MNK.Levels.MasterfulBlitz)
                // Chakra actions
                return OriginalHook(MNK.MasterfulBlitz);
        }

        return actionID;
    }
}

internal class MonkRiddleOfFire : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.RiddleOfFire)
        {
            var brotherhood = IsEnabled(CustomComboPreset.MonkRiddleOfFireBrotherhood);
            var wind = IsEnabled(CustomComboPreset.MonkRiddleOfFireWind);

            if (brotherhood && wind)
            {
                if (level >= MNK.Levels.RiddleOfWind)
                    return CalcBestAction(actionID, MNK.RiddleOfFire, MNK.Brotherhood, MNK.RiddleOfWind);

                if (level >= MNK.Levels.Brotherhood)
                    return CalcBestAction(actionID, MNK.RiddleOfFire, MNK.Brotherhood);

                return actionID;
            }

            if (brotherhood)
            {
                if (level >= MNK.Levels.Brotherhood)
                    return CalcBestAction(actionID, MNK.RiddleOfFire, MNK.Brotherhood);

                return actionID;
            }

            if (wind)
            {
                if (level >= MNK.Levels.RiddleOfWind)
                    return CalcBestAction(actionID, MNK.RiddleOfFire, MNK.RiddleOfWind);

                return actionID;
            }
        }

        return actionID;
    }
}
