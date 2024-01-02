using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public sealed class SummaryPreviewer
{
    private readonly ToolTip ShowSet = new() { InitialDelay = 200, IsBalloon = false, AutoPopDelay = 32_767 };
    private readonly CryPlayer Cry = new();
    private readonly PokePreview Previewer = new();
    private CancellationTokenSource _source = new();
    private static HoverSettings Settings => Main.Settings.Hover;
    private static ISaveFileProvider SAV { get; } = null!;
    private static IPKMView Editor { get; } = null!;

    public void Show(Control pb, PKM pk)
    {
        if (pk.Species == 0)
        {
            Clear();
            return;
        }

        if (Settings.HoverSlotShowPreview && Control.ModifierKeys != Keys.Alt)
            UpdatePreview(pb, pk);
        else if (Settings.HoverSlotShowText)
            ShowSet.SetToolTip(pb, GetPreviewText(pk, new LegalityAnalysis(pk)));
        if (Settings.HoverSlotPlayCry)
            Cry.PlayCry(pk, pk.Context);
    }

    private void UpdatePreview(Control pb, PKM pk)
    {
        _source.Cancel();
        _source = new();
        UpdatePreviewPosition(new());
        Previewer.Populate(pk);
        Previewer.Show();
    }

    public void UpdatePreviewPosition(Point location)
    {
        var cLoc = Cursor.Position;
        var shift = Settings.PreviewCursorShift;
        cLoc.Offset(shift);
        Previewer.Location = cLoc;
    }

    public void Show(Control pb, IEncounterInfo enc)
    {
        if (enc.Species == 0)
        {
            Clear();
            return;
        }

        if (Settings.HoverSlotShowText)
            ShowSet.SetToolTip(pb, GetPreviewText(enc));
        if (Settings.HoverSlotPlayCry)
            Cry.PlayCry(enc, enc.Context);
    }

    public void Clear()
    {
        var src = _source;
        Task.Run(async () =>
        {
            await Task.Delay(50, CancellationToken.None).ConfigureAwait(false);
            if (!src.IsCancellationRequested)
                Previewer.Invoke(Previewer.Hide);
        }, src.Token);
        ShowSet.RemoveAll();
        Cry.Stop();
    }

    // 追求额外的预览信息
    public static List<string> H2PreviewText(List<string> result, PKM pk, LegalityAnalysis la)
    {
        // 如果没有设定，则返回原本的文本
        if (!Settings.HoverSlotShowH2Text)
            return result;

        // 生成信息
        result.Add("======= 致诚之心定制信息 =======");
        result.Add($"相遇时间：{pk.MetDate}");
        result.Add($"训练家：{pk.OT_Name}  {pk.DisplayTID}({pk.DisplaySID})");
        result.Add($"PID：{pk.PID.ToString("X8")}  EC：{pk.EncryptionConstant.ToString("X8")}");
        

        // 显示追踪码
        switch ((GameVersion)pk.Version)
        {
            case GameVersion.SH or GameVersion.SW or GameVersion.SWSH:
                {
                    result.Add($"HomeTracker：{((PK8)pk).Tracker}");
                }
                break;
            case GameVersion.BD or GameVersion.SP or GameVersion.BDSP:
                {
                    result.Add($"HomeTracker：{((PB8)pk).Tracker}");
                }
                break;
            case GameVersion.PLA:
                {
                    result.Add($"HomeTracker：{((PA8)pk).Tracker}");
                }
                break;
            case GameVersion.SL or GameVersion.VL or GameVersion.SV:
                {
                    result.Add($"HomeTracker：{((PK9)pk).Tracker}");
                }
                break;
        }

        // 显示合法报告
        bool verbose = Control.ModifierKeys == Keys.Control;
        var report = la.Report(verbose);
        result.Add($"======= 合法报告 =======\n{report}");

        return result;
    }


    public static string GetPreviewText(PKM pk, LegalityAnalysis la)
    {
        var text = ShowdownParsing.GetLocalizedPreviewText(pk, Main.Settings.Startup.Language);
        if (!Main.Settings.Hover.HoverSlotShowEncounter)
            return text;
        var result = new List<string> { text, string.Empty };
        LegalityFormatting.AddEncounterInfo(la, result);

        // 追加致诚之心定制内容
        result = H2PreviewText(result, pk, la);

        return string.Join(Environment.NewLine, result);
    }

    private static string GetPreviewText(IEncounterInfo enc)
    {
        var lines = enc.GetTextLines(GameInfo.Strings);
        return string.Join(Environment.NewLine, lines);
    }
}
