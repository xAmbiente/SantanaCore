
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
  [Dto]
  public class NoteGiftDto
  {
    public NoteGiftDto()
    {
      Text = "";
    }

     public uint Unk1 { get; set; }

     public uint Unk2 { get; set; }

    
    public string Text { get; set; }

     public ItemNumber ItemNumber { get; set; }

     public ItemPriceType PriceType { get; set; }

     public ItemPeriodType PeriodType { get; set; }

     public ushort Period { get; set; }

     public byte Color { get; set; }

     public int Flags { get; set; }

     public byte Mode { get; set; }

    public byte Unk5
    {
      get => Color;
      set => Color = value;
    }

    public int Unk6
    {
      get => Flags;
      set => Flags = value;
    }

    public byte Unk7
    {
      get => Mode;
      set => Mode = value;
    }

    public uint Effect
    {
      get => (uint)(Flags & 0x00FFFFFF);
      set => Flags = (Flags & unchecked((int)0xFF000000)) | ((int)value & 0x00FFFFFF);
    }

    public byte SpecialState
    {
      get => (byte)((Flags >> 24) & 0xFF);
      set => Flags = (Flags & 0x00FFFFFF) | (value << 24);
    }
  }
}
