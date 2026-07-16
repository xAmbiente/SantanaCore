
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
  [Dto]
  public class PlayerInfoDto
  {
    public PlayerInfoDto()
    {
      Info = new PlayerInfoShortDto();
      Location = new PlayerLocationDto();
    }

    public PlayerInfoDto(PlayerInfoShortDto info, PlayerLocationDto location)
    {
      Info = info;
      Location = location;

      if (info == null)
      {
        Info = new PlayerInfoShortDto();
      }

      if (location == null)
      {
        Location = new PlayerLocationDto();
      }
    }

     public PlayerInfoShortDto Info { get; set; }

     public PlayerLocationDto Location { get; set; }
  }
}
