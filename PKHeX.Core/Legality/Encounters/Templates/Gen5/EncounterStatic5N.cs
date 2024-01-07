using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Static Encounter from N
/// </summary>
public sealed record EncounterStatic5N(uint PID)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK5>, IFixedTrainer, IFixedNature
{
    public int Generation => 5;
    public EntityContext Context => EntityContext.Gen5;
    public GameVersion Version => GameVersion.B2W2;
    public const bool NSparkle = true;
    public bool IsFixedTrainer => true;
    private const uint ID32 = 2;
    private const byte IV = 30;

    public byte Form => 0;
    int ILocation.Location => Location;
    public bool IsShiny => false;
    public Shiny Shiny => Shiny.FixedValue;
    public bool EggEncounter => false;
    public int EggLocation => 0;
    public Ball FixedBall => Species == (int)Core.Species.Zorua ? Ball.Poke : Ball.None; // Zorua can't be captured; others can.

    public required ushort Species { get; init; }
    public required byte Level { get; init; }
    public required byte Location { get; init; }
    public required Nature Nature { get; init; }
    public required AbilityPermission Ability { get; init; }

    public string Name => "Dream Radar Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.B2W2[Species];
        var pk = new PK5
        {
            Species = Species,
            CurrentLevel = LevelMin,
            Met_Location = Location,
            Met_Level = LevelMin,
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)(FixedBall != Ball.None ? FixedBall : Ball.Poke),

            Version = (byte)version,
            Language = lang,

            OT_Friendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            IV_HP = IV,
            IV_ATK = IV,
            IV_DEF = IV,
            IV_SPA = IV,
            IV_SPD = IV,
            IV_SPE = IV,

            NSparkle = NSparkle,
            OT_Name = GetOT(lang),
            OT_Gender = 0,
            ID32 = ID32,
            PID = PID,
            Nature = (byte)Nature,
            Gender = pi.Genderless ? 2 : 0,
            Ability = Ability switch
            {
                AbilityPermission.OnlyFirst => pi.Ability1,
                AbilityPermission.OnlySecond => pi.Ability2,
                _ => pi.AbilityH,
            },
            HiddenAbility = Ability == AbilityPermission.OnlyHidden,
        };

        EncounterUtil1.SetEncounterMoves(pk, version, LevelMin);

        pk.ResetPartyStats();

        return pk;
    }

    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (PID != pk.PID)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        if (pk.Met_Location != Location)
            return false;
        if (pk.Met_Level != Level)
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        var pi = PersonalTable.B2W2.GetFormEntry(Species, Form);
        if (pk.Gender != (pi.Genderless ? 2 : 0))
            return false;
        if (pk.OT_Gender != 0)
            return false;
        if (pk is not { IV_HP: IV, IV_ATK: IV, IV_DEF: IV, IV_SPA: IV, IV_SPD: IV, IV_SPE: IV })
            return false;
        if (pk.ID32 != ID32)
            return false;
        return true;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
        return pk.Egg_Location == expect;
    }

    private static string GetOT(int lang) => lang == (int)LanguageID.Japanese ? "Ｎ" : "N";
    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int lang) => trainer.SequenceEqual(GetOT(lang));

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion
}
