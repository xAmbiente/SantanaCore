
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
  [Dto]
  public class ArenaUserDataDto
  {
    
    public float WinRate { get; set; }
    
    public float KdRate { get; set; }
    
    public float KdPercent { get; set; }
    
    public float DoubleKillRate { get; set; }
    
    public float TripleKillRate { get; set; }
    
    public uint ShortestKillTime { get; set; }
    
    public uint LeaderSelected { get; set; }
    
    public uint LeaderKills { get; set; }
  }
}
