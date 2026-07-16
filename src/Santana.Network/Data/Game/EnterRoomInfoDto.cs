using System.Net;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class EnterRoomInfoDto
  {
    public EnterRoomInfoDto()
    {
      RelayEndPoint = new IPEndPoint(0, 0);
    }

     public uint RoomId { get; set; }

     public Santana.GameRule GameRule { get; set; }

     public byte MapID { get; set; }

     public byte PlayerLimit { get; set; }

     public GameTimeState TimeState { get; set; }

     public GameState State { get; set; }

     public uint TimeLimit { get; set; }

     public int Unk1 { get; set; }

     public uint TimeSync { get; set; }

     public uint ScoreLimit { get; set; }

     public byte Unk2 { get; set; }

    [EndpointStr] 
    public IPEndPoint RelayEndPoint { get; set; }

     public byte hasSpectator { get; set; }

     public int Spectator { get; set; }

     public byte FMBURNMode { get; set; }

     public ulong Unk4 { get; set; }
  }
}
