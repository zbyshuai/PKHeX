using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 1 <see cref="SaveFile"/> object.
/// </summary>
public sealed class SAV1 : SaveFile, ILangDeviantSave, IEventFlagArray, IEventWorkArray<byte>, IBoxDetailName, IDaycareStorage
{
    protected internal override string ShortSummary => $"{OT} ({Version}) - {PlayTimeString}";
    public override string Extension => ".sav";
    public bool IsVirtualConsole => State.Exportable && Metadata.FileName is { } s && s.StartsWith("sav", StringComparison.Ordinal) && s.Contains(".dat"); // default to GB-Era for non-exportable

    public int SaveRevision => Japanese ? 0 : 1;
    public string SaveRevisionString => (Japanese ? "J" : "U") + (IsVirtualConsole ? "VC" : "GB");
    public bool Japanese { get; }
    public bool Korean => false;
    public override int Language => Japanese ? 1 : -1;

    public override PersonalTable1 Personal { get; }

    public override ReadOnlySpan<ushort> HeldItems => [];

    public override IReadOnlyList<string> PKMExtensions => Array.FindAll(PKM.Extensions, f =>
    {
        int gen = f[^1] - 0x30;
        return gen is 1 or 2;
    });

    public SAV1(GameVersion version = GameVersion.RBY, bool japanese = false) : base(SaveUtil.SIZE_G1RAW)
    {
        Version = version;
        Japanese = japanese;
        Offsets = Japanese ? SAV1Offsets.JPN : SAV1Offsets.INT;
        Personal = version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
        Initialize(version);
        ClearBoxes();
    }

    public SAV1(byte[] data, GameVersion versionOverride = GameVersion.Any) : base(data)
    {
        Japanese = SaveUtil.GetIsG1SAVJ(Data);
        Offsets = Japanese ? SAV1Offsets.JPN : SAV1Offsets.INT;

        Version = versionOverride != GameVersion.Any ? versionOverride : SaveUtil.GetIsG1SAV(data);
        Personal = Version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
        if (Version == GameVersion.Invalid)
            return;

        Initialize(versionOverride);
    }

    private void Initialize(GameVersion versionOverride)
    {
        // see if RBY can be differentiated
        if (versionOverride is not (GameVersion.RB or GameVersion.YW))
        {
            if (Starter != 0)
                Version = Yellow ? GameVersion.YW : GameVersion.RB;
            else
                Version = Data[Offsets.PikaFriendship] != 0 ? GameVersion.YW : GameVersion.RB;
        }

        Box = Data.Length;
        Array.Resize(ref Data, Data.Length + SIZE_RESERVED);
        Party = GetPartyOffset(0);

        // Stash boxes after the save file's end.
        int stored = SIZE_STOREDBOX;
        var capacity = Japanese ? PokeListType.StoredJP : PokeListType.Stored;
        int baseDest = Data.Length - SIZE_RESERVED;
        for (int box = 0; box < BoxCount; box++)
        {
            int boxOfs = GetBoxRawDataOffset(box);
            UnpackBox(boxOfs, baseDest, stored, box, capacity);
        }

        if ((uint)CurrentBox < BoxCount)
        {
            // overwrite previously cached box data.
            UnpackBox(Offsets.CurrentBox, baseDest, stored, CurrentBox, capacity);
        }

        var party = Data.AsSpan(Offsets.Party, SIZE_STOREDPARTY).ToArray();
        var partyPL = new PokeList1(party, PokeListType.Party, Japanese);
        for (int i = 0; i < partyPL.Pokemon.Length; i++)
        {
            var dest = GetPartyOffset(i);
            var pkDat = i < partyPL.Count
                ? new PokeList1(partyPL[i]).Write()
                : new byte[PokeList1.GetDataLength(PokeListType.Single, Japanese)];
            pkDat.CopyTo(Data, dest);
        }

        Span<byte> rawDC = stackalloc byte[0x38];
        Data.AsSpan(Offsets.Daycare, rawDC.Length).CopyTo(rawDC);
        byte[] TempDaycare = new byte[PokeList1.GetDataLength(PokeListType.Single, Japanese)];
        TempDaycare[0] = rawDC[0];

        rawDC.Slice(1, StringLength).CopyTo(TempDaycare.AsSpan(2 + 1 + PokeCrypto.SIZE_1PARTY + StringLength));
        rawDC.Slice(1 + StringLength, StringLength).CopyTo(TempDaycare.AsSpan(2 + 1 + PokeCrypto.SIZE_1PARTY));
        rawDC.Slice(1 + (2 * StringLength), PokeCrypto.SIZE_1STORED).CopyTo(TempDaycare.AsSpan(2 + 1));

        PokeList1 daycareList = new(TempDaycare, PokeListType.Single, Japanese);
        daycareList.Write().CopyTo(Data, GetPartyOffset(7));
        DaycareOffset = GetPartyOffset(7);

        // Enable Pokedex editing
    }

    private int DaycareOffset = -1;
    public override bool HasPokeDex => true;

    private void UnpackBox(int srcOfs, int destOfs, int boxSize, int boxIndex, PokeListType boxCapacity)
    {
        var boxData = Data.AsSpan(srcOfs, boxSize).ToArray();
        var boxDest = destOfs + (boxIndex * SIZE_BOX);
        var boxPL = new PokeList1(boxData, boxCapacity, Japanese);
        for (int i = 0; i < boxPL.Pokemon.Length; i++)
        {
            var slotOfs = boxDest + (i * SIZE_STORED);
            var slotData = Data.AsSpan(slotOfs, SIZE_STORED);
            if (i < boxPL.Count)
                new PokeList1(boxPL[i]).Write().CopyTo(slotData);
            else
                slotData.Clear();
        }
    }

    private void PackBox(int boxDest, int boxIndex, PokeListType boxCapacity)
    {
        var boxPL = new PokeList1(boxCapacity, Japanese);
        int slot = 0;
        for (int i = 0; i < boxPL.Pokemon.Length; i++)
        {
            var slotOfs = boxDest + (i * SIZE_STORED);
            var slotData = Data.AsSpan(slotOfs, SIZE_STORED);
            PK1 pk = GetPKM(slotData.ToArray());
            if (pk.Species > 0)
                boxPL[slot++] = pk;
        }

        // copy to box location
        var boxData = boxPL.Write();
        int boxSrc = GetBoxRawDataOffset(boxIndex);
        SetData(Data.AsSpan(boxSrc), boxData);

        // copy to active loc if current box
        if (boxIndex == CurrentBox)
            SetData(Data.AsSpan(Offsets.CurrentBox), boxData);
    }

    private const int SIZE_RESERVED = 0x8000; // unpacked box data
    private readonly SAV1Offsets Offsets;

    // Event Flags
    public int EventFlagCount => 0xA00; // 320 * 8
    public bool GetEventFlag(int flagNumber)
    {
        if ((uint)flagNumber >= EventFlagCount)
            throw new ArgumentOutOfRangeException(nameof(flagNumber), $"Event Flag to get ({flagNumber}) is greater than max ({EventFlagCount}).");
        return GetFlag(Offsets.EventFlag + (flagNumber >> 3), flagNumber & 7);
    }

    public void SetEventFlag(int flagNumber, bool value)
    {
        if ((uint)flagNumber >= EventFlagCount)
            throw new ArgumentOutOfRangeException(nameof(flagNumber), $"Event Flag to set ({flagNumber}) is greater than max ({EventFlagCount}).");
        SetFlag(Offsets.EventFlag + (flagNumber >> 3), flagNumber & 7, value);
    }

    // Event Work
    public int EventWorkCount => 0x100;
    public byte GetWork(int index) => Data[Offsets.EventWork + index];
    public void SetWork(int index, byte value) => Data[Offsets.EventWork + index] = value;

    protected override byte[] GetFinalData()
    {
        var capacity = Japanese ? PokeListType.StoredJP : PokeListType.Stored;
        for (int box = 0; box < BoxCount; box++)
        {
            var boxOfs = GetBoxOffset(box);
            PackBox(boxOfs, box, capacity);
        }

        var partyPL = new PokeList1(PokeListType.Party, Japanese);
        int pSlot = 0;
        for (int i = 0; i < 6; i++)
        {
            var ofs = GetPartyOffset(i);
            var data = Data.AsSpan(ofs, SIZE_STORED).ToArray();
            PK1 partyPK = GetPKM(data);
            if (partyPK.Species > 0)
                partyPL[pSlot++] = partyPK;
        }
        partyPL.Write().CopyTo(Data, Offsets.Party);

        // Daycare is read-only, but in case it ever becomes editable, copy it back in.
        Span<byte> rawDC = Data.AsSpan(GetDaycareSlotOffset(index: 0), SIZE_STORED);
        Span<byte> dc = stackalloc byte[1 + (2 * StringLength) + PokeCrypto.SIZE_1STORED];
        dc[0] = IsDaycareOccupied(0) ? (byte)1 : (byte)0;
        rawDC.Slice(2 + 1 + PokeCrypto.SIZE_1PARTY + StringLength, StringLength).CopyTo(dc[1..]);
        rawDC.Slice(2 + 1 + PokeCrypto.SIZE_1PARTY, StringLength).CopyTo(dc[(1 + StringLength)..]);
        rawDC.Slice(2 + 1, PokeCrypto.SIZE_1STORED).CopyTo(dc[(1 + (2 * StringLength))..]);
        dc.CopyTo(Data.AsSpan(Offsets.Daycare));

        SetChecksums();
        return Data[..^SIZE_RESERVED];
    }

    private int GetBoxRawDataOffset(int box)
    {
        if (box < BoxCount / 2)
            return 0x4000 + (box * SIZE_STOREDBOX);
        return 0x6000 + ((box - (BoxCount / 2)) * SIZE_STOREDBOX);
    }

    // Configuration
    protected override SAV1 CloneInternal() => new(Write(), Version);

    protected override int SIZE_STORED => Japanese ? PokeCrypto.SIZE_1JLIST : PokeCrypto.SIZE_1ULIST;
    protected override int SIZE_PARTY => Japanese ? PokeCrypto.SIZE_1JLIST : PokeCrypto.SIZE_1ULIST;
    private int SIZE_BOX => BoxSlotCount*SIZE_STORED;
    private int SIZE_STOREDBOX => PokeList1.GetDataLength(Japanese ? PokeListType.StoredJP : PokeListType.Stored, Japanese);
    private int SIZE_STOREDPARTY => PokeList1.GetDataLength(PokeListType.Party, Japanese);

    public override PK1 BlankPKM => new(Japanese);
    public override Type PKMType => typeof(PK1);

    public override ushort MaxMoveID => Legal.MaxMoveID_1;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_1;
    public override int MaxAbilityID => Legal.MaxAbilityID_1;
    public override int MaxItemID => Legal.MaxItemID_1;
    public override int MaxBallID => 0; // unused
    public override GameVersion MaxGameID => GameVersion.RBY; // unused
    public override int MaxMoney => 999999;
    public override int MaxCoins => 9999;

    public override int BoxCount => Japanese ? 8 : 12;
    public override int MaxEV => EffortValues.Max12;
    public override int MaxIV => 15;
    public override byte Generation => 1;
    public override EntityContext Context => EntityContext.Gen1;
    public override int MaxStringLengthOT => Japanese ? 5 : 7;
    public override int MaxStringLengthNickname => Japanese ? 5 : 10;
    public override int BoxSlotCount => Japanese ? 30 : 20;

    public override bool HasParty => true;
    private int StringLength => Japanese ? GBPKML.StringLengthJapanese : GBPKML.StringLengthNotJapan;

    public override bool IsPKMPresent(ReadOnlySpan<byte> data) => EntityDetection.IsPresentGB(data);

    // Checksums
    protected override void SetChecksums() => Data[Offsets.ChecksumOfs] = GetRBYChecksum(Offsets.OT, Offsets.ChecksumOfs);
    public override bool ChecksumsValid => Data[Offsets.ChecksumOfs] == GetRBYChecksum(Offsets.OT, Offsets.ChecksumOfs);
    public override string ChecksumInfo => ChecksumsValid ? "Checksum valid." : "Checksum invalid";

    private byte GetRBYChecksum(int start, int end)
    {
        var span = Data.AsSpan(start, end - start);
        byte result = 0;
        foreach (ref var b in span)
            result += b;
        return (byte)~result;
    }

    // Trainer Info
    public override GameVersion Version { get; set; }

    public override string OT
    {
        get => GetString(Data.AsSpan(Offsets.OT, MaxStringLengthOT));
        set => SetString(Data.AsSpan(Offsets.OT, MaxStringLengthOT + 1), value, MaxStringLengthOT, StringConverterOption.ClearZero);
    }

    public Span<byte> OriginalTrainerTrash { get => Data.AsSpan(Offsets.OT, StringLength); set { if (value.Length == StringLength) value.CopyTo(Data.AsSpan(Offsets.OT)); } }

    public override byte Gender
    {
        get => 0;
        set { }
    }

    public override uint ID32
    {
        get => TID16;
        set => TID16 = (ushort)value;
    }

    public override ushort TID16
    {
        get => ReadUInt16BigEndian(Data.AsSpan(Offsets.TID16));
        set => WriteUInt16BigEndian(Data.AsSpan(Offsets.TID16), value);
    }

    public override ushort SID16 { get => 0; set { } }

    public string Rival
    {
        get => GetString(Data.AsSpan(Offsets.Rival, MaxStringLengthOT));
        set => SetString(Data.AsSpan(Offsets.Rival, MaxStringLengthOT), value, MaxStringLengthOT, StringConverterOption.Clear50);
    }

    public Span<byte> Rival_Trash { get => Data.AsSpan(Offsets.Rival, StringLength); set { if (value.Length == StringLength) value.CopyTo(Data.AsSpan(Offsets.Rival)); } }

    public byte RivalStarter { get => Data[Offsets.Starter - 2]; set => Data[Offsets.Starter - 2] = value; }
    public bool Yellow => Starter == 0x54; // Pikachu
    public byte Starter { get => Data[Offsets.Starter]; set => Data[Offsets.Starter] = value; }

    public ref byte WramD72E => ref Data[Offsets.Starter + 0x17]; // offset relative to player starter

    // bit0 of d72e
    public bool IsSilphLaprasReceived { get => (WramD72E & 1) != 0; set => WramD72E = (byte)((WramD72E & 0xFE) | (value ? 1 : 0)); }

    public byte PikaFriendship
    {
        get => Data[Offsets.PikaFriendship];
        set => Data[Offsets.PikaFriendship] = value;
    }

    public int PikaBeachScore
    {
        get => BinaryCodedDecimal.ToInt32LE(Data.AsSpan(Offsets.PikaBeachScore, 2));
        set => BinaryCodedDecimal.WriteBytesLE(Data.AsSpan(Offsets.PikaBeachScore, 2), Math.Min(9999, value));
    }

    public override string PlayTimeString => !PlayedMaximum ? base.PlayTimeString : $"{base.PlayTimeString} {Checksums.CRC16_CCITT(Data):X4}";

    public override int PlayedHours
    {
        get => Data[Offsets.PlayTime + 0];
        set
        {
            if (value >= byte.MaxValue) // Set 255:00:00.00 and flag
            {
                PlayedMaximum = true;
                value = byte.MaxValue;
                PlayedMinutes = PlayedSeconds = PlayedFrames = 0;
            }
            Data[Offsets.PlayTime + 0] = (byte) value;
        }
    }

    public bool PlayedMaximum
    {
        get => Data[Offsets.PlayTime + 1] != 0;
        set => Data[Offsets.PlayTime + 1] = value ? (byte)1 : (byte)0;
    }

    public override int PlayedMinutes
    {
        get => Data[Offsets.PlayTime + 2];
        set => Data[Offsets.PlayTime + 2] = (byte)value;
    }

    public override int PlayedSeconds
    {
        get => Data[Offsets.PlayTime + 3];
        set => Data[Offsets.PlayTime + 3] = (byte)value;
    }

    public int PlayedFrames
    {
        get => Data[Offsets.PlayTime + 4];
        set => Data[Offsets.PlayTime + 4] = (byte)value;
    }

    public int Badges
    {
        get => Data[Offsets.Badges];
        set => Data[Offsets.Badges] = (byte)value;
    }

    private byte Options
    {
        get => Data[Offsets.Options];
        set => Data[Offsets.Options] = value;
    }

    public bool BattleEffects
    {
        get => (Options & 0x80) == 0;
        set => Options = (byte)((Options & 0x7F) | (value ? 0 : 0x80));
    }

    public bool BattleStyleSwitch
    {
        get => (Options & 0x40) == 0;
        set => Options = (byte)((Options & 0xBF) | (value ? 0 : 0x40));
    }

    public int Sound
    {
        get => (Options & 0x30) >> 4;
        set => Options = (byte)((Options & 0xCF) | ((value & 3) << 4));
    }

    public int TextSpeed
    {
        get => Options & 0x7;
        set => Options = (byte)((Options & 0xF8) | (value & 7));
    }

    // yellow only
    public byte GBPrinterBrightness { get => Data[Offsets.PrinterBrightness]; set => Data[Offsets.PrinterBrightness] = value; }

    public override uint Money
    {
        get => (uint)BinaryCodedDecimal.ToInt32BE(Data.AsSpan(Offsets.Money, 3));
        set
        {
            value = (uint)Math.Min(value, MaxMoney);
            BinaryCodedDecimal.WriteBytesBE(Data.AsSpan(Offsets.Money, 3), (int)value);
        }
    }

    public uint Coin
    {
        get => (uint)BinaryCodedDecimal.ToInt32BE(Data.AsSpan(Offsets.Coin, 2));
        set
        {
            value = (ushort)Math.Min(value, MaxCoins);
            BinaryCodedDecimal.WriteBytesBE(Data.AsSpan(Offsets.Coin, 2), (int)value);
        }
    }

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            InventoryPouch[] pouch =
            [
                new InventoryPouchGB(InventoryType.Items, ItemStorage1.Instance, 99, Offsets.Items, 20),
                new InventoryPouchGB(InventoryType.PCItems, ItemStorage1.Instance, 99, Offsets.PCItems, 50),
            ];
            return pouch.LoadAll(Data);
        }
        set => value.SaveAll(Data);
    }

    public int DaycareSlotCount => 1;

    public Memory<byte> GetDaycareSlot(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(index, 0, nameof(index));
        return Data.AsMemory(DaycareOffset, SIZE_STORED);
    }

    private int GetDaycareSlotOffset(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(index, 0, nameof(index));
        return DaycareOffset;
    }

    public bool IsDaycareOccupied(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(index, 0, nameof(index));
        return Data[Offsets.Daycare] == 0x01;
    }

    public void SetDaycareOccupied(int index, bool occupied)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(index, 0, nameof(index));
        Data[Offsets.Daycare] = (byte)(occupied ? 0x01 : 0x00);
    }

    // Storage
    public override int PartyCount
    {
        get => Data[Offsets.Party];
        protected set => Data[Offsets.Party] = (byte)value;
    }

    public override int GetBoxOffset(int box) => Data.Length - SIZE_RESERVED + (box * SIZE_BOX);
    public override int GetPartyOffset(int slot) => Data.Length - SIZE_RESERVED + (BoxCount * SIZE_BOX) + (slot * SIZE_STORED);

    public override int CurrentBox
    {
        get => Data[Offsets.CurrentBoxIndex] & 0x7F;
        set => Data[Offsets.CurrentBoxIndex] = (byte)((Data[Offsets.CurrentBoxIndex] & 0x80) | (value & 0x7F));
    }

    public bool CurrentBoxChanged
    {
        get => (Data[Offsets.CurrentBoxIndex] & 0x80) != 0;
        set => Data[Offsets.CurrentBoxIndex] = (byte)((Data[Offsets.CurrentBoxIndex] & 0x7F) | (byte)(value ? 0x80 : 0));
    }

    public string GetBoxName(int box)
    {
        if (Japanese)
            return BoxDetailNameExtensions.GetDefaultBoxNameJapanese(box);
        return BoxDetailNameExtensions.GetDefaultBoxName(box);
    }

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        // Don't allow for custom box names
    }

    protected override PK1 GetPKM(byte[] data)
    {
        if (data.Length == SIZE_STORED)
            return new PokeList1(data, PokeListType.Single, Japanese)[0];
        return new(data);
    }

    protected override byte[] DecryptPKM(byte[] data)
    {
        return data;
    }

    // Pokédex
    protected override void SetDex(PKM pk)
    {
        ushort species = pk.Species;
        if (!CanSetDex(species))
            return;

        SetCaught(pk.Species, true);
        SetSeen(pk.Species, true);
    }

    private bool CanSetDex(ushort species)
    {
        if (species == 0)
            return false;
        if (species > MaxSpeciesID)
            return false;
        if (Version == GameVersion.Invalid)
            return false;
        return true;
    }

    public override bool GetSeen(ushort species) => GetDexFlag(Offsets.DexSeen, species);
    public override bool GetCaught(ushort species) => GetDexFlag(Offsets.DexCaught, species);
    public override void SetSeen(ushort species, bool seen) => SetDexFlag(Offsets.DexSeen, species, seen);
    public override void SetCaught(ushort species, bool caught) => SetDexFlag(Offsets.DexCaught, species, caught);

    private bool GetDexFlag(int region, ushort species)
    {
        int bit = species - 1;
        int ofs = bit >> 3;
        return GetFlag(region + ofs, bit & 7);
    }

    private void SetDexFlag(int region, ushort species, bool value)
    {
        int bit = species - 1;
        int ofs = bit >> 3;
        SetFlag(region + ofs, bit & 7, value);
    }

    public override void WriteSlotFormatStored(PKM pk, Span<byte> data)
    {
        // pk that have never been boxed have yet to save the 'current level' for box indication
        // set this value at this time
        ((PK1)pk).Stat_LevelBox = pk.CurrentLevel;
        base.WriteSlotFormatStored(pk, data);
    }

    public override void WriteBoxSlot(PKM pk, Span<byte> data)
    {
        // pk that have never been boxed have yet to save the 'current level' for box indication
        // set this value at this time
        ((PK1)pk).Stat_LevelBox = pk.CurrentLevel;
        base.WriteBoxSlot(pk, data);
    }

    private const int SpawnFlagCount = 0xF0;

    public bool[] EventSpawnFlags
    {
        get
        {
            // RB uses 0xE4 (0xE8) flags, Yellow uses 0xF0 flags. Just grab 0xF0
            bool[] data = new bool[SpawnFlagCount];
            for (int i = 0; i < data.Length; i++)
                data[i] = GetFlag(Offsets.ObjectSpawnFlags + (i >> 3), i & 7);
            return data;
        }
        set
        {
            if (value.Length != SpawnFlagCount)
                return;
            for (int i = 0; i < value.Length; i++)
                SetFlag(Offsets.ObjectSpawnFlags + (i >> 3), i & 7, value[i]);
        }
    }

    public override string GetString(ReadOnlySpan<byte> data) => StringConverter12.GetString(data, Japanese);

    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
    {
        return StringConverter12.SetString(destBuffer, value, maxLength, Japanese, option);
    }
}
