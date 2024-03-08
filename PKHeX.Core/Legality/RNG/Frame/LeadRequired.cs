using static PKHeX.Core.LeadRequired;

namespace PKHeX.Core;

/// <summary>
/// Indicates the requirement of the player's lead Pokémon, first sent out when starting a battle.
/// </summary>
/// <remarks>Ordered by increasing preference.</remarks>
public enum LeadRequired : byte
{
    /// <summary> Sentinel value for invalid/impossible to yield lead conditions. </summary>
    Invalid = 0,

    // Illuminate can't "fail" to apply.
    // Suction Cups failing will fail to yield the encounter, otherwise we'd have no lead required.
    // Intimidate succeeding will fail to yield the encounter.

    /// <inheritdoc cref="Static"/>
    StaticMagnetFail,
    /// <inheritdoc cref="CuteCharm"/>
    CuteCharmFail,
    /// <inheritdoc cref="Synchronize"/>
    SynchronizeFail,
    /// <inheritdoc cref="PressureHustleSpirit"/>
    PressureHustleSpiritFail,

    /// <summary> <see cref="Ability.Intimidate"/> or <see cref="Ability.KeenEye"/> failed to activate. </summary>
    IntimidateKeenEyeFail,

    /// <summary> <see cref="Ability.Illuminate"/> or <see cref="Ability.ArenaTrap"/> </summary>
    Illuminate,
    /// <summary> <see cref="Ability.SuctionCups"/> or <see cref="Ability.StickyHold"/> </summary>
    SuctionCups,

    /// <summary> <see cref="Ability.Pressure"/> or <see cref="Ability.Hustle"/> or <see cref="Ability.VitalSpirit"/> </summary>
    PressureHustleSpirit,
    /// <summary> <see cref="Ability.MagnetPull"/> </summary>
    MagnetPull,
    /// <summary> <see cref="Ability.Static"/> </summary>
    Static,
    /// <summary> <see cref="Ability.CuteCharm"/> </summary>
    CuteCharm,
    /// <summary> <see cref="Ability.Synchronize"/> </summary>
    Synchronize,

    /// <summary> No Lead ability effect is present, or is not checked for this type of frame. </summary>
    None = byte.MaxValue,
}

public static class LeadRequiredExtensions
{
    public static bool IsFailTuple(this LeadRequired lr) => lr
        is PressureHustleSpiritFail
        or SynchronizeFail
        or CuteCharmFail
        or StaticMagnetFail;

    public static LeadRequired GetRegular(this LeadRequired lr) => lr switch
    {
        PressureHustleSpiritFail => PressureHustleSpirit,
        SynchronizeFail          => Synchronize,
        CuteCharmFail            => CuteCharm,
        StaticMagnetFail         => Static,
        _                        => None,
    };

    public static Ability GetAbility(this LeadRequired lr) => lr switch
    {
        Synchronize           => Ability.Synchronize,
        CuteCharm             => Ability.CuteCharm,
        Static                => Ability.Static,
        MagnetPull            => Ability.MagnetPull,
        PressureHustleSpirit  => Ability.Pressure,
        SuctionCups           => Ability.SuctionCups,
        Illuminate            => Ability.Illuminate,
        IntimidateKeenEyeFail => Ability.Intimidate,
        _                     => Ability.None,
    };

    public static (Ability Ability, bool IsFail) GetDisplayAbility(this LeadRequired lr)
    {
        var isFail = false;
        if (lr.IsFailTuple())
        {
            isFail = true;
            lr = lr.GetRegular();
        }
        else if (lr is IntimidateKeenEyeFail)
        {
            isFail = true;
        }
        return (lr.GetAbility(), isFail);
    }
}
