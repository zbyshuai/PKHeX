using System;

namespace PKHeX.Core;

/// <summary>Generation 5 Trade with Fixed PID</summary>
public sealed record EncounterTrade5BW : IEncounterable, IEncounterMatch, IFixedTrainer, IFixedNickname, IFixedGender, IFixedNature, IEncounterConvertible<PK5>
{
    public int Generation => 5;
    public EntityContext Context => EntityContext.Gen5;
    public int Location => Locations.LinkTrade5NPC;
    public bool IsFixedNickname => true;
    public GameVersion Version { get; }
    public Shiny Shiny => Shiny.Never;
    public bool EggEncounter => false;
    public int EggLocation => 0;
    public Ball FixedBall => Ball.Poke;
    public bool IsShiny => false;
    public bool IsFixedTrainer => true;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    private string[] TrainerNames { get; }
    private string[] Nicknames { get; }

    public required ushort Species { get; init; }
    public required byte Level { get; init; }
    public required AbilityPermission Ability { get; init; }
    public required byte OTGender { get; init; }
    public required ushort ID32 { get; init; }
    public required byte Gender { get; init; }
    public required IndividualValueSet IVs { get; init; }
    public required Nature Nature { get; init; }
    public byte Form { get; init; }

    /// <summary> Fixed <see cref="PKM.PID"/> value the encounter must have.</summary>
    public uint PID { get; }

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;

    public EncounterTrade5BW(ReadOnlySpan<string[]> names, byte index, GameVersion version, uint pid)
    {
        Version = version;
        Nicknames = EncounterUtil.GetNamesForLanguage(names, index);
        TrainerNames = EncounterUtil.GetNamesForLanguage(names, (uint)(index + (names[1].Length >> 1)));
        PID = pid;
    }

    /// <summary>
    /// Checks if the language can be missing.
    /// </summary>
    /// <remarks>
    /// Generation 5 trades from B/W forgot to set the Language ID, so it remains as 0.
    /// <br/> This value is corrected when the entity is transferred from PK5->PK6.
    /// <br/> B2/W2 is unaffected by this game data bug.
    /// </remarks>
    public static bool IsValidMissingLanguage(PKM pk) => pk.Format == 5;

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.BW[Species];
        var pk = new PK5
        {
            PID = PID,
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            Met_Location = Location,
            Met_Level = Level,
            MetDate = EncounterDate.GetDateNDS(),
            Gender = Gender,
            Nature = (byte)Nature,
            Ball = (byte)FixedBall,

            ID32 = ID32,
            Version = (byte)version,
            Language = lang == 1 ? 0 : lang, // Trades for JPN games have language ID of 0, not 1.
            OT_Gender = OTGender,
            OT_Name = TrainerNames[lang],

            OT_Friendship = pi.BaseFriendship,

            IsNicknamed = IsFixedNickname,
            Nickname = IsFixedNickname ? Nicknames[lang] : SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        EncounterUtil1.SetEncounterMoves(pk, version, Level);
        criteria.SetRandomIVs(pk, IVs);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        pk.ResetPartyStats();

        return pk;
    }

    #endregion

    #region Matching

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => (uint)language < TrainerNames.Length && trainer.SequenceEqual(TrainerNames[language]);
    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language) => (uint)language < Nicknames.Length && nickname.SequenceEqual(Nicknames[language]);
    public string GetNickname(int language) => (uint)language < Nicknames.Length ? Nicknames[language] : Nicknames[0];

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk.Met_Level != Level)
            return false;
        if (!Legal.GetIsFixedIVSequenceValidNoRand(IVs, pk))
            return false;
        if (!IsMatchNatureGenderShiny(pk))
            return false;
        if (pk.ID32 != ID32)
            return false;
        if (evo.Form != Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (pk.OT_Gender != OTGender)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        return true;
    }

    private bool IsMatchNatureGenderShiny(PKM pk)
    {
        if (PID != pk.EncryptionConstant)
            return false;
        if ((int)Nature != pk.Nature)
            return false;
        return true;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var expect = EggLocation;
        if (pk is PB8)
            expect = Locations.Default8bNone;
        return pk.Egg_Location == expect;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion
}
