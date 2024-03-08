using System;
using System.Runtime.CompilerServices;
using static PKHeX.Core.LeadRequired;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.SlotType4;

namespace PKHeX.Core;

/// <summary>
/// Method J logic used by <see cref="DPPt"/> RNG.
/// </summary>
public static class MethodJ
{
    /// <summary>
    /// High-level method to get the first possible encounter conditions.
    /// </summary>
    /// <param name="enc">Encounter template.</param>
    /// <param name="seed">Seed that immediately generates the PID.</param>
    /// <param name="evo">Level range constraints for the capture, if known.</param>
    /// <param name="format">Current format (different from 4)</param>
    public static LeadSeed GetSeed<TEnc, TEvo>(TEnc enc, uint seed, TEvo evo, byte format)
        where TEnc : IEncounterSlot4
        where TEvo : ILevelRange
    {
        var pid = ClassicEraRNG.GetSequentialPID(seed);
        var nature = (byte)(pid % 25);

        var frames = GetReversalWindow(seed, nature);
        return GetOriginSeed(enc, seed, nature, frames, evo.LevelMin, evo.LevelMax, format);
    }

    /// <inheritdoc cref="GetSeed{TEnc,TEvo}"/>
    public static LeadSeed GetSeed<TEnc>(TEnc enc, uint seed, byte format)
        where TEnc : IEncounterSlot4 => GetSeed(enc, seed, enc, format);

    // Summary of Random Determinations:
    // For constant-value rand choices, the games avoid using modulo via:
    // Rand(x) == rand16() / ((0xFFFF/x) + 1)
    // For variable rand choices (like level & Static/Magnet Pull), they use modulo.
    // Nature:                       rand() / 0xA3E == nature (1/25 odds)
    // Cute Charm:                   rand() / 0x5556 != 0; (2/3 odds)
    // Sync:                         rand() >> 15 == 0; (50% odds)
    // Static/Magnet Pull:           rand() >> 15 == 0;
    // Pressure/Hustle/Vital Spirit: rand() >> 15 == 1;
    // Intimidate/Keen Eye:          rand() >> 15 == 1; -- 0 will reject the encounter.
    // Feebas Spot:                  rand() >> 15 == 1; -- 0 will use regular encounters.

    private const byte Format = 4;

    private static bool IsCuteCharmFail(uint rand) => (rand / 0x5556) == 0; // 1/3 odds
    private static bool IsCuteCharmPass(uint rand) => (rand / 0x5556) != 0; // 2/3 odds

  //private static bool IsSyncFail(uint rand) => (rand >> 15) != 0;
    private static bool IsSyncPass(uint rand) => (rand >> 15) == 0;

    private static bool IsStaticMagnetFail(uint rand) => (rand >> 15) != 0;
    private static bool IsStaticMagnetPass(uint rand) => (rand >> 15) == 0;

    private static bool IsHustleVitalFail(uint rand) => (rand >> 15) != 1;
    private static bool IsHustleVitalPass(uint rand) => (rand >> 15) == 1;

  //private static bool IsIntimidateKeenEyeFail(uint rand) => (rand >> 15) != 1;
    private static bool IsIntimidateKeenEyePass(uint rand) => (rand >> 15) == 1;

    private static bool IsFeebasChance(uint rand) => (rand >> 15) == 1;

    private static uint GetNature(uint rand) => rand / 0xA3Eu;

    /// <summary>
    /// Gets the first possible origin seed and lead for the input encounter &amp; constraints.
    /// </summary>
    public static LeadSeed GetOriginSeed<T>(T enc, uint seed, byte nature, int reverseCount, byte levelMin, byte levelMax, byte format = Format)
        where T : IEncounterSlot4
    {
        LeadSeed prefer = default;
        while (true)
        {
            if (TryGetMatch(enc, levelMin, levelMax, seed, nature, format, out var result))
            {
                if (CheckEncounterActivation(enc, ref result))
                {
                    if (result.IsNoRequirement())
                        return result;
                    if (result.IsBetterThan(prefer))
                        prefer = result;
                }
            }
            if (reverseCount == 0)
                return prefer;
            reverseCount--;
            seed = LCRNG.Prev2(seed);
        }
    }

    private static bool CheckEncounterActivation<T>(T enc, ref LeadSeed result)
        where T : IEncounterSlot4
    {
        if (enc.Type.IsFishingRodType())
        {
            if (enc is EncounterSlot4 { Parent.IsCoronetFeebasArea: true } s4)
            {
                if (!IsValidCoronetB1F(s4, ref result.Seed))
                    return false;
            }
            // D/P don't reference Suction Cups or Sticky Hold.
            return enc is IVersion { Version: Pt }
                ? IsFishPossible(enc.Type, ref result.Seed, ref result.Lead)
                : IsFishPossible(enc.Type, ref result.Seed);
        }
        // Can sweet scent trigger.
        return true;
    }

    private static bool CheckEncounterActivation<T>(T enc, ref uint result)
        where T : IEncounterSlot4
    {
        // Lead is required to be Cute Charm.
        if (enc.Type.IsFishingRodType())
        {
            if (enc is EncounterSlot4 { Parent.IsCoronetFeebasArea: true } s4)
            {
                if (!IsValidCoronetB1F(s4, ref result))
                    return false;
            }
            return IsFishPossible(enc.Type, ref result);
        }
        // Can sweet scent trigger.
        return true;
    }

    private static bool IsValidCoronetB1F(ISpeciesForm s4, ref uint result)
    {
        // The game rolls to check if it might need to replace the slots with Feebas.
        // This occurs in Mt. Coronet B1F; if passed, check if the player is on a Feebas tile before replacing.
        // 0 - Hook
        // 1 - CheckTiles -- current seed
        // 2- ESV
        if (s4.Species is (int)Species.Feebas && !IsFeebasChance(result >> 16))
            return false;

        // Regular slots don't need to falsify the rand().
        // Players can just fish from a non-Feebas tile.
        // The rand() call is unavoidable when in the area.
        result = LCRNG.Prev(result);
        return true;
    }

    /// <summary>
    /// Attempts to find a matching seed for the given encounter and constraints for Cute Charm buffered PIDs.
    /// </summary>
    public static bool TryGetMatchCuteCharm<T>(T enc, ReadOnlySpan<uint> seeds, byte nature, byte levelMin, byte levelMax, out uint result)
        where T : IEncounterSlot4
    {
        foreach (uint seed in seeds)
        {
            var p0 = seed >> 16; // 0
            var reg = GetNature(p0) == nature;
            if (!reg)
                continue;
            var ctx = new FrameCheckDetails<T>(enc, seed, levelMin, levelMax, 4);
            if (!TryGetMatchCuteCharm(ctx, out result))
                continue;
            if (!CheckEncounterActivation(enc, ref result))
                continue;
            return true;
        }
        result = default; return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetMatch<T>(T enc, byte levelMin, byte levelMax, uint seed, byte nature, byte format, out LeadSeed result)
        where T : IEncounterSlot4
    {
        var p0 = seed >> 16; // 0
        var reg = GetNature(p0) == nature;
        if (reg)
        {
            var ctx = new FrameCheckDetails<T>(enc, seed, levelMin, levelMax, format);
            return TryGetMatchNoSync(ctx, out result);
        }
        var syncProc = IsSyncPass(p0);
        if (syncProc)
        {
            var ctx = new FrameCheckDetails<T>(enc, seed, levelMin, levelMax, format);
            if (IsSlotValidRegular(ctx, out seed))
            {
                result = new(seed, Synchronize);
                return true;
            }
        }
        result = default;
        return false;
    }

    private static bool TryGetMatchCuteCharm<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsCuteCharmFail(ctx.Prev1))
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidCuteCharmFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsCuteCharmPass(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidSyncFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsSyncPass(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidIntimidate<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        // Requires lead with level 5+ above the encounter's level. Always possible.
        if (IsIntimidateKeenEyePass(ctx.Prev1)) // encounter routine aborted
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidHustleVitalFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsHustleVitalPass(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool TryGetMatchNoSync<T>(in FrameCheckDetails<T> ctx, out LeadSeed result)
        where T : IEncounterSlot4
    {
        if (IsSlotValidRegular(ctx, out uint seed))
        { result = new(seed, None); return true; }

        if (IsSlotValidSyncFail(ctx, out seed))
        { result = new(seed, SynchronizeFail); return true; }
        if (IsSlotValidCuteCharmFail(ctx, out seed))
        { result = new(seed, CuteCharmFail); return true; }
        if (IsSlotValidHustleVitalFail(ctx, out seed))
        { result = new(seed, PressureHustleSpiritFail); return true; }
        if (IsSlotValidStaticMagnetFail(ctx, out seed))
        { result = new(seed, StaticMagnetFail); return true; }
        // Intimidate/Keen Eye failing will result in no encounter.

        if (IsSlotValidStaticMagnet(ctx, out seed, out var lead))
        { result = new(seed, lead); return true; }
        if (IsSlotValidHustleVital(ctx, out seed))
        { result = new(seed, PressureHustleSpirit); return true; }
        if (IsSlotValidIntimidate(ctx, out seed))
        { result = new(seed, IntimidateKeenEyeFail); return true; }

        result = default; return false;
    }

    private static bool IsSlotValidFrom1Skip<T>(FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (!ctx.Encounter.IsFixedLevel())
        {
            if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev2))
            {
                if (IsSlotValid(ctx.Encounter, ctx.Prev3))
                { result = ctx.Seed4; return true; }
            }
        }
        else // Not random level
        {
            if (IsSlotValid(ctx.Encounter, ctx.Prev2))
            { result = ctx.Seed3; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidRegular<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (!ctx.Encounter.IsFixedLevel())
        {
            if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
            {
                if (IsSlotValid(ctx.Encounter, ctx.Prev2))
                { result = ctx.Seed3; return true; }
            }
        }
        else // Not random level
        {
            if (IsSlotValid(ctx.Encounter, ctx.Prev1))
            { result = ctx.Seed2; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidHustleVital<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsHustleVitalFail(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        var expectLevel = ctx.Encounter.PressureLevel;
        if (!IsOriginalLevelValid(ctx.LevelMin, ctx.LevelMax, ctx.Format, expectLevel))
        { result = default; return false; }

        if (!ctx.Encounter.IsFixedLevel())
        {
            // Don't bother evaluating Prev1 for level, as it's always bumped to max after.
            if (IsSlotValid(ctx.Encounter, ctx.Prev3))
            { result = ctx.Seed4; return true; }
        }
        else // Not random level
        {
            if (IsSlotValid(ctx.Encounter, ctx.Prev2))
            { result = ctx.Seed3; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidStaticMagnet<T>(in FrameCheckDetails<T> ctx, out uint result, out LeadRequired lead)
        where T : IEncounterSlot4
    {
        lead = None;
        if (!ctx.Encounter.IsFixedLevel())
        {
            if (IsStaticMagnetFail(ctx.Prev3)) // should have triggered
            { result = default; return false; }

            if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
            {
                if (ctx.Encounter.IsSlotValidStaticMagnet(ctx.Prev2, out lead))
                { result = ctx.Seed4; return true; }
            }
        }
        else // Not random level
        {
            if (IsStaticMagnetFail(ctx.Prev3)) // should have triggered
            { result = default; return false; }

            if (ctx.Encounter.IsSlotValidStaticMagnet(ctx.Prev1, out lead))
            { result = ctx.Seed3; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidStaticMagnetFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (!ctx.Encounter.IsFixedLevel())
        {
            if (IsStaticMagnetPass(ctx.Prev3)) // should have triggered
            { result = default; return false; }

            if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
            {
                if (IsSlotValid(ctx.Encounter, ctx.Prev2))
                { result = ctx.Seed4; return true; }
            }
        }
        else // Not random level
        {
            if (IsStaticMagnetPass(ctx.Prev2)) // should have triggered
            { result = default; return false; }

            if (IsSlotValid(ctx.Encounter, ctx.Prev1))
            { result = ctx.Seed3; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValid<T>(T enc, uint u16SlotRand)
        where T : IEncounterSlot4
    {
        if (enc.Type is HoneyTree)
            return true; // pre-determined
        var slot = SlotMethodJ.GetSlot(enc.Type, u16SlotRand);
        return slot == enc.SlotNumber;
    }

    private static bool IsLevelValid<T>(T enc, byte min, byte max, byte format, uint u16LevelRand) where T : ILevelRange
    {
        var level = GetExpectedLevel(enc, u16LevelRand);
        return IsOriginalLevelValid(min, max, format, level);
    }

    private static bool IsOriginalLevelValid(byte min, byte max, byte format, uint level)
    {
        if (format == Format)
            return level == min; // Met Level matches
        return LevelRangeExtensions.IsLevelWithinRange((int)level, min, max);
    }

    private static uint GetExpectedLevel(ILevelRange enc, uint u16LevelRand)
    {
        uint mod = 1u + enc.LevelMax - enc.LevelMin;
        return (u16LevelRand % mod) + enc.LevelMin;
    }

    /// <summary>
    /// Gets the amount of reverses allowed prior to another satisfactory PID being generated.
    /// </summary>
    /// <param name="seed">Seed that generates the expected resulting PID.</param>
    /// <param name="nature">Nature of the resulting PID.</param>
    /// <returns>Count of reverses allowed for no specific lead (not cute charm).</returns>
    public static int GetReversalWindow(uint seed, byte nature)
    {
        int ctr = 0;
        // Seed is currently the second RNG call. Unroll to the first.
        uint b = seed >> 16;
        while (true)
        {
            var a = LCRNG.Prev16(ref seed);
            var pid = b << 16 | a;
            if (pid % 25 == nature)
                break;
            b = LCRNG.Prev16(ref seed);
            ctr++;
        }
        return ctr;
    }

    private static bool IsFishPossible(SlotType4 encType, ref uint seed)
    {
        var rodRate = GetRodRate(encType);
        var u16 = seed >> 16;
        var roll = u16 / 656;
        if (roll < rodRate)
        {
            seed = LCRNG.Prev(seed);
            return true;
        }
        return false;
    }

    private static bool IsFishPossible(SlotType4 encType, ref uint seed, ref LeadRequired lead)
    {
        var rodRate = GetRodRate(encType);
        var u16 = seed >> 16;
        var roll = u16 / 656;
        if (roll < rodRate)
        {
            seed = LCRNG.Prev(seed);
            return true;
        }

        if (lead != None)
            return false;

        // Suction Cups / Sticky Hold
        if (roll < rodRate * 2)
        {
            seed = LCRNG.Prev(seed);
            lead = SuctionCups;
            return true;
        }

        return false;
    }

    private static byte GetRodRate(SlotType4 type) => type switch
    {
        Old_Rod => 25,
        Good_Rod => 50,
        Super_Rod => 75,
        _ => 0,
    };

    private static bool IsFishingRodType(this SlotType4 t) => t
        is Old_Rod
        or Good_Rod
        or Super_Rod;
}
