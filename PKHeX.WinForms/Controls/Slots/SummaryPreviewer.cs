using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Reflection;

using H2Core.PKMString;


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
            ShowSet.SetToolTip(pb, GetPreviewText(enc, Settings.HoverSlotShowEncounterVerbose));
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
        if (!Main.Settings.H2.HoverSlotShowH2Text)
            return result;
        // 实例化
        var h2strings = new DodoString(pk);
        var strings = GameInfo.GetStrings("zh-Hans");

        // 传入参数
        result.Add("======= 致诚之心定制信息 =======");
        string lang = $"{Main.Settings.Startup.Language}";
        string version = strings.gamelist[(int)pk.Version];;
        result.Add($"来源版本：{version}  相遇时间：{pk.MetDate} 相遇地点：{h2strings.MetLocation}");
        result.Add($"训练家：{pk.OriginalTrainerName}  {pk.DisplayTID}({pk.DisplaySID})");
        result.Add($"PID：{pk.PID.ToString("X8")}  EC：{pk.EncryptionConstant.ToString("X8")}");
        
        // 显示追踪码
        result.Add($"HomeTracker：{h2strings.Tracker}");

        // 添加体型内容
        string Scale = $"体型: -";
        Scale = (pk is PK9 pk9) ? $"体型:{pk9.Scale} 身高:{pk9.HeightScalar} 体重:{pk9.WeightScalar}" : Scale; 
        Scale = (pk is PA8 pa8) ? $"体型:{pa8.Scale}  身高:{pa8.HeightScalar} 体重:{pa8.WeightScalar}" : Scale; 
        Scale = (pk is PK8 pk8) ? $"身高:{pk8.HeightScalar} 体重:{pk8.WeightScalar}" : Scale; 
        Scale = (pk is PB8 pb8) ? $"身高:{pb8.HeightScalar} 体重:{pb8.WeightScalar}" : Scale; 
        result.Add(Scale);

        // 添加证章信息
        if (h2strings.Marks != "")
            result.Add($"证章：{h2strings.Marks}");

        // 添加回忆技能信息
        string RelearnMovesString = string.Join(",", h2strings.RelearnMoves);
        if (RelearnMovesString.Length > 0)
            result.Add($"可回忆招式：{RelearnMovesString}");

        // 将原有的Nature字段改成原始属性
        result = result.Select(x => {
            if (x.Contains("Nature"))
                return x.Replace("Nature", $"（原始性格:{strings.Natures[(int)pk.Nature]}）");
            else
                return x;
        }).ToList();

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

    private static string GetPreviewText(IEncounterInfo enc, bool verbose = false)
    {
        var lines = enc.GetTextLines(GameInfo.Strings, verbose);
        return string.Join(Environment.NewLine, lines);
    }
}
