using Dalamud.Game.ClientState.JobGauge.Types;

using DreadCombo = Dalamud.Game.ClientState.JobGauge.Enums.DreadCombo;

namespace XIVComboExpandedPlugin.Combos;

internal static class VPR
{
    public const byte JobID = 41;

    public const uint
            SteelFangs = 34606,
            ReavingFangs = 34607,
            HuntersSting = 34608,
            SwiftskinsSting = 34609,
            FlankstingStrike = 34610,
            FlanksbaneFang = 34611,
            HindstingStrike = 34612,
            HindsbaneFang = 34613,

            SteelMaw = 34614,
            ReavingMaw = 34615,
            HuntersBite = 34616,
            SwiftskinsBite = 34617,
            JaggedMaw = 34618,
            BloodiedMaw = 34619,

            Dreadwinder = 34620,
            HuntersCoil = 34621,
            SwiftskinsCoil = 34622,
            PitOfDread = 34623,
            HuntersDen = 34624,
            SwiftskinsDen = 34625,

            SerpentsTail = 35920,
            DeathRattle = 34634,
            LastLash = 34635,
            Twinfang = 35921,
            Twinblood = 35922,
            TwinfangBite = 34636,
            TwinfangThresh = 34638,
            TwinbloodBite = 34637,
            TwinbloodThresh = 34639,

            UncoiledFury = 34633,
            UncoiledTwinfang = 34644,
            UncoiledTwinblood = 34645,

            SerpentsIre = 34647,
            Reawaken = 34626,
            FirstGeneration = 34627,
            SecondGeneration = 34628,
            ThirdGeneration = 34629,
            FourthGeneration = 34630,
            Ouroboros = 34631,
            FirstLegacy = 34640,
            SecondLegacy = 34641,
            ThirdLegacy = 34642,
            FourthLegacy = 34643,

            WrithingSnap = 34632,
            Slither = 34646;

    public static class Buffs
    {
        public const ushort
            FlankstungVenom = 3645,
            FlanksbaneVenom = 3646,
            HindstungVenom = 3647,
            HindsbaneVenom = 3648,
            GrimhuntersVenom = 3649,
            GrimskinsVenom = 3650,
            HuntersVenom = 3657,
            SwiftskinsVenom = 3658,
            FellhuntersVenom = 3659,
            FellskinsVenom = 3660,
            PoisedForTwinfang = 3665,
            PoisedForTwinblood = 3666,
            HuntersInstinct = 3668, // Double check, might also be 4120
            Swiftscaled = 3669,     // Might also be 4121
            Reawakened = 3670,
            ReadyToReawaken = 3671,
            HonedSteel = 3672,
            HonedReavers = 3772;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            SteelFangs = 1,
            HuntersSting = 5,
            ReavingFangs = 10,
            WrithingSnap = 15,
            SwiftskinsSting = 20,
            SteelMaw = 25,
            Single3rdCombo = 30, // Includes Flanksting, Flanksbane, Hindsting, and Hindsbane
            ReavingMaw = 35,
            Slither = 40,
            HuntersBite = 40,
            SwiftskinsBike = 45,
            AoE3rdCombo = 50,    // Jagged Maw and Bloodied Maw
            DeathRattle = 55,
            LastLash = 60,
            Dreadwinder = 65,    // Also includes Hunter's Coil and Swiftskin's Coil
            PitOfDread = 70,     // Also includes Hunter's Den and Swiftskin's Den
            TwinsSingle = 75,    // Twinfang Bite and Twinblood Bite
            TwinsAoE = 80,       // Twinfang Thresh and Twinblood Thresh
            UncoiledFury = 82,
            UncoiledTwins = 92,  // Uncoiled Twinfang and Uncoiled Twinblood
            SerpentsIre = 86,
            EnhancedRattle = 88, // Third stack of Rattling Coil can be accumulated
            Reawaken = 90,       // Also includes First Generation through Fourth Generation
            Ouroboros = 96,
            Legacies = 100;      // First through Fourth Legacy
    }
}

internal class AutoFangsFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperAutoFangsFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if ((actionID == VPR.SteelFangs || actionID == VPR.ReavingFangs) && OriginalHook(VPR.SteelFangs) == VPR.SteelFangs)
            return HasEffect(VPR.Buffs.HonedReavers) ? VPR.ReavingFangs : VPR.SteelFangs;

        return actionID;
    }
}

internal class AutoMawFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperAutoMawFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if ((actionID == VPR.SteelMaw || actionID == VPR.ReavingMaw) && OriginalHook(VPR.SteelMaw) == VPR.SteelMaw)
            return HasEffect(VPR.Buffs.HonedReavers) ? VPR.ReavingMaw : VPR.SteelMaw;

        return actionID;
    }
}

internal class SteelTailFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperSteelTailFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.SteelFangs)
        {
            if (OriginalHook(VPR.SerpentsTail) == VPR.DeathRattle)
                return VPR.DeathRattle;
        }

        if (actionID == VPR.ReavingFangs)
        {
            if (OriginalHook(VPR.SerpentsTail) == VPR.DeathRattle)
                return VPR.DeathRattle;
        }

        return actionID;
    }
}

internal class SteelTailAoEFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperSteelTailAoEFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.SteelMaw)
        {
            if (OriginalHook(VPR.SerpentsTail) == VPR.LastLash)
                return VPR.LastLash;
        }

        if (actionID == VPR.ReavingMaw)
        {
            if (OriginalHook(VPR.SerpentsTail) == VPR.LastLash)
                return VPR.LastLash;
        }

        return actionID;
    }
}

internal class TwinCoilFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperTwinCoilFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.HuntersCoil)
        {
            if (HasEffect(VPR.Buffs.HuntersVenom))
                return VPR.TwinfangBite;
            if (HasEffect(VPR.Buffs.SwiftskinsVenom))
                return VPR.TwinbloodBite;
            if (OriginalHook(VPR.Twinfang) == VPR.TwinfangBite)
                return VPR.TwinfangBite;
            if (OriginalHook(VPR.Twinblood) == VPR.TwinbloodBite)
                return VPR.TwinbloodBite;
        }

        if (actionID == VPR.SwiftskinsCoil)
        {
            if (HasEffect(VPR.Buffs.HuntersVenom))
                return VPR.TwinfangBite;
            if (HasEffect(VPR.Buffs.SwiftskinsVenom))
                return VPR.TwinbloodBite;
            if (OriginalHook(VPR.Twinfang) == VPR.TwinfangBite)
                return VPR.TwinfangBite;
            if (OriginalHook(VPR.Twinblood) == VPR.TwinbloodBite)
                return VPR.TwinbloodBite;
        }

        return actionID;
    }
}

internal class TwinDenFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperTwinDenFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.HuntersDen)
        {
            if (HasEffect(VPR.Buffs.FellhuntersVenom))
                return VPR.TwinfangThresh;
            if (HasEffect(VPR.Buffs.FellskinsVenom))
                return VPR.TwinbloodThresh;
            if (OriginalHook(VPR.Twinfang) == VPR.TwinfangThresh)
                return VPR.TwinfangThresh;
            if (OriginalHook(VPR.Twinblood) == VPR.TwinbloodThresh)
                return VPR.TwinbloodThresh;
        }

        if (actionID == VPR.SwiftskinsDen)
        {
            if (HasEffect(VPR.Buffs.FellhuntersVenom))
                return VPR.TwinfangThresh;
            if (HasEffect(VPR.Buffs.FellskinsVenom))
                return VPR.TwinbloodThresh;
            if (OriginalHook(VPR.Twinfang) == VPR.TwinfangThresh)
                return VPR.TwinfangThresh;
            if (OriginalHook(VPR.Twinblood) == VPR.TwinbloodThresh)
                return VPR.TwinbloodThresh;
        }

        return actionID;
    }
}

internal class AutoGenerationLegacies : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperAutoGenerationsLegaciesFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.Reawaken && HasEffect(VPR.Buffs.Reawakened))
        {
            var gauge = GetJobGauge<VPRGauge>();

            if (level >= VPR.Levels.Legacies)
            {
                if (OriginalHook(VPR.SerpentsTail) == VPR.FirstLegacy ||
                    OriginalHook(VPR.SerpentsTail) == VPR.SecondLegacy ||
                    OriginalHook(VPR.SerpentsTail) == VPR.ThirdLegacy ||
                    OriginalHook(VPR.SerpentsTail) == VPR.FourthLegacy)
                    return OriginalHook(VPR.SerpentsTail);
            }

            var maxtribute = 4;
            if (level >= VPR.Levels.Ouroboros)
                maxtribute = 5;
            if (gauge.AnguineTribute == maxtribute)
                return VPR.FirstGeneration;
            if (gauge.AnguineTribute == maxtribute - 1)
                return VPR.SecondGeneration;
            if (gauge.AnguineTribute == maxtribute - 2)
                return VPR.ThirdGeneration;
            if (gauge.AnguineTribute == maxtribute - 3)
                return VPR.FourthGeneration;
            if (gauge.AnguineTribute == 1 && level >= VPR.Levels.Ouroboros)
                return VPR.Ouroboros;
            }

        return actionID;
    }
}

internal class GenerationLegacies : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperGenerationLegaciesFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.SteelFangs)
        {
            if (OriginalHook(VPR.SerpentsTail) == VPR.FirstLegacy)
                return VPR.FirstLegacy;
        }

        if (actionID == VPR.ReavingFangs)
        {
            if (OriginalHook(VPR.SerpentsTail) == VPR.SecondLegacy)
                return VPR.SecondLegacy;
        }

        if (actionID == VPR.HuntersCoil)
        {
            if (OriginalHook(VPR.SerpentsTail) == VPR.ThirdLegacy)
                return VPR.ThirdLegacy;
        }

        if (actionID == VPR.SwiftskinsCoil)
        {
            if (OriginalHook(VPR.SerpentsTail) == VPR.FourthLegacy)
                return VPR.FourthLegacy;
        }

        return actionID;
    }
}

internal class GenerationLegaciesAoE : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperGenerationLegaciesAoEFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.SteelMaw)
        {
            if (OriginalHook(VPR.SerpentsTail) == VPR.FirstLegacy)
                return VPR.FirstLegacy;
        }

        if (actionID == VPR.ReavingMaw)
        {
            if (OriginalHook(VPR.SerpentsTail) == VPR.SecondLegacy)
                return VPR.SecondLegacy;
        }

        if (actionID == VPR.HuntersDen)
        {
            if (OriginalHook(VPR.SerpentsTail) == VPR.ThirdLegacy)
                return VPR.ThirdLegacy;
        }

        if (actionID == VPR.SwiftskinsDen)
        {
            if (OriginalHook(VPR.SerpentsTail) == VPR.FourthLegacy)
                return VPR.FourthLegacy;
        }

        return actionID;
    }
}

internal class UncoiledFollowupFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperUncoiledFollowupFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.UncoiledFury)
        {
            // If I'm reading this right, it will always want to go in this order
            if (OriginalHook(VPR.Twinfang) == VPR.UncoiledTwinfang && HasEffect(VPR.Buffs.PoisedForTwinfang))
                return VPR.UncoiledTwinfang;

            if (level >= VPR.Levels.UncoiledTwins && OriginalHook(VPR.Twinblood) == VPR.UncoiledTwinblood)
                return VPR.UncoiledTwinblood;
        }

        return actionID;
    }
}

internal class DreadfangsDreadwinderFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperDreadfangsDreadwinderFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.ReavingFangs)
        {
            // I think in this case if we're not in a combo (and something else isn't replacing Dread Fangs), we can just replace if we have charges
            if (level >= VPR.Levels.Dreadwinder && IsOriginal(VPR.ReavingFangs) && IsCooldownUsable(VPR.Dreadwinder) && IsOriginal(VPR.SerpentsTail)) // Add the check for Serpent's Tail to avoid stepping on other combo
                return VPR.Dreadwinder;
        }

        return actionID;
    }
}

internal class PitOfDreadFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperPitOfDreadFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.ReavingMaw)
        {
            if (level >= VPR.Levels.PitOfDread && IsOriginal(VPR.ReavingMaw) && IsCooldownUsable(VPR.PitOfDread) && IsOriginal(VPR.SerpentsTail)) // Add the check for Serpent's Tail to avoid stepping on other combo
                return VPR.PitOfDread;
        }

        return actionID;
    }
}

internal class MergeSerpentTwinsFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperMergeSerpentTwinsFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.SerpentsTail)
        {
            if (!IsOriginal(VPR.SerpentsTail))
                return OriginalHook(VPR.SerpentsTail);

            if (HasEffect(VPR.Buffs.PoisedForTwinfang) || HasEffect(VPR.Buffs.HuntersVenom) || HasEffect(VPR.Buffs.FellhuntersVenom))
                return OriginalHook(VPR.Twinfang);

            if (HasEffect(VPR.Buffs.PoisedForTwinblood) || HasEffect(VPR.Buffs.SwiftskinsVenom) || HasEffect(VPR.Buffs.FellskinsVenom))
                return OriginalHook(VPR.Twinblood);

            if (!IsOriginal(VPR.Twinfang))
                return OriginalHook(VPR.Twinfang);

            if (!IsOriginal(VPR.Twinblood))
                return OriginalHook(VPR.Twinblood);
        }

        return actionID;
    }
}

internal class MergeTwinsSerpentFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperMergeTwinsSerpentFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.Twinfang)
        {
            if (!IsOriginal(VPR.SerpentsTail))
                return OriginalHook(VPR.SerpentsTail);

            return OriginalHook(VPR.Twinfang);
        }

        if (actionID == VPR.Twinblood)
        {
            if (!IsOriginal(VPR.SerpentsTail))
                return OriginalHook(VPR.SerpentsTail);

            return OriginalHook(VPR.Twinblood);
        }

        return actionID;
    }
}

//internal class PvPMainComboFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperPvPMainComboFeature;

//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == VPR.SteelFangs)
//        {
//            if (HasEffect(VPR.Buffs.Reawakened))
//            {
//                var gauge = GetJobGauge<VPRGauge>();
//                var maxtribute = 4;
//                if (level >= VPR.Levels.Ouroboros)
//                    maxtribute = 5;
//                if (gauge.AnguineTribute == maxtribute)
//                    return VPR.FirstGeneration;
//                if (gauge.AnguineTribute == maxtribute - 1)
//                    return VPR.SecondGeneration;
//                if (gauge.AnguineTribute == maxtribute - 2)
//                    return VPR.ThirdGeneration;
//                if (gauge.AnguineTribute == maxtribute - 3)
//                    return VPR.FourthGeneration;
//            }

//            // First step, decide whether or not we need to apply debuff
//            if (OriginalHook(VPR.SteelFangs) == VPR.SteelFangs)
//            {
//                var noxious = FindTargetEffect(VPR.Debuffs.NoxiousGash);
//                if (level >= VPR.Levels.ReavingFangs && (noxious is null || noxious?.RemainingTime < 12)) // 12s hopefully means we won't miss anything on a Reawaken window
//                    return VPR.ReavingFangs;

//                return VPR.SteelFangs;
//            }

//            // Second step, if we have a third step buff use that combo, otherwise use from default combo
//            if (OriginalHook(VPR.SteelFangs) == VPR.HuntersSting)
//            {
//                if (HasEffect(VPR.Buffs.HindsbaneVenom) || HasEffect(VPR.Buffs.HindstungVenom))
//                    return VPR.SwiftskinsSting;
//                if (HasEffect(VPR.Buffs.FlanksbaneVenom) || HasEffect(VPR.Buffs.FlankstungVenom))
//                    return VPR.HuntersSting;

//                if (IsEnabled(CustomComboPreset.ViperPvPMainComboStartFlankstingFeature) || IsEnabled(CustomComboPreset.ViperPvPMainComboStartFlanksbaneFeature))
//                    return VPR.HuntersSting;

//                return VPR.SwiftskinsSting;
//            }

//            // Third step, if we are here, prefer to use what we have buffs for, otherwise use defaults
//            if (OriginalHook(VPR.SteelFangs) == VPR.FlankstingStrike || OriginalHook(VPR.SteelFangs) == VPR.HindstingStrike)
//            {
//                if (HasEffect(VPR.Buffs.HindsbaneVenom))
//                    return VPR.HindsbaneFang;
//                if (HasEffect(VPR.Buffs.HindstungVenom))
//                    return VPR.HindstingStrike;
//                if (HasEffect(VPR.Buffs.FlanksbaneVenom))
//                    return VPR.FlanksbaneFang;
//                if (HasEffect(VPR.Buffs.FlankstungVenom))
//                    return VPR.FlankstingStrike;

//                if (IsEnabled(CustomComboPreset.ViperPvPMainComboStartHindstingFeature))
//                    return VPR.HindstingStrike;
//                if (IsEnabled(CustomComboPreset.ViperPvPMainComboStartFlanksbaneFeature))
//                    return VPR.FlanksbaneFang;
//                if (IsEnabled(CustomComboPreset.ViperPvPMainComboStartFlankstingFeature))
//                    return VPR.FlankstingStrike;
//                return VPR.HindsbaneFang;
//            }
//        }

//        return actionID;
//    }
//}

//internal class PvPMainComboAoEFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperPvPMainComboAoEFeature;

//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == VPR.SteelMaw)
//        {
//            if (HasEffect(VPR.Buffs.Reawakened))
//            {
//                var gauge = GetJobGauge<VPRGauge>();
//                var maxtribute = 4;
//                if (level >= VPR.Levels.Ouroboros)
//                    maxtribute = 5;
//                if (gauge.AnguineTribute == maxtribute)
//                    return VPR.FirstGeneration;
//                if (gauge.AnguineTribute == maxtribute - 1)
//                    return VPR.SecondGeneration;
//                if (gauge.AnguineTribute == maxtribute - 2)
//                    return VPR.ThirdGeneration;
//                if (gauge.AnguineTribute == maxtribute - 3)
//                    return VPR.FourthGeneration;
//            }

//            // First step, decide whether or not we need to apply debuff
//            if (OriginalHook(VPR.SteelMaw) == VPR.SteelMaw)
//            {
//                var noxious = FindTargetEffect(VPR.Debuffs.NoxiousGash); // TODO: Would be useful to handle the case with no target
//                if (level >= VPR.Levels.ReavingMaw && (noxious is null || noxious?.RemainingTime < 12)) // 12s hopefully means we won't miss anything on a Reawaken window
//                    return VPR.ReavingMaw;

//                return VPR.SteelMaw;
//            }

//            // Second step, since there's no requirement here, we can just use whichever has the shorter buff timer
//            if (OriginalHook(VPR.SteelMaw) == VPR.HuntersBite)
//            {
//                var swift = FindEffect(VPR.Buffs.Swiftscaled);
//                var instinct = FindEffect(VPR.Buffs.HuntersInstinct);
//                if (swift is null) // I think we'd always want to prioritize swift since it speeds up the rotation
//                    return VPR.SwiftskinsBite;
//                if (instinct is null)
//                    return VPR.HuntersBite;
//                if (swift?.RemainingTime <= instinct?.RemainingTime)
//                    return VPR.SwiftskinsBite;

//                return VPR.HuntersBite;
//            }

//            if (OriginalHook(VPR.SteelMaw) == VPR.JaggedMaw)
//            {
//                if (HasEffect(VPR.Buffs.GrimhuntersVenom))
//                    return VPR.JaggedMaw;
//                if (HasEffect(VPR.Buffs.GrimskinsVenom))
//                    return VPR.BloodiedMaw;

//                if (IsEnabled(CustomComboPreset.ViperPvPMainComboAoEStartBloodiedFeature))
//                    return VPR.BloodiedMaw;

//                return VPR.JaggedMaw;
//            }
//        }

//        return actionID;
//    }
//}

internal class PvPWinderComboFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperPvPWinderComboFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.Dreadwinder)
        {
            var gauge = GetJobGauge<VPRGauge>();
            if (level >= VPR.Levels.Ouroboros && HasEffect(VPR.Buffs.Reawakened))
            {
                if (gauge.AnguineTribute == 1)
                    return VPR.Ouroboros;
            }

            if (IsEnabled(CustomComboPreset.ViperTwinCoilFeature))
            {
                if (HasEffect(VPR.Buffs.HuntersVenom))
                    return VPR.TwinfangBite;
                if (HasEffect(VPR.Buffs.SwiftskinsVenom))
                    return VPR.TwinbloodBite;
                if (OriginalHook(VPR.Twinfang) == VPR.TwinfangBite)
                    return VPR.TwinfangBite;
                if (OriginalHook(VPR.Twinblood) == VPR.TwinbloodBite)
                    return VPR.TwinbloodBite;
            }

            if (IsEnabled(CustomComboPreset.ViperPvPWinderComboStartHuntersFeature))
            {
                if (gauge.DreadCombo is DreadCombo.Dreadwinder and not DreadCombo.PitOfDread and not DreadCombo.HuntersDen and not DreadCombo.SwiftskinsDen)
                    return VPR.HuntersCoil;
                if (gauge.DreadCombo is DreadCombo.HuntersCoil and not DreadCombo.PitOfDread and not DreadCombo.HuntersDen and not DreadCombo.SwiftskinsDen)
                    return VPR.SwiftskinsCoil;
                if (gauge.DreadCombo is DreadCombo.SwiftskinsCoil and not DreadCombo.PitOfDread and not DreadCombo.HuntersDen and not DreadCombo.SwiftskinsDen)
                    return VPR.Dreadwinder;

                return VPR.Dreadwinder;
            }

            if (gauge.DreadCombo is DreadCombo.Dreadwinder and not DreadCombo.PitOfDread and not DreadCombo.HuntersDen and not DreadCombo.SwiftskinsDen)
                return VPR.SwiftskinsCoil;
            if (gauge.DreadCombo is DreadCombo.SwiftskinsCoil and not DreadCombo.PitOfDread and not DreadCombo.HuntersDen and not DreadCombo.SwiftskinsDen)
                return VPR.HuntersCoil;
            if (gauge.DreadCombo is DreadCombo.HuntersCoil and not DreadCombo.PitOfDread and not DreadCombo.HuntersDen and not DreadCombo.SwiftskinsDen)
                return VPR.Dreadwinder;

            return VPR.Dreadwinder;
        }

        return actionID;
    }
}

internal class PvPPitComboFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperPvPPitComboFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.PitOfDread)
        {
            var gauge = GetJobGauge<VPRGauge>();
            if (level >= VPR.Levels.Ouroboros && HasEffect(VPR.Buffs.Reawakened))
            {
                if (gauge.AnguineTribute == 1)
                    return VPR.Ouroboros;
            }

            if (IsEnabled(CustomComboPreset.ViperTwinCoilFeature))
            {
                if (HasEffect(VPR.Buffs.FellhuntersVenom))
                    return VPR.TwinfangThresh;
                if (HasEffect(VPR.Buffs.FellskinsVenom))
                    return VPR.TwinbloodThresh;
                if (OriginalHook(VPR.Twinfang) == VPR.TwinfangThresh)
                    return VPR.TwinfangThresh;
                if (OriginalHook(VPR.Twinblood) == VPR.TwinbloodThresh)
                    return VPR.TwinbloodThresh;
            }

            if (IsEnabled(CustomComboPreset.ViperPvPPitComboStartHuntersFeature))
            {
                if (gauge.DreadCombo is DreadCombo.PitOfDread and not DreadCombo.Dreadwinder and not DreadCombo.HuntersCoil and not DreadCombo.SwiftskinsCoil)
                    return VPR.HuntersDen;
                if (gauge.DreadCombo is DreadCombo.HuntersDen and not DreadCombo.Dreadwinder and not DreadCombo.HuntersCoil and not DreadCombo.SwiftskinsCoil)
                    return VPR.SwiftskinsDen;
                if (gauge.DreadCombo is DreadCombo.SwiftskinsDen and not DreadCombo.Dreadwinder and not DreadCombo.HuntersCoil and not DreadCombo.SwiftskinsCoil)
                    return VPR.PitOfDread;

                return VPR.PitOfDread;
            }

            if (gauge.DreadCombo is DreadCombo.PitOfDread and not DreadCombo.Dreadwinder and not DreadCombo.HuntersCoil and not DreadCombo.SwiftskinsCoil)
                return VPR.SwiftskinsDen;
            if (gauge.DreadCombo is DreadCombo.SwiftskinsDen and not DreadCombo.Dreadwinder and not DreadCombo.HuntersCoil and not DreadCombo.SwiftskinsCoil)
                return VPR.HuntersDen;
            if (gauge.DreadCombo is DreadCombo.HuntersDen and not DreadCombo.Dreadwinder and not DreadCombo.HuntersCoil and not DreadCombo.SwiftskinsCoil)
                return VPR.PitOfDread;

            return VPR.PitOfDread;
        }

        return actionID;
    }
}

// TODO: Once Gauge is implemented
internal class FuryAndIreFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ViperFuryAndIreFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.UncoiledFury && level >= VPR.Levels.UncoiledFury)
        {
            var gauge = GetJobGauge<VPRGauge>();
            if (gauge.RattlingCoilStacks == 0)
                return VPR.SerpentsIre;
        }

        return actionID;
    }
}