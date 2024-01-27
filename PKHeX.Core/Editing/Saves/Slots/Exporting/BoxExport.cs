using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using H2Core.Resource;


namespace PKHeX.Core;

/// <summary>
/// Logic for exporting a <see cref="SaveFile"/> to a folder of <see cref="PKM"/> files.
/// </summary>
public static class BoxExport
{
    /// <summary>
    /// File namer to use for exporting if none is provided.
    /// </summary>
    private static IFileNamer<PKM> Default => EntityFileNamer.Namer;

    /// <summary>
    /// Export a box in the <see cref="SaveFile"/> to the specified folder.
    /// </summary>
    /// <param name="sav">Save file to export</param>
    /// <param name="destPath">Folder to export to</param>
    /// <param name="box">Box to export</param>
    /// <param name="settings">Settings to use for exporting</param>
    public static int Export(SaveFile sav, string destPath, int box, BoxExportSettings settings)
        => Export(sav, destPath, box, Default, settings);

    /// <summary>
    /// Export a box in the <see cref="SaveFile"/> to the specified folder.
    /// </summary>
    /// <param name="sav">Save file to export</param>
    /// <param name="destPath">Folder to export to</param>
    /// <param name="box">Box to export</param>
    /// <param name="namer">File namer to use for exporting</param>
    /// <param name="settings">Settings to use for exporting</param>
    public static int Export(SaveFile sav, string destPath, int box, IFileNamer<PKM> namer, BoxExportSettings settings)
        => ExportBox(sav, destPath, namer, box, settings, sav.BoxSlotCount, sav.SlotCount);

    /// <summary>
    /// Export all boxes in the <see cref="SaveFile"/> to the specified folder.
    /// </summary>
    /// <param name="sav">Save file to export</param>
    /// <param name="destPath">Folder to export to</param>
    /// <param name="settings">Settings to use for exporting</param>
    public static int Export(SaveFile sav, string destPath, BoxExportSettings settings)
        => Export(sav, destPath, Default, settings);

    /// <summary>
    /// Export all boxes in the <see cref="SaveFile"/> to the specified folder.
    /// </summary>
    /// <param name="sav">Save file to export</param>
    /// <param name="destPath">Folder to export to</param>
    /// <param name="namer">File namer to use for exporting</param>
    /// <param name="settings">Settings to use for exporting</param>
    /// <returns>Number of files exported</returns>
    public static int Export(SaveFile sav, string destPath, IFileNamer<PKM> namer, BoxExportSettings settings)
    {
        if (!sav.HasBox)
            return 0;

        int total = sav.SlotCount;
        int boxSlotCount = sav.BoxSlotCount;

        var startBox = settings.Scope == BoxExportScope.Current ? sav.CurrentBox : 0;
        var endBox = settings.Scope == BoxExportScope.Current ? startBox + 1 : sav.BoxCount;

        var ctr = 0;
        // Export each box specified.
        for (int box = startBox; box < endBox; box++)
        {
            var boxFolder = destPath;
            if (settings.FolderCreation == BoxExportFolderMode.FolderEachBox)
            {
                var folderName = GetFolderName(sav, box, settings.FolderPrefix);
                boxFolder = Path.Combine(destPath, folderName);
                Directory.CreateDirectory(boxFolder);
            }
            ctr += ExportBox(sav, boxFolder, namer, box, settings, boxSlotCount, total);
        }
        return ctr;
    }

    private static int ExportBox(SaveFile sav, string destPath, IFileNamer<PKM> namer, int box, BoxExportSettings settings,
        int boxSlotCount, int total)
    {
        int count = GetSlotCountForBox(boxSlotCount, box, total);
        int ctr = 0;
        // Export each slot in the box.
        for (int slot = 0; slot < count; slot++)
        {
            var pk = sav.GetBoxSlotAtIndex(box, slot);
            if (IsUndesirableForExport(pk))
            {
                if (settings.EmptySlots == BoxExportEmptySlots.Skip)
                    continue;
            }

            
            var fileNameMode = settings.FileNameMode;
            var fileName = GetFileName(pk, settings.FileIndexPrefix, namer, box, slot, boxSlotCount, fileNameMode);
            var fn = Path.Combine(destPath, fileName);
            File.WriteAllBytes(fn, pk.DecryptedPartyData);
            ctr++;
        }
        return ctr;
    }

    private static bool IsUndesirableForExport(PKM pk) => pk.Species == 0 || !pk.Valid;

    private static int GetSlotCountForBox(int boxSlotCount, int box, int total)
    {
        // Account for any jagged-boxes with less than the usual number of slots.
        int absoluteStart = boxSlotCount * box;
        int count = boxSlotCount;
        if (absoluteStart + count > total)
            count = total - absoluteStart;
        return count;
    }

    private static string GetFolderName(SaveFile sav, int box, BoxExportFolderNaming mode)
    {
        var boxName = Util.CleanFileName(sav.GetBoxName(box));
        return mode switch
        {
            BoxExportFolderNaming.BoxName => boxName,
            BoxExportFolderNaming.Index => $"{box + 1:00}",
            BoxExportFolderNaming.IndexBoxName => $"{box + 1:00} {boxName}",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };
    }

    private static string GetFileName(PKM pk, BoxExportIndexPrefix mode, IFileNamer<PKM> namer, int box, int slot, int boxSlotCount, BoxExportFileNameMode FileNameMode)
    {
        string slotName;
        if (FileNameMode == BoxExportFileNameMode.H2)
            slotName = GetH2Name(namer, pk);
        else
            slotName = GetInnerName(namer, pk);
        
        var fileName = Util.CleanFileName(slotName);
        var prefix = GetPrefix(mode, box, slot, boxSlotCount);

        return $"{prefix}{fileName}.{pk.Extension}";
    }

    private static string GetPrefix(BoxExportIndexPrefix mode, int box, int slot, int boxSlotCount) => mode switch
    {
        BoxExportIndexPrefix.None => string.Empty,
        BoxExportIndexPrefix.InAll => $"{(box * boxSlotCount) + slot:0000} - ",
        BoxExportIndexPrefix.InBox => $"{slot:00} - ",
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
    };
    


    private static string GetH2Name(IFileNamer<PKM> namer, PKM pk)
    {
        int[] BallItemIDs  = [ 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 492, 493, 494, 495, 496, 497, 498, 499, 576, 851, 1785, 1710, 1711, 1712, 1713, 1746, 1747, 1748, 1749, 1750, 1771 ];

        string slotName = "";
        string SpeciesInfo = "";
        
        if (pk.IsNicknamed == true)
        {
            SpeciesInfo = pk.Nickname;
            slotName = $"{pk.Species} - {SpeciesInfo} - {pk.PID}";
        }
        if (pk.IsEgg == true)
        {
            SpeciesInfo = GameInfo.GetStrings("zh").Species[ pk.Species ] + "蛋";
            slotName = $"{pk.Species} - {SpeciesInfo} - {pk.PID}";
        }
        else
        {
            slotName = GetInnerName(namer, pk);
        }
        // else
        // {
        //     // 根据规则生成名字
        //     string IVSpecial = "";
        //     int Vs = 0;
        //     if (pk.IV_ATK == 31)
        //         Vs += 1;
        //     if (pk.IV_DEF == 31)
        //         Vs += 1;
        //     if (pk.IV_HP == 31)
        //         Vs += 1;
        //     if (pk.IV_SPA == 31)
        //         Vs += 1;
        //     if (pk.IV_SPD == 31)
        //         Vs += 1;
        //     if (pk.IV_SPE == 31)
        //         Vs += 1;
        //     if (pk.IV_ATK == 0)
        //         IVSpecial += "0A";
        //     if (pk.IV_SPD == 0)
        //         IVSpecial += "0S";
                
        //     string Ball = Properties.Resources.text_Items_zh.Split("\n")[ BallItemIDs[pk.Ball] ];
        //     string IVs = Vs < 4 ? "" : IVSpecial == "" ? $"{Vs}V" : $"{Vs}V" + IVSpecial;
        //     string Specie = GameInfo.GetStrings("zh").Species[ pk.Species ];
        //     string Item = Properties.Resources.text_Items_zh.Split("\n")[ pk.HeldItem ];
            
        //     SpeciesInfo = $"{Ball}-";
        //     SpeciesInfo += pk.IsShiny ? "闪" + Specie : IVs != "" ? Specie + IVs : Item != "无" ? $"{Specie}(携带:{Item})" : "";
        // }

        
        
        
        
        try
        {
            return Util.CleanFileName(slotName);
        }
        catch { return "Name Error"; }
    }

    

    private static string GetInnerName(IFileNamer<PKM> namer, PKM pk)
    {
        try
        {
            var slotName = namer.GetName(pk);
            return Util.CleanFileName(slotName);
        }
        catch { return "Name Error"; }
    }
}
