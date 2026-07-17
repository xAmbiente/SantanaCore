using System.Drawing;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class ChannelInfoDto
  {
    public ChannelInfoDto()
    {
      Name = "";
      Rank = "FREE";
      Description = "";
      Color = Color.Black;
      minKD = 0.0f;
      maxKD = -1.0f;
    }

     public ushort Id { get; set; }

     public ushort PlayersOnline { get; set; }

     public ushort PlayerLimit { get; set; }

    [IntBool]
    public bool IsClanChannel { get; set; }

    public string Name { get; set; }

    public string Rank { get; set; }

    public string Description { get; set; }

    public Color Color { get; set; }

     public uint MinLevel { get; set; }

     public uint MaxLevel { get; set; }

     public float minKD { get; set; }

     public float maxKD { get; set; }
  }
}
