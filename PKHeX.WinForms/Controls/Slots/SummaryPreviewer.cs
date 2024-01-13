using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using H2Core.PkHex.ToString;
using static System.Net.Mime.MediaTypeNames;
using static H2Core.PkHex.ToString.LangDict;
using System.IO;
using System.Reflection;


namespace PKHeX.WinForms.Controls;

public sealed class SummaryPreviewer
{
    private readonly ToolTip ShowSet = new() { InitialDelay = 200, IsBalloon = false, AutoPopDelay = 32_767 };
    private readonly CryPlayer Cry = new();
    private readonly PokePreview Previewer = new();
    private CancellationTokenSource _source = new();
    private static HoverSettings Settings => Main.Settings.Hover;
    public void Show(Control pb, PKM pk, SaveFile SAV)
    {
        if (pk.Species == 0)
        {
            Clear();
            return;
        }

        if (Settings.HoverSlotShowPreview && Control.ModifierKeys != Keys.Alt)
            UpdatePreview(pb, pk, SAV);
        else if (Settings.HoverSlotShowText)
            ShowSet.SetToolTip(pb, GetPreviewText(pk, new LegalityAnalysis(pk), SAV));
        if (Settings.HoverSlotPlayCry)
            Cry.PlayCry(pk, pk.Context);
    }

    private void UpdatePreview(Control pb, PKM pk, SaveFile SAV)
    {
        _source.Cancel();
        _source = new();
        UpdatePreviewPosition(new());
        Previewer.Populate(pk, SAV);
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
            if (!Previewer.IsHandleCreated)
                return; // not shown ever

            // Give a little bit of fade-out delay
            await Task.Delay(50, CancellationToken.None).ConfigureAwait(false);
            if (!src.IsCancellationRequested)
                Previewer.Invoke(Previewer.Hide);
        }, src.Token);
        ShowSet.RemoveAll();
        Cry.Stop();
    }

    // 追求额外的预览信息
    
    public static List<string> H2PreviewText(List<string> result, PKM pk, LegalityAnalysis la, SaveFile SAV)
    {
        // 如果没有设定，则返回原本的文本
        if (!Settings.HoverSlotShowH2Text)
            return result;

        // 传入参数
        result.Add("======= 致诚之心定制信息 =======");
        string lang = $"{Main.Settings.Startup.Language}";
        string version = PKMToString.Version(pk.Version, lang);
        result.Add($"来源版本：{version}  相遇时间：{pk.MetDate}");
        result.Add($"训练家：{pk.OT_Name}  {pk.DisplayTID}({pk.DisplaySID})");
        result.Add($"PID：{pk.PID.ToString("X8")}  EC：{pk.EncryptionConstant.ToString("X8")}");


        // 显示追踪码
        switch (SAV.Version)
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

        // 显示合法报告(仅显示合法报告)
        bool verbose = Control.ModifierKeys == Keys.Control;
        var report = la.Report(true);

        if (verbose)
        {
            var resultNew = new List<string>();
            string valid = report.Contains("非法") ? "**非法!**" : report.Contains("可疑") ? "**存在可疑!**" : report;
            resultNew.Add($"======= 合法报告 =======\n{valid}");
            return resultNew;
        }
        else
        {
            string valid = report.Contains("非法") ? "**非法!**" : report.Contains("可疑") ? "**存在可疑!**" : "合法";
            result.Add($"======= 合法信息 =======\n{valid}");
        }

        return result;
    }


    public static string GetPreviewText(PKM pk, LegalityAnalysis la, SaveFile SAV)
    {
        // 预设输出结果
        var result = new List<string>();

        // ps信息
        var text = ShowdownParsing.GetLocalizedPreviewText(pk, Main.Settings.Startup.Language);

        if (!Main.Settings.Hover.HoverSlotShowEncounter)
            return text;

        result.Add(text);

        LegalityFormatting.AddEncounterInfo(la, result);

        // 追加致诚之心定制内容
        result = H2PreviewText(result, pk, la, SAV);

        return string.Join(Environment.NewLine, result);
    }

    private static string GetPreviewText(IEncounterInfo enc)
    {
        var lines = enc.GetTextLines(GameInfo.Strings);
        return string.Join(Environment.NewLine, lines);
    }
}
