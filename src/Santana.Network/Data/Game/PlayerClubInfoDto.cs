
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class PlayerClubInfoDto
  {
    public PlayerClubInfoDto()
    {
      Id = 0;
      Type = "";
      Name = "";
    }

    
    public uint Id { get; set; }

    
    public string Name { get; set; }

    
    public string Type { get; set; }
  }
}
