using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class PCT
{
    public const byte JobID = 42;

    public const uint
        FireRed = 34650,
        AeroGreen = 34651,
        WaterBlue = 34652,
        BlizzardCyan = 34653,
        EarthYellow = 34654,
        ThunderMagenta = 34655,
        ExtraFireRed = 34656,
        ExtraAeroGreen = 34657,
        ExtraWaterBlue = 34658,
        ExtraBlizzardCyan = 34659,
        ExtraEarthYellow = 34660,
        ExtraThunderMagenta = 34661,
        HolyInWhite = 34662,
        CometBlack = 34663,
        PomMotif = 34664,
        WingMotif = 34665,
        ClawMotif = 34666,
        MawMotif = 34667,
        HammerMotif = 34668,
        StarrySkyMotif = 34669,
        PomMuse = 34670,
        WingedMuse = 34671,
        ClawedMuse = 34672,
        FangedMuse = 34673,
        StrikingMuse = 34674,
        StarryMuse = 34675,
        MogOftheAges = 34676,
        Retribution = 34677,
        HammerStamp = 34678,
        HammerBrush = 34679,
        PolishingHammer = 34680,
        StarPrism1 = 34681,
        StarPrism2 = 34682,
        SubstractivePalette = 34683,
        Smudge = 34684,
        TemperaCoat = 34685,
        TemperaGrassa = 34686,
        RainbowDrip = 34688,
        CreatureMotif = 34689,
        WeaponMotif = 34690,
        LandscapeMotif = 34691,
        CreatureMotifDrawn = 35347,
        WeaponMotifDrawn = 35348,
        LandscapeMotifDrawn = 35349;

    public static class Buffs
    {
        public const ushort
            SubstractivePalette = 3674,
            Chroma2Ready = 3675,
            Chroma3Ready = 3676,
            RainbowReady = 3679,
            HammerReady = 3680,
            StarPrismReady = 3681,
            Installation = 3688,
            ArtisticInstallation = 3689,
            SubstractivePaletteReady = 3690,
            InvertedColors = 3691;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            FireRed = 1,
            AeroGreen = 5,
            TemperaCoat = 10,
            WaterBlue = 15,
            Smudge = 20,
            ExtraFireRed = 25,
            CreatureMotif = 30,
            PomMotif = 30,
            WingMotif = 30,
            PomMuse = 30,
            WingedMuse = 30,
            MogOftheAges = 30,
            ExtraAeroGreen = 35,
            ExtraWaterBlue = 45,
            HammerMotif = 50,
            HammerStamp = 50,
            WeaponMotif = 50,
            StrikingMuse = 50,
            SubstractivePalette = 60,
            BlizzardCyan = 60,
            EarthYellow = 60,
            ThunderMagenta = 60,
            ExtraBlizzardCyan = 60,
            ExtraEarthYellow = 60,
            ExtraThunderMagenta = 60,
            StarrySkyMotif = 70,
            LandscapeMotif = 70,
            MiracleWhite = 80,
            HammerBrush = 86,
            PolishingHammer = 86,
            TemperaGrassa = 88,
            CometBlack = 90,
            RainbowDrip = 92,
            ClawMotif = 96,
            MawMotif = 96,
            ClawedMuse = 96,
            FangedMuse = 96,
            StarryMuse = 70,
            Retribution = 96,
            StarPrism1 = 100,
            StarPrism2 = 100;
    }

    internal class PictomancerSTCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PctAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<PCTGauge>();

            if ((actionID == PCT.FireRed || actionID == PCT.BlizzardCyan) && IsEnabled(CustomComboPreset.PictomancerRainbowStarter) && !InCombat() && level >= PCT.Levels.RainbowDrip)
            {
                return PCT.RainbowDrip;
            }

            if (actionID == PCT.FireRed || actionID == PCT.BlizzardCyan)
            {
                if (IsEnabled(CustomComboPreset.PictomancerStarPrismAutoCombo))
                {
                    if (HasEffect(PCT.Buffs.StarPrismReady))
                    {
                        return PCT.StarPrism1;
                    }
                }

                if (IsEnabled(CustomComboPreset.PictomancerRainbowAutoCombo))
                {
                    if (HasEffect(PCT.Buffs.RainbowReady))
                    {
                        return PCT.RainbowDrip;
                    }
                }

                if (IsEnabled(CustomComboPreset.PictomancerAutoMogCombo))
                {
                    if ((gauge.MooglePortraitReady || gauge.MadeenPortraitReady) && GetRemainingCharges(PCT.MogOftheAges) > 0)
                    {
                        return OriginalHook(PCT.MogOftheAges);
                    }
                }

                if (IsEnabled(CustomComboPreset.PictomancerSubtractiveAutoCombo) && !HasEffect(PCT.Buffs.SubstractivePalette))
                {
                    if (IsEnabled(CustomComboPreset.PictomancerSubtractiveEarlyAutoCombo)
                        && (gauge.PalleteGauge >= 50 || HasEffect(PCT.Buffs.SubstractivePaletteReady)))
                        return PCT.SubstractivePalette;

                    if (HasEffect(PCT.Buffs.SubstractivePaletteReady) || (HasEffect(PCT.Buffs.Chroma3Ready) && (gauge.PalleteGauge == 100)))
                        return PCT.SubstractivePalette;
                }

                if (IsEnabled(CustomComboPreset.PictomancerHolyAutoCombo))
                {
                    if (gauge.Paint == 5)
                    {
                        if (HasEffect(PCT.Buffs.InvertedColors))
                            return PCT.CometBlack;
                        return PCT.HolyInWhite;
                    }
                }

                if (IsEnabled(CustomComboPreset.PictomancerSubtractiveSTCombo))
                {
                    if (!HasEffect(PCT.Buffs.SubstractivePalette))
                    {
                        if (HasEffect(PCT.Buffs.Chroma2Ready))
                        {
                            return PCT.AeroGreen;
                        }
                        else if (HasEffect(PCT.Buffs.Chroma3Ready))
                        {
                            return PCT.WaterBlue;
                        }

                        return PCT.FireRed;
                    }
                }
            }

            return actionID;
        }
    }

    internal class PictomancerAoECombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PctAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<PCTGauge>();

            if ((actionID == PCT.ExtraFireRed || actionID == PCT.ExtraBlizzardCyan) && IsEnabled(CustomComboPreset.PictomancerRainbowStarter) && !InCombat())
            {
                return PCT.RainbowDrip;
            }

            if (actionID == PCT.ExtraBlizzardCyan)
            {
                if (IsEnabled(CustomComboPreset.PictomancerSubtractiveAutoCombo) && !HasEffect(PCT.Buffs.SubstractivePalette))
                {
                    if (IsEnabled(CustomComboPreset.PictomancerSubtractiveEarlyAutoCombo)
                        && (gauge.PalleteGauge >= 50 || HasEffect(PCT.Buffs.SubstractivePaletteReady)))
                        return PCT.SubstractivePalette;

                    if (HasEffect(PCT.Buffs.Chroma3Ready) && (gauge.PalleteGauge == 100))
                        return PCT.SubstractivePalette;
                }

                if (IsEnabled(CustomComboPreset.PictomancerSubtractiveAoECombo))
                {
                    if (!HasEffect(PCT.Buffs.SubstractivePalette))
                    {
                        if (actionID == PCT.ExtraBlizzardCyan)
                        {
                            if (HasEffect(PCT.Buffs.Chroma2Ready) && level >= PCT.Levels.ExtraAeroGreen)
                            {
                                return PCT.ExtraAeroGreen;
                            }
                            else if (HasEffect(PCT.Buffs.Chroma3Ready) && level >= PCT.Levels.ExtraWaterBlue)
                            {
                                return PCT.ExtraWaterBlue;
                            }

                            return OriginalHook(PCT.ExtraFireRed);
                        }
                    }
                }
            }

            return actionID;
        }
    }

    internal class PictomancerSubtractiveAutoCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerSubtractiveAutoCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<PCTGauge>();
            if (actionID == PCT.WaterBlue || actionID == PCT.ExtraWaterBlue)
            {
                if (HasEffect(PCT.Buffs.Chroma3Ready) && !HasEffect(PCT.Buffs.SubstractivePalette) && gauge.PalleteGauge == 100)
                {
                    return PCT.SubstractivePalette;
                }
            }

            return actionID;
        }
    }

    internal class PictomancerHolyCometCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerHolyCometCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PCT.HolyInWhite)
            {
                if (IsEnabled(CustomComboPreset.PictomancerRainbowHolyCombo) && HasEffect(PCT.Buffs.RainbowReady))
                {
                    return PCT.RainbowDrip;
                }

                if (HasEffect(PCT.Buffs.InvertedColors))
                    return PCT.CometBlack;
            }

            return actionID;
        }
    }

    internal class PictomancerCreatureMotifCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PctAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<PCTGauge>();

            if (actionID == PCT.CreatureMotif)
            {
                if (IsEnabled(CustomComboPreset.PictomancerCreatureMogCombo))
                {
                    if (gauge.MooglePortraitReady || gauge.MadeenPortraitReady)
                    {
                        if (IsCooldownUsable(PCT.MogOftheAges))
                            return OriginalHook(PCT.MogOftheAges);
                    }
                }

                if (IsEnabled(CustomComboPreset.PictomancerCreatureMotifCombo))
                {
                    if (actionID == PCT.CreatureMotif)
                    {
                        if (OriginalHook(PCT.CreatureMotifDrawn) != PCT.CreatureMotifDrawn)
                            return OriginalHook(PCT.CreatureMotifDrawn);
                    }
                }
            }

            return actionID;
        }
    }

    internal class PictomancerWeaponMotifCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PctAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<PCTGauge>();

            if (actionID == PCT.WeaponMotif)
            {
                if (IsEnabled(CustomComboPreset.PictomancerWeaponMotifCombo))
                {
                    if (gauge.WeaponMotifDrawn)
                        return PCT.StrikingMuse;
                }

                if (IsEnabled(CustomComboPreset.PictomancerWeaponHammerCombo))
                {
                    if (HasEffect(PCT.Buffs.HammerReady))
                    {
                        return OriginalHook(PCT.HammerStamp);
                    }
                }
            }

            return actionID;
        }
    }

    internal class PictomancerLandscapeMotifCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PctAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<PCTGauge>();

            if (actionID == PCT.LandscapeMotif)
            {
                if (IsEnabled(CustomComboPreset.PictomancerLandscapeMotifCombo))
                {
                    if (gauge.LandscapeMotifDrawn)
                        return PCT.StarryMuse;
                }

                if (IsEnabled(CustomComboPreset.PictomancerLandscapePrismCombo))
                {
                    if (HasEffect(PCT.Buffs.StarPrismReady))
                    {
                        return OriginalHook(PCT.StarPrism1);
                    }
                }
            }

            return actionID;
        }
    }
}
