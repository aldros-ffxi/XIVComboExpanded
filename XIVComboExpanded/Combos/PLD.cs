namespace XIVComboExpandedPlugin.Combos;

internal static class PLD
{
    public const byte ClassID = 1;
    public const byte JobID = 19;

    public const uint
        FastBlade = 9,
        RiotBlade = 15,
        ShieldBash = 16,
        FightOrFlight = 20,
        RageOfHalone = 21,
        CircleOfScorn = 23,
        ShieldLob = 24,
        IronWill = 28,
        SpiritsWithin = 29,
        GoringBlade = 3538,
        RoyalAuthority = 3539,
        Clemency = 3541,
        TotalEclipse = 7381,
        Requiescat = 7383,
        HolySpirit = 7384,
        LowBlow = 7540,
        Prominence = 16457,
        HolyCircle = 16458,
        Confiteor = 16459,
        Atonement = 16460,
        Expiacion = 25747,
        BladeOfFaith = 25748,
        BladeOfTruth = 25749,
        BladeOfValor = 25750,
        IronWillRemoval = 32065,
        Supplication = 36918,
        Sepulchre = 36919,
        Imperator = 36921,
        BladeOfHonor = 36922;

    public static class Buffs
    {
        public const ushort
            FightOrFlight = 76,
            IronWill = 79,
            Requiescat = 1368,
            DivineMight = 2673,
            GoringBladeReady = 3847,
            AtonementReady = 1902,
            SupplicationReady = 3827,
            SepulchreReady = 3828,
            ConfiteorReady = 3019,
            BladeOfHonorReady = 3831;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            FightOrFlight = 2,
            RiotBlade = 4,
            IronWill = 10,
            LowBlow = 12,
            SpiritsWithin = 30,
            CircleOfScorn = 50,
            RageOfHalone = 26,
            Prominence = 40,
            GoringBlade = 54,
            RoyalAuthority = 60,
            HolySpirit = 64,
            DivineMagicMastery = 64,
            Requiescat = 68,
            HolyCircle = 72,
            Atonement = 76,
            Supplication = 76,
            Sepulchre = 76,
            Confiteor = 80,
            Expiacion = 86,
            BladeOfFaith = 90,
            BladeOfHonor = 100;
    }
}

internal abstract class PaladinCombo : CustomCombo
{
    protected bool HasMp(uint spell)
    {
        int cost;
        switch (spell)
        {
            case PLD.Clemency:
                cost = 2000;
                break;
            case PLD.HolySpirit:
            case PLD.HolyCircle:
            case PLD.Confiteor:
            case PLD.BladeOfFaith:
            case PLD.BladeOfTruth:
            case PLD.BladeOfValor:
                cost = 1000;
                break;
            default:
                cost = 0;
                break;
        }

        if (LocalPlayer?.Level >= PLD.Levels.DivineMagicMastery)
            cost /= 2;

        return LocalPlayer?.CurrentMp >= cost;
    }
}

internal class PaladinRoyalAuthority : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.RageOfHalone || actionID == PLD.RoyalAuthority)
        {
            var inMeleeRange = InMeleeRange(); // Only calculate this once, to save some CPU cycles

            if (IsEnabled(CustomComboPreset.PaladinRoyalAuthorityGoringBladeComboFeature) && HasEffect(PLD.Buffs.GoringBladeReady))
                return PLD.GoringBlade;

            if (level >= PLD.Levels.Confiteor && IsEnabled(CustomComboPreset.PaladinRoyalAuthorityConfiteorComboFeature))
            {
                var original = OriginalHook(PLD.Confiteor);
                if (original != PLD.Confiteor)
                    return original;

                if (HasEffect(PLD.Buffs.BladeOfHonorReady))
                    return OriginalHook(PLD.Imperator);

                if (HasEffect(PLD.Buffs.ConfiteorReady))
                    return OriginalHook(PLD.Confiteor);
            }

            // During FoF, prioritize the higher-potency Divine Might cast over Atonement and the normal combo chain
            if (IsEnabled(CustomComboPreset.PaladinRoyalAuthorityFightOrFlightFeature))
            {
                var fof = FindEffect(PLD.Buffs.FightOrFlight);
                if (fof != null)
                {
                    // Inside FoF, the Atonement combo has priority over Holy Spirit due to potency, but only if we can fit in Sepulchre (480p), vs Holy Spirit 470p.
                    // Basically, for the extra 3 GCDs in FoF after the Blades combo, using Atonement -> Supplication -> Sepulchre is 10 more potency than using
                    // Holy Spirit -> Atonement -> Supplication.  If we can't fit in Sepulchre, however, Holy Spirit is better.
                    // Note: Technically, this is only true after level 94's Melee Mastery II trait, as prior to that, Holy Spirit with Divine Might has 10 more
                    // potency than Sepulchre, rather than the other way around.  However, since optimizing gains or losses of only 10p doesn't really matter at all
                    // while leveling, we don't bother to adjust this logic for lower levels, except where Atonement isn't yet learned.
                    if (level >= PLD.Levels.Atonement && IsEnabled(CustomComboPreset.PaladinRoyalAuthorityAtonementComboFeature))
                    {
                        // These use a fixed 2.5s for the GCD, because I don't know how to pull the actual GCD length.
                        // Technically, this can fail to use the correct ability if the GCD is less than 2.5s and the
                        // remaining duration of FoF is just barely enough, but since latency is a thing,
                        // it's probably good to have this type of buffer anyway.
                        if (HasEffect(PLD.Buffs.SepulchreReady) && inMeleeRange)
                            return OriginalHook(PLD.Atonement);

                        if (HasEffect(PLD.Buffs.SupplicationReady) && inMeleeRange && (fof.RemainingTime > 2.5 || !HasEffect(PLD.Buffs.DivineMight)))
                            return OriginalHook(PLD.Atonement);

                        if (HasEffect(PLD.Buffs.AtonementReady) && inMeleeRange && (fof.RemainingTime > 5 || !HasEffect(PLD.Buffs.DivineMight)))
                            return OriginalHook(PLD.Atonement);
                    }

                    if (HasEffect(PLD.Buffs.DivineMight) && this.HasMp(PLD.HolySpirit))
                        return PLD.HolySpirit;
                }

                if (HasEffect(PLD.Buffs.Requiescat) && this.HasMp(PLD.HolySpirit))
                    return PLD.HolySpirit;
            }

            if (level >= PLD.Levels.Atonement && IsEnabled(CustomComboPreset.PaladinRoyalAuthorityAtonementComboFeature))
            {
                var sepulchre = FindEffect(PLD.Buffs.SepulchreReady);
                if (sepulchre != null && inMeleeRange && (lastComboMove == PLD.RiotBlade || sepulchre.RemainingTime <= 2.5))
                    return OriginalHook(PLD.Atonement);

                var supplication = FindEffect(PLD.Buffs.SupplicationReady);
                if (supplication != null && inMeleeRange && (lastComboMove == PLD.RiotBlade || supplication.RemainingTime <= 5))
                    return OriginalHook(PLD.Atonement);

                var atonementReady = FindEffect(PLD.Buffs.AtonementReady);
                if (atonementReady != null && inMeleeRange && (lastComboMove == PLD.RiotBlade || atonementReady.RemainingTime <= 7.5))
                    return OriginalHook(PLD.Atonement);
            }

            if (level >= PLD.Levels.HolySpirit && IsEnabled(CustomComboPreset.PaladinRoyalAuthorityDivineMightFeature))
            {
                var divineMight = FindEffect(PLD.Buffs.DivineMight);
                if (this.HasMp(PLD.HolySpirit) && divineMight != null && (lastComboMove == PLD.RiotBlade || divineMight.RemainingTime <= 2.5 || !inMeleeRange))
                    return PLD.HolySpirit;
            }

            if (IsEnabled(CustomComboPreset.PaladinRoyalAuthorityCombo))
            {
                // Royal Authority
                if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.RageOfHalone)
                    return OriginalHook(PLD.RageOfHalone);

                if (lastComboMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                    return PLD.RiotBlade;
                return PLD.FastBlade;
            }

            if (IsEnabled(CustomComboPreset.PaladinAtonementCombo) && level >= PLD.Levels.Atonement)
            {
                var sepulchre = FindEffect(PLD.Buffs.SepulchreReady);
                if (sepulchre != null && inMeleeRange)
                    return OriginalHook(PLD.Atonement);

                var supplication = FindEffect(PLD.Buffs.SupplicationReady);
                if (supplication != null && inMeleeRange)
                    return OriginalHook(PLD.Atonement);

                var atonementReady = FindEffect(PLD.Buffs.AtonementReady);
                if (atonementReady != null && inMeleeRange)
                    return OriginalHook(PLD.Atonement);
            }
        }

        return actionID;
    }
}

internal class PaladinProminence : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinProminenceCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.Prominence)
        {
            if (IsEnabled(CustomComboPreset.PaladinProminenceGoringBladeComboFeature) && HasEffect(PLD.Buffs.GoringBladeReady))
                return PLD.GoringBlade;

            if (level >= PLD.Levels.Confiteor && IsEnabled(CustomComboPreset.PaladinProminenceConfiteorComboFeature))
            {
                var original = OriginalHook(PLD.Confiteor);
                if (original != PLD.Confiteor)
                    return original;

                if (HasEffect(PLD.Buffs.BladeOfHonorReady))
                    return OriginalHook(PLD.Imperator);

                if (HasEffect(PLD.Buffs.ConfiteorReady))
                    return OriginalHook(PLD.Confiteor);
            }

            // During FoF, prioritize the higher-potency Divine Might cast over the normal combo chain
            if (IsEnabled(CustomComboPreset.PaladinProminenceDivineMightFeature))
            {
                if (level >= PLD.Levels.HolyCircle && this.HasMp(PLD.HolyCircle))
                {
                    if (HasEffect(PLD.Buffs.FightOrFlight) && (HasEffect(PLD.Buffs.DivineMight) || HasEffect(PLD.Buffs.Requiescat)))
                        return PLD.HolyCircle;
                }
            }

            if (lastComboMove == PLD.TotalEclipse && level >= PLD.Levels.Prominence)
            {
                if (IsEnabled(CustomComboPreset.PaladinProminenceDivineMightFeature))
                {
                    if (level >= PLD.Levels.HolyCircle && HasEffect(PLD.Buffs.DivineMight) && this.HasMp(PLD.HolyCircle))
                        return PLD.HolyCircle;
                }

                return PLD.Prominence;
            }

            return PLD.TotalEclipse;
        }

        return actionID;
    }
}

internal class PaladinHolySpiritHolyCircle : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinConfiteorFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.HolySpirit || actionID == PLD.HolyCircle)
        {
            if (level >= PLD.Levels.Confiteor)
            {
                var original = OriginalHook(PLD.Confiteor);
                if (original != PLD.Confiteor)
                    return original;

                if (HasEffect(PLD.Buffs.ConfiteorReady))
                    return PLD.Confiteor;
            }
        }

        return actionID;
    }
}

internal class PaladinHolySpirit : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinHolySpiritLevelSyncFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.HolySpirit && level < PLD.Levels.HolySpirit)
          return PLD.ShieldLob;
        return actionID;
    }
}

internal class PaladinRequiescat : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.Requiescat || actionID == PLD.Imperator)
        {
            // Prioritize Goring Blade over the Confiteor combo.  While Goring Blade deals less damage (700p) than
            // most of the Confiteor combo (900p -> 700p -> 800p -> 900p), Goring Blade uniquely requires melee
            // range to cast, while the entire Confiteor combo chain does not.  Since Requiescat also requires
            // melee range to cast, the most reliable time that the player will be in melee range during the Req
            // buff is immediately following the usage of Req.  This minimizes potential losses and potential
            // cooldown drift if the player is forced out of melee range during the Confiteor combo and is unable
            // to return to melee range by the time it is completed.
            //
            // Since Goring Blade, the entire Confiteor combo, *and* one additional GCD (typically Holy Spirit) fits
            // within even the shortest of party buffs (15s ones like Battle Litany), this should not result in a
            // net reduction in potency, and *may* in fact increase it if someone is slightly late in applying
            // their party buffs, as it shifts the high-potency Confiteor cast back into the party buff window by a
            // single GCD.
            if (IsEnabled(CustomComboPreset.PaladinRequiescatFightOrFlightFeature))
            {
                // Don't use Goring Blade until Requiescat is on cooldown, unless it somehow got desynced and is >5s left on its cooldown.
                // This prevents an odd effect where shortly after casting Fight or Flight, Goring Blade would be cast instead of Requiescat, causing drift.
                // This also respects the user's selection in the Actions and Trait window for whether FoF turns into Goring Blade, only including it on
                // Requiescat if FoF would also include it.
                // Lastly, if the user is not currently in melee range, Goring Blade will be skipped, usually in favor of the Confiteor combo below.
                if (OriginalHook(PLD.FightOrFlight) == PLD.GoringBlade && GetCooldown(PLD.Requiescat).CooldownRemaining > 5 && InMeleeRange())
                    return PLD.GoringBlade;
            }

            if (IsEnabled(CustomComboPreset.PaladinRequiescatCombo))
            {
                if (level >= PLD.Levels.Confiteor)
                {
                    // Blade combo
                    var original = OriginalHook(PLD.Confiteor);
                    if (original != PLD.Confiteor)
                        return original;

                    if (HasEffect(PLD.Buffs.BladeOfHonorReady))
                        return OriginalHook(PLD.Imperator);

                    if (HasEffect(PLD.Buffs.ConfiteorReady))
                        return OriginalHook(PLD.Confiteor);
                }

                // This should only occur if the user is below the level for the full 4-part Confiteor combo (level 90), as after that level, all 4
                // stacks of Requiescat will be consumed by the Confiteor combo.
                if (level >= PLD.Levels.Requiescat && HasEffect(PLD.Buffs.Requiescat))
                    return PLD.HolySpirit;
            }

            if (IsEnabled(CustomComboPreset.PaladinRequiescatFightOrFlightFeature))
            {
                if (level >= PLD.Levels.FightOrFlight)
                {
                    if (level < PLD.Levels.Requiescat)
                        return PLD.FightOrFlight;

                    // Prefer FoF if it is off cooldown, or if it will be ready sooner than Requiescat.  In practice, this
                    // means that Req should only be returned if FoF is on cooldown and Req is not, ie. immediately after
                    // FoF is cast.  This ensures that the button shows the action that will next be available for use in
                    // that hotbar slot, rather than swapping to FoF at the last instant when FoF comes off cooldown a
                    // a single weave slot earlier than Req.
                    return CalcBestAction(PLD.FightOrFlight, PLD.FightOrFlight, PLD.Requiescat);
                }
            }
        }

        return actionID;
    }
}

internal class PaladinSpiritsWithinCircleOfScorn : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinScornfulSpiritsFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.SpiritsWithin || actionID == PLD.Expiacion || actionID == PLD.CircleOfScorn)
        {
            if (level >= PLD.Levels.Expiacion)
                return CalcBestAction(actionID, PLD.Expiacion, PLD.CircleOfScorn);

            if (level >= PLD.Levels.CircleOfScorn)
                return CalcBestAction(actionID, PLD.SpiritsWithin, PLD.CircleOfScorn);

            return PLD.SpiritsWithin;
        }

        return actionID;
    }
}

internal class PaladinShieldBash : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinShieldBashFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.ShieldBash)
        {
            if (level >= PLD.Levels.LowBlow && IsCooldownUsable(PLD.LowBlow))
                return PLD.LowBlow;
        }

        return actionID;
    }
}
