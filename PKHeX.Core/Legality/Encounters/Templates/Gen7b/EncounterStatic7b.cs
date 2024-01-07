namespace PKHeX.Core;

/// <summary>
/// Generation 7 Static Encounter (<see cref="GameVersion.GG"/>
/// </summary>
public sealed record EncounterStatic7b(GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PB7>, IFlawlessIVCount, IFixedIVSet
{
    public int Generation => 7;
    public EntityContext Context => EntityContext.Gen7b;
    int ILocation.Location => Location;
    public int EggLocation => 0;
    public bool IsShiny => false;
    public bool EggEncounter => false;

    public required ushort Species { get; init; }
    public required byte Level { get; init; }
    public byte Form { get; init; }
    public byte Location { get; init;}
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall { get; init; }
    public Shiny Shiny { get; init; }
    public byte FlawlessIVCount { get; init; }
    public IndividualValueSet IVs { get; init; }

    public string Name => "Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PB7 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PB7 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        var pi = PersonalTable.GG[Species, Form];
        var pk = new PB7
        {
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            OT_Friendship = pi.BaseFriendship,
            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)version,
            MetDate = EncounterDate.GetDateSwitch(),
            Ball = (byte)Ball.Poke,

            HeightScalar = PokeSizeUtil.GetRandomScalar(),
            WeightScalar = PokeSizeUtil.GetRandomScalar(),

            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };
        SetPINGA(pk, criteria, pi);
        pk.ResetHeight();
        pk.ResetWeight();
        pk.ResetCP();
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PB7 pk, EncounterCriteria criteria, PersonalInfo7GG pi)
    {
        pk.PID = Util.Rand32();
        pk.EncryptionConstant = Util.Rand32();
        pk.Nature = (int)criteria.GetNature();
        pk.Gender = criteria.GetGender(pi);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));

        if (IVs.IsSpecified)
            criteria.SetRandomIVs(pk, IVs);
        else
            criteria.SetRandomIVs(pk, FlawlessIVCount);
    }

    #region Matching
    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchEggLocation(pk))
            return false;
        if (pk.Met_Location != Location)
            return false;
        if (pk.Met_Level != Level)
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (FlawlessIVCount != 0 && pk.FlawlessIVCount < FlawlessIVCount)
            return false;
        return true;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
        return pk.Egg_Location == expect;
    }
    #endregion
}
