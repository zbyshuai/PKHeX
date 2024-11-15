using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class BlueberryQuestRecord9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Data)
{
    public uint Quests1  { get => ReadUInt32LittleEndian(Data[0x4..]); set => WriteUInt32LittleEndian(Data[0x4..], value); }
    public uint Quests2  { get => ReadUInt32LittleEndian(Data[0x50..]); set => WriteUInt32LittleEndian(Data[0x50..], value); }
    public uint Quests3  { get => ReadUInt32LittleEndian(Data[0x9c..]); set => WriteUInt32LittleEndian(Data[0x9c..], value); }
    public uint QuestsDoneSolo  { get => ReadUInt32LittleEndian(Data[0x188..]); set => WriteUInt32LittleEndian(Data[0x188..], value); }
    public uint QuestsDoneGroup { get => ReadUInt32LittleEndian(Data[0x18C..]); set => WriteUInt32LittleEndian(Data[0x18C..], value); }
}
