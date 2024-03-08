using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Settings for Parsing Legality
/// </summary>
/// <remarks><see cref="LegalityAnalysis"/></remarks>
public static class ParseSettings
{
    internal static ITrainerInfo ActiveTrainer { get; set; } = new SimpleTrainerInfo(GameVersion.Any) { OT = string.Empty, Language = -1 };

    /// <summary>
    /// Toggles whether the word filter should be used when checking the data.
    /// </summary>
    public static bool CheckWordFilter { get; set; } = true;

    /// <summary>
    /// Setting to specify if an analysis should permit data sourced from the physical cartridge era of Game Boy games.
    /// </summary>
    /// <remarks>If false, indicates to use Virtual Console rules (which are transferable to Gen7+)</remarks>
    public static bool AllowGBCartEra { private get; set; }

    /// <summary>
    /// Setting to specify if an analysis should permit trading a Generation 1 origin file to Generation 2, then back. Useful for checking RBY Metagame rules.
    /// </summary>
    public static bool AllowGen1Tradeback { get; set; } = true;

    public static Severity NicknamedTrade { get; private set; } = Severity.Invalid;
    public static Severity NicknamedMysteryGift { get; private set; } = Severity.Fishy;
    public static Severity RNGFrameNotFound3 { get; private set; } = Severity.Fishy;
    public static Severity RNGFrameNotFound4 { get; private set; } = Severity.Invalid;
    public static Severity Gen7TransferStarPID { get; private set; } = Severity.Fishy;
    public static Severity Gen8MemoryMissingHT { get; private set; } = Severity.Fishy;
    public static Severity HOMETransferTrackerNotPresent { get; private set; } = Severity.Fishy;
    public static Severity NicknamedAnotherSpecies { get; private set; } = Severity.Fishy;
    public static Severity ZeroHeightWeight { get; private set; } = Severity.Fishy;
    public static Severity CurrentHandlerMismatch { get; private set; } = Severity.Invalid;
    public static bool CheckActiveHandler { get; set; }

    public static IReadOnlyList<string> MoveStrings { get; private set; } = Util.GetMovesList(GameLanguage.DefaultLanguage);
    public static IReadOnlyList<string> SpeciesStrings { get; private set; } = Util.GetSpeciesList(GameLanguage.DefaultLanguage);
    public static string GetMoveName(ushort move) => move >= MoveStrings.Count ? LegalityCheckStrings.L_AError : MoveStrings[move];

    public static void ChangeLocalizationStrings(IReadOnlyList<string> moves, IReadOnlyList<string> species)
    {
        SpeciesStrings = species;
        MoveStrings = moves;
    }

    /// <summary>
    /// Checks to see if Crystal is available to visit/originate from.
    /// </summary>
    /// <remarks>Pokémon Crystal was never released in Korea.</remarks>
    /// <param name="Korean">Korean data being checked</param>
    /// <returns>True if Crystal data is allowed</returns>
    public static bool AllowGen2Crystal(bool Korean) => !Korean;

    /// <summary>
    /// Checks to see if Crystal is available to visit/originate from.
    /// </summary>
    /// <param name="pk">Data being checked</param>
    /// <returns>True if Crystal data is allowed</returns>
    public static bool AllowGen2Crystal(PKM pk) => !pk.Korean;

    /// <summary>
    /// Checks to see if the Move Reminder (Relearner) is available.
    /// </summary>
    /// <remarks> Pokémon Stadium 2 was never released in Korea.</remarks>
    /// <param name="pk">Data being checked</param>
    /// <returns>True if Crystal data is allowed</returns>
    public static bool AllowGen2MoveReminder(PKM pk) => !pk.Korean && AllowGBStadium2;

    public static bool AllowGen2OddEgg(PKM pk) => !pk.Japanese || AllowGBCartEra;

    public static bool AllowGBVirtualConsole3DS => !AllowGBCartEra;
    public static bool AllowGBEraEvents => AllowGBCartEra;
    public static bool AllowGBStadium2 => AllowGBCartEra;

    internal static bool IsFromActiveTrainer(PKM pk) => ActiveTrainer.IsFromTrainer(pk);

    /// <summary>
    /// Initializes certain settings
    /// </summary>
    /// <param name="sav">Newly loaded save file</param>
    /// <returns>Save file is Physical GB cartridge save file (not Virtual Console)</returns>
    public static bool InitFromSaveFileData(SaveFile sav)
    {
        ActiveTrainer = sav;
        return AllowGBCartEra = sav switch
        {
            SAV1 { IsVirtualConsole: true } => false,
            SAV2 { IsVirtualConsole: true } => false,
            { Generation: 1 or 2 } => true,
            _ => false,
        };
    }

    internal static bool IgnoreTransferIfNoTracker => HOMETransferTrackerNotPresent == Severity.Invalid;

    public static void InitFromSettings(IParseSettings settings)
    {
        CheckWordFilter = settings.CheckWordFilter;
        AllowGen1Tradeback = settings.AllowGen1Tradeback;
        NicknamedTrade = settings.NicknamedTrade;
        NicknamedMysteryGift = settings.NicknamedMysteryGift;
        RNGFrameNotFound3 = settings.RNGFrameNotFound3;
        RNGFrameNotFound4 = settings.RNGFrameNotFound4;
        Gen7TransferStarPID = settings.Gen7TransferStarPID;
        HOMETransferTrackerNotPresent = settings.HOMETransferTrackerNotPresent;
        Gen8MemoryMissingHT = settings.Gen8MemoryMissingHT;
        NicknamedAnotherSpecies = settings.NicknamedAnotherSpecies;
        ZeroHeightWeight = settings.ZeroHeightWeight;
        CurrentHandlerMismatch = settings.CurrentHandlerMismatch;
        CheckActiveHandler = settings.CheckActiveHandler;
    }
}

public interface IParseSettings
{
    bool CheckWordFilter { get; }
    bool CheckActiveHandler { get; }
    bool AllowGen1Tradeback { get; }

    Severity NicknamedTrade { get; }
    Severity NicknamedMysteryGift { get; }
    Severity RNGFrameNotFound3 { get; }
    Severity RNGFrameNotFound4 { get; }
    Severity Gen7TransferStarPID { get; }
    Severity Gen8MemoryMissingHT { get; }
    Severity HOMETransferTrackerNotPresent { get; }
    Severity NicknamedAnotherSpecies { get; }
    Severity ZeroHeightWeight { get; }
    Severity CurrentHandlerMismatch { get; }
}

public interface IBulkAnalysisSettings
{
    bool CheckActiveHandler { get; }
}
