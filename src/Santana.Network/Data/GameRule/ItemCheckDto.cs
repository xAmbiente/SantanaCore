using System;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
  [Dto]
  public class ItemCheckDto
  {
    public ItemCheckDto()
    {
      ItemNumber = 0;
      Effects = Array.Empty<uint>();
    }

     public ulong ItemId { get; set; }

     public ItemNumber ItemNumber { get; set; }

     public uint Color { get; set; }

    public uint[] Effects { get; set; }

     public float Power { get; set; }

     public float MoveSpeedRate { get; set; }
  }
}
