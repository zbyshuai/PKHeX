using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace PKHeX.Core;

class H2Namer
{
    BoxExportSettings settings;

    private int[] BallItemIDs  = [ 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 492, 493, 494, 495, 496, 497, 498, 499, 576, 851, 1785, 1710, 1711, 1712, 1713, 1746, 1747, 1748, 1749, 1750, 1771 ];
    
    private string LanguageID = "zh";
    private bool isSetRandomName (PKM pk) => pk.OriginalTrainerFriendship == int.Parse(settings.RandomNameRule) || pk.HandlingTrainerFriendship == int.Parse(settings.RandomNameRule) && settings.UseRandomName == true;
    private string GetBallInfo(PKM pk) => GameInfo.GetStrings(LanguageID).balllist[ pk.Ball ];
    private string GetAbilityInfo(PKM pk) => GameInfo.GetStrings(LanguageID).Ability[ pk.Ability ];
    private string GetNatureInfo(PKM pk) => GameInfo.GetStrings(LanguageID).Natures[ (int)pk.Nature];

    private string GetForm(PKM pk)
    {
        // 定义参数
        ushort species = pk.Species;
        byte form = pk.Form;
        var strings = GameInfo.GetStrings(LanguageID);
        
        // 获取Form文字
        string[] formList = FormConverter.GetFormList(species, strings.Types, strings.forms, GameInfo.GenderSymbolASCII, pk.Generation == 9 ? EntityContext.Gen9 : EntityContext.Gen4);

        // 如果不存在形态，则返回空内容
        if (formList.Length == 0)
            return "";
        
        // 如果存在形态，则提取形态
        // formList[0] = "";

        if (form >= formList.Length)
            form = (byte)(formList.Length - 1);

        return formList[form].Contains('-') ? formList[form] : formList[form] == "" ? "" : $"-{formList[form]}";
    }

    private string GetFormArgument(PKM pk)
    {
        string[] formArgumentList = FormConverter.GetFormArgumentStrings(pk.Species);
        uint formArgumentIndex = ((PK9)pk).FormArgument;
        string formArgument = formArgumentIndex > formArgumentList.Length ? "" : formArgumentList[formArgumentIndex];
        return formArgument == "" ? "" : $"-{formArgument}";
    }

    private string GetSpecieNameInfo(PKM pk)
    {
        // 此处可优化成形态名
        string Species = GameInfo.GetStrings(LanguageID).Species[ pk.Species ];
        string form = GetForm(pk);
        string formArgument = GetFormArgument(pk);

        string Shiny = pk.IsShiny ? "★ " : "";
        string Egg = pk.IsEgg ? "蛋" : "";
        string SpeciesInfo = Shiny + Species + form + formArgument + Egg;

        // 如果是随机类型，有昵称且为随机名称
        if ( isSetRandomName(pk) )
            SpeciesInfo += "(R)";

        return SpeciesInfo;
    }
    
    private string GetHeldItemInfo(PKM pk)
    {
        string Item = Properties.Resources.text_Items_zh.Split("\n")[ pk.HeldItem ];
        return $"{Item}";
    }

    private string GetIvsInfo(PKM pk)
    {   
        int Vs = 0;
        string IVSpecial = "";
        
        // 计算V的数量
        if (pk.IV_ATK == 31)
            Vs += 1;
        if (pk.IV_DEF == 31)
            Vs += 1;
        if (pk.IV_HP == 31)
            Vs += 1;
        if (pk.IV_SPA == 31)
            Vs += 1;
        if (pk.IV_SPD == 31)
            Vs += 1;
        if (pk.IV_SPE == 31)
            Vs += 1;

        // 计算特殊值
        if (pk.IV_ATK == 0)
            IVSpecial += "0A";
        if (pk.IV_SPE == 0)
            IVSpecial += "0S";
        
        // 根据规则生成名字
        string IVs = Vs < 4 ? "" : IVSpecial == "" ? $"{Vs}V" : $"{Vs}V" + IVSpecial;
        return IVs;
    }

    private string GetMark(PKM pk)
    {
        Dictionary<string, string> ribbonDictionary = new Dictionary<string, string>
        {
            { "RibbonMarkLunchtime", "正午之证" },
            { "RibbonMarkSleepyTime", "午夜之证" },
            { "RibbonMarkDusk", "黄昏之证" },
            { "RibbonMarkDawn", "拂晓之证" },
            { "RibbonMarkCloudy", "阴云之证" },
            { "RibbonMarkRainy", "降雨之证" },
            { "RibbonMarkStormy", "落雷之证" },
            { "RibbonMarkSnowy", "降雪之证" },
            { "RibbonMarkBlizzard", "暴雪之证" },
            { "RibbonMarkDry", "干燥之证" },
            { "RibbonMarkSandstorm", "沙尘之证" },
            { "RibbonMarkMisty", "浓雾之证" },
            { "RibbonMarkDestiny", "命运之证" },
            { "RibbonMarkFishing", "上钩之证" },
            { "RibbonMarkCurry", "咖喱之证" },
            { "RibbonMarkUncommon", "偶遇之证" },
            { "RibbonMarkRare", "未知之证" },
            { "RibbonMarkRowdy", "淘气之证" },
            { "RibbonMarkAbsentMinded", "无虑之证" },
            { "RibbonMarkJittery", "紧张之证" },
            { "RibbonMarkExcited", "期待之证" },
            { "RibbonMarkCharismatic", "领袖之证" },
            { "RibbonMarkCalmness", "冷静之证" },
            { "RibbonMarkIntense", "热情之证" },
            { "RibbonMarkZonedOut", "疏忽之证" },
            { "RibbonMarkJoyful", "幸福之证" },
            { "RibbonMarkAngry", "愤怒之证" },
            { "RibbonMarkSmiley", "微笑之证" },
            { "RibbonMarkTeary", "悲伤之证" },
            { "RibbonMarkUpbeat", "爽快之证" },
            { "RibbonMarkPeeved", "激动之证" },
            { "RibbonMarkIntellectual", "理性之证" },
            { "RibbonMarkFerocious", "本能之证" },
            { "RibbonMarkCrafty", "狡猾之证" },
            { "RibbonMarkScowling", "凶悍之证" },
            { "RibbonMarkKindly", "优雅之证" },
            { "RibbonMarkFlustered", "动摇之证" },
            { "RibbonMarkPumpedUp", "昂扬之证" },
            { "RibbonMarkZeroEnergy", "倦怠之证" },
            { "RibbonMarkPrideful", "自信之证" },
            { "RibbonMarkUnsure", "自卑之证" },
            { "RibbonMarkHumble", "木讷之证" },
            { "RibbonMarkThorny", "不纯之证" },
            { "RibbonMarkVigor", "活力之证" },
            { "RibbonMarkSlump", "不振之证" },
            { "RibbonHisui", "洗翠奖章" },
            { "RibbonTwinklingStar", "闪亮之星奖章" },
            { "RibbonChampionPaldea", "帕底亚冠军奖章" },
            { "RibbonMarkJumbo", "大个子之证" },
            { "RibbonMarkMini", "小不点之证" },
            { "RibbonMarkItemfinder", "捡拾之证" },
            { "RibbonMarkPartner", "搭档之证" },
            { "RibbonMarkGourmand", "美食之证" },
            { "RibbonOnceInALifetime", "千载难逢奖章" },
            { "RibbonMarkAlpha", "头目之证" },
            { "RibbonMarkMightiest", "最强之证" },
            { "RibbonMarkTitan", "宝主之证" },
            { "RibbonPartner", "同伴奖章" }
        };

        foreach (var rib in ribbonDictionary)
        {
            // 通过字符串获取属性的值
            string propertyName = rib.Key;
            PropertyInfo? propertyInfo = typeof(PK9).GetProperty(propertyName);

            // 判断是否存在Ribbon
            bool? hasRibbon = false;
            if (propertyInfo != null)
                // 获取属性的值
                hasRibbon = (bool?)propertyInfo.GetValue(((PK9)pk));
            else
                hasRibbon = false;

            // 存在Ribbon则返回Mark
            if (hasRibbon == true)
                return rib.Value;
            // 不存在则继续匹配
            else
                continue;
        }
        
        // 如果全部没有匹配到，则返回空
        return "";
        
    }

    public H2Namer(BoxExportSettings settings)
    {
        this.settings = settings;
    }

    private string WithNickName(PKM pk)
    {
        return $"{pk.Species} - {pk.Nickname} - {Util.Rand.Rand32()}";
    }
    private string WithSpecificName(PKM pk)
    {
        return $"{pk.Species} - {this.settings.SpecificName} - {Util.Rand.Rand32()}";
    }
    private string WithPKMName(PKM pk)
    {
        List<string> SpeciesInfoList = new();
        
        // 必定使用宝可梦的名称
        SpeciesInfoList.Add( this.GetSpecieNameInfo(pk) );
        // 判断使用球种信息
        if (this.settings.BallInfo == H2BallInfo.Use)
            SpeciesInfoList.Add( this.GetBallInfo(pk) );
        // 判断使用性格信息
        if (this.settings.NatureInfo == H2NatureInfo.Use)
            SpeciesInfoList.Add( this.GetNatureInfo(pk) );
        // 判断使用IV信息
        if (this.settings.IVsInfo == H2IVsInfo.Use)
            SpeciesInfoList.Add( this.GetIvsInfo(pk) );
        // 判断使用特性信息
        if (this.settings.AbilityInfo == H2AbilityInfo.Use)
            SpeciesInfoList.Add( this.GetAbilityInfo(pk) );        
        // 判断是否使用证章信息
        if (this.settings.RibbonInfo == H2RibbonInfo.Use)
            SpeciesInfoList.Add( this.GetMark(pk) );            
        // 判断使用道具信息
        if (this.settings.HeldItemInfo == H2HeldItemInfo.Use)
            SpeciesInfoList.Add( this.GetHeldItemInfo(pk) );
        
        // 删除内容为空的项目
        SpeciesInfoList.RemoveAll(item => string.IsNullOrEmpty(item));
        
        // 生成信息
        string SpeciesInfo = string.Join(settings.SepStyle, SpeciesInfoList);
        
        return $"{pk.Species} - {SpeciesInfo} - {Util.Rand.Rand32()}";
    }
    

    public string Generate(PKM pk)
    {   
        string slotName;
        // 如果使用特定昵称
        if (this.settings.SpecificName != "")
            slotName = this.WithSpecificName(pk);
        // 如果有昵称
        else if (pk.IsNicknamed == true && settings.UseNickName == H2UseNickName.Use && !pk.IsEgg)
            slotName = this.WithNickName(pk);
        // 如果是正常的宝可梦.
        else
            slotName = this.WithPKMName(pk);
        try
        {
            return Util.CleanFileName(slotName);
        }
        catch { return "Name Error"; }
    }
}


    

    


