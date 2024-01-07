namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen1"/>.
/// </summary>
public sealed record EncounterSlot1(EncounterArea1 Parent, ushort Species, byte LevelMin, byte LevelMax, byte SlotNumber)
    : IEncounterConvertible<PK1>, IEncounterable, IEncounterMatch, INumberedSlot
{
    public int Generation => 1;
    public EntityContext Context => EntityContext.Gen1;
    public bool EggEncounter => false;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => TransporterLogic.IsHiddenDisallowedVC1(Species) ? AbilityPermission.OnlyFirst : AbilityPermission.OnlyHidden;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;

    public byte Form => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public int Location => Parent.Location;
    public SlotType Type => Parent.Type;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);

    public PK1 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK1 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, Version);
        var isJapanese = lang == (int)LanguageID.Japanese;
        var pi = EncounterUtil1.GetPersonal1(Version, Species);
        var pk = new PK1(isJapanese)
        {
            Species = Species,
            CurrentLevel = LevelMin,
            Catch_Rate = EncounterUtil1.GetWildCatchRate(Version, Species),
            DV16 = EncounterUtil1.GetRandomDVs(Util.Rand),

            OT_Name = EncounterUtil1.GetTrainerName(tr, lang),
            TID16 = tr.TID16,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            Type1 = pi.Type1,
            Type2 = pi.Type2,
        };

        EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }
    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (LevelMin > evo.LevelMax)
            return false;

        if (pk is not PK1 pk1)
            return true;

        var rate = pk1.Catch_Rate;
        var expect = EncounterUtil1.GetWildCatchRate(Version, Species);
        if (expect != rate && !(ParseSettings.AllowGen1Tradeback && GBRestrictions.IsTradebackCatchRate(rate)))
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;
    #endregion
}
