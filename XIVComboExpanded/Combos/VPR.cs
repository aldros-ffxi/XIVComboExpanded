using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

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

            Vicewinder = 34620,
            HuntersCoil = 34621,
            SwiftskinsCoil = 34622,
            VicePit = 34623,
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
            Vicewinder = 65,     // Also includes Hunter's Coil and Swiftskin's Coil
            VicePit = 70,        // Also includes Hunter's Den and Swiftskin's Den
            TwinsSingle = 75,    // Twinfang Bite and Twinblood Bite
            TwinsAoE = 80,       // Twinfang Thresh and Twinblood Thresh
            UncoiledFury = 82,
            SerpentsIre = 86,
            EnhancedRattle = 88, // Third stack of Rattling Coil can be accumulated
            Reawaken = 90,       // Also includes First Generation through Fourth Generation
            UncoiledTwins = 92,  // Uncoiled Twinfang and Uncoiled Twinblood
            Ouroboros = 96,      // Also includes a 5th Anguine Tribute stack from Reawaken
            Legacies = 100;      // First through Fourth Legacy
    }
}

internal class ViperFangs : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.SteelFangs || actionID == VPR.ReavingFangs)
        {
            var gauge = GetJobGauge<VPRGauge>();
            var maxtribute = level >= VPR.Levels.Ouroboros ? 5 : 4;

            if (IsEnabled(CustomComboPreset.ViperSteelTailFeature) && OriginalHook(VPR.SerpentsTail) == VPR.DeathRattle)
                return VPR.DeathRattle;

            if (IsEnabled(CustomComboPreset.ViperGenerationLegaciesFeature))
            {
                if (actionID == VPR.SteelFangs && OriginalHook(VPR.SerpentsTail) == VPR.FirstLegacy)
                    return VPR.FirstLegacy;

                if (actionID == VPR.ReavingFangs && OriginalHook(VPR.SerpentsTail) == VPR.SecondLegacy)
                    return VPR.SecondLegacy;
            }

            if (IsEnabled(CustomComboPreset.ViperSteelCoilFeature))
            {
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

                if (actionID == VPR.SteelFangs)
                {
                    if (gauge.AnguineTribute == maxtribute)
                        return VPR.FirstGeneration;

                    if (gauge.AnguineTribute == maxtribute - 2)
                        return VPR.ThirdGeneration;

                    if (CanUseAction(VPR.SwiftskinsCoil) && gauge.AnguineTribute == 0)
                        return VPR.SwiftskinsCoil;
                }

                if (actionID == VPR.ReavingFangs)
                {
                    if (gauge.AnguineTribute >= 3 || (level < VPR.Levels.Ouroboros && gauge.AnguineTribute >= 2))
                        return VPR.SecondGeneration;

                    if (gauge.AnguineTribute > 0)
                        return VPR.FourthGeneration;

                    if (CanUseAction(VPR.HuntersCoil) && gauge.AnguineTribute == 0)
                        return VPR.HuntersCoil;
                }
            }

            if (IsEnabled(CustomComboPreset.ViperAutoViceSTFeature) &&
                level >= VPR.Levels.Vicewinder && IsOriginal(VPR.ReavingFangs) && 
                IsCooldownUsable(VPR.Vicewinder) && IsOriginal(VPR.SerpentsTail))
                return VPR.Vicewinder;

            if (IsEnabled(CustomComboPreset.ViperAutoSteelReavingFeature) && OriginalHook(VPR.SteelFangs) == VPR.SteelFangs)
                return HasEffect(VPR.Buffs.HonedReavers) ? VPR.ReavingFangs : VPR.SteelFangs;
        }

        return actionID;
    }
}

internal class ViperMaws : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.SteelMaw || actionID == VPR.ReavingMaw)
        {
            var gauge = GetJobGauge<VPRGauge>();
            var maxtribute = level >= VPR.Levels.Ouroboros ? 5 : 4;

            if (IsEnabled(CustomComboPreset.ViperSteelTailFeature) && OriginalHook(VPR.SerpentsTail) == VPR.LastLash)
                return VPR.LastLash;

            if (IsEnabled(CustomComboPreset.ViperGenerationLegaciesFeature))
            {
                if (actionID == VPR.SteelMaw && OriginalHook(VPR.SerpentsTail) == VPR.FirstLegacy)
                    return VPR.FirstLegacy;

                if (actionID == VPR.ReavingMaw && OriginalHook(VPR.SerpentsTail) == VPR.SecondLegacy)
                    return VPR.SecondLegacy;
            }

            if (IsEnabled(CustomComboPreset.ViperSteelCoilFeature))
            {
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

                if (actionID == VPR.SteelMaw)
                {
                    if (gauge.AnguineTribute == maxtribute)
                        return VPR.FirstGeneration;

                    if (gauge.AnguineTribute == maxtribute - 2)
                        return VPR.ThirdGeneration;

                    if (CanUseAction(VPR.SwiftskinsDen) && gauge.AnguineTribute == 0)
                        return VPR.SwiftskinsDen;
                }

                if (actionID == VPR.ReavingMaw)
                {
                    if (gauge.AnguineTribute >= 3 || (level < VPR.Levels.Ouroboros && gauge.AnguineTribute >= 2))
                        return VPR.SecondGeneration;

                    if (gauge.AnguineTribute > 0)
                        return VPR.FourthGeneration;

                    if (CanUseAction(VPR.HuntersDen) && gauge.AnguineTribute == 0)
                        return VPR.HuntersDen;
                }
            }

            if (IsEnabled(CustomComboPreset.ViperAutoViceAoEFeature) &&
                level >= VPR.Levels.VicePit && IsOriginal(VPR.ReavingMaw) && 
                IsCooldownUsable(VPR.VicePit) && IsOriginal(VPR.SerpentsTail))
                return VPR.VicePit;

            if (IsEnabled(CustomComboPreset.ViperAutoSteelReavingFeature) && OriginalHook(VPR.SteelMaw) == VPR.SteelMaw)
                return HasEffect(VPR.Buffs.HonedReavers) ? VPR.ReavingMaw : VPR.SteelMaw;
        }

        return actionID;
    }
}

internal class ViperCoils : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.HuntersCoil || actionID == VPR.SwiftskinsCoil)
        {
            if (IsEnabled(CustomComboPreset.ViperGenerationLegaciesFeature))
            {
                if (actionID == VPR.HuntersCoil && OriginalHook(VPR.SerpentsTail) == VPR.ThirdLegacy)
                    return VPR.ThirdLegacy;

                if (actionID == VPR.SwiftskinsCoil && OriginalHook(VPR.SerpentsTail) == VPR.FourthLegacy)
                    return VPR.FourthLegacy;
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
        }

        return actionID;
    }
}

internal class ViperDens : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.HuntersDen || actionID == VPR.SwiftskinsDen)
        {
            if (IsEnabled(CustomComboPreset.ViperGenerationLegaciesFeature))
            {
                if (actionID == VPR.HuntersDen && OriginalHook(VPR.SerpentsTail) == VPR.ThirdLegacy)
                    return VPR.ThirdLegacy;

                if (actionID == VPR.SwiftskinsDen && OriginalHook(VPR.SerpentsTail) == VPR.FourthLegacy)
                    return VPR.FourthLegacy;
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
        }

        return actionID;
    }
}

internal class ViperUncoiled : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.UncoiledFury)
        {
            if (IsEnabled(CustomComboPreset.ViperUncoiledFollowupFeature))
            {
                if (OriginalHook(VPR.Twinfang) == VPR.UncoiledTwinfang && HasEffect(VPR.Buffs.PoisedForTwinfang))
                    return VPR.UncoiledTwinfang;

                if (OriginalHook(VPR.Twinblood) == VPR.UncoiledTwinblood)
                    return VPR.UncoiledTwinblood;
            }

            if (IsEnabled(CustomComboPreset.ViperFuryAndIreFeature) && level >= VPR.Levels.SerpentsIre) 
            {
                var gauge = GetJobGauge<VPRGauge>();
                if (gauge.RattlingCoilStacks == 0)
                    return VPR.SerpentsIre;
            }
        }

        return actionID;
    }
}

internal class ViperVicewinder : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.Vicewinder)
        {
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

            if (IsEnabled(CustomComboPreset.ViperPvPWinderComboFeature))
            {
                var gauge = GetJobGauge<VPRGauge>();
                if (level >= VPR.Levels.Ouroboros && HasEffect(VPR.Buffs.Reawakened) && gauge.AnguineTribute == 1)
                        return VPR.Ouroboros;

                if (IsEnabled(CustomComboPreset.ViperPvPWinderComboStartHuntersFeature) && CanUseAction(VPR.HuntersCoil))
                    return VPR.HuntersCoil;

                if (CanUseAction(VPR.SwiftskinsCoil))
                    return VPR.SwiftskinsCoil;

                if (CanUseAction(VPR.HuntersCoil))
                    return VPR.HuntersCoil;

                return VPR.Vicewinder;
            }
        }

        return actionID;
    }
}

internal class ViperVicepit : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.VicePit)
        {
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

            if (IsEnabled(CustomComboPreset.ViperPvPPitComboFeature))
            {
                var gauge = GetJobGauge<VPRGauge>();
                if (level >= VPR.Levels.Ouroboros && HasEffect(VPR.Buffs.Reawakened) && gauge.AnguineTribute == 1)
                        return VPR.Ouroboros;

                if (IsEnabled(CustomComboPreset.ViperPvPPitComboStartHuntersFeature) && CanUseAction(VPR.HuntersDen))
                    return VPR.HuntersDen;

                if (CanUseAction(VPR.SwiftskinsDen))
                    return VPR.SwiftskinsDen;

                if (CanUseAction(VPR.HuntersDen))
                    return VPR.HuntersDen;

                return VPR.VicePit;
            }
        }

        return actionID;
    }
}

internal class ViperReawaken : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.Reawaken)
        {
            if (IsEnabled(CustomComboPreset.ViperReawakenAIOFeature) && HasEffect(VPR.Buffs.Reawakened))
            {
                var gauge = GetJobGauge<VPRGauge>();

                if (level >= VPR.Levels.Legacies)
                {   
                    var original = OriginalHook(VPR.SerpentsTail);
                    if (original == VPR.FirstLegacy || 
                        original == VPR.SecondLegacy ||
                        original == VPR.ThirdLegacy ||
                        original == VPR.FourthLegacy)
                        return original;
                }

                var maxtribute = level >= VPR.Levels.Ouroboros ? 5 : 4;
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
        }

        return actionID;
    }
}

internal class ViperoGCDs : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.SerpentsTail)
        {
            if (IsEnabled(CustomComboPreset.ViperMergeSerpentTwinsFeature))
            {
                if (!IsOriginal(VPR.SerpentsTail))
                    return OriginalHook(VPR.SerpentsTail);

                if (HasEffect(VPR.Buffs.PoisedForTwinfang) || 
                    HasEffect(VPR.Buffs.HuntersVenom) || 
                    HasEffect(VPR.Buffs.FellhuntersVenom))
                    return OriginalHook(VPR.Twinfang);

                if (HasEffect(VPR.Buffs.PoisedForTwinblood) || 
                    HasEffect(VPR.Buffs.SwiftskinsVenom) || 
                    HasEffect(VPR.Buffs.FellskinsVenom))
                    return OriginalHook(VPR.Twinblood);

                if (!IsOriginal(VPR.Twinfang))
                    return OriginalHook(VPR.Twinfang);

                if (!IsOriginal(VPR.Twinblood))
                    return OriginalHook(VPR.Twinblood);
            }
        }

        if (actionID == VPR.Twinfang || actionID == VPR.Twinblood)
        {
            if (IsEnabled(CustomComboPreset.ViperMergeTwinsSerpentFeature) && !IsOriginal(VPR.SerpentsTail))
                return OriginalHook(VPR.SerpentsTail);
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

