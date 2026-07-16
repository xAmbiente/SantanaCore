
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class TaskDto
  {
     public uint Id { get; set; }

     public byte Unk { get; set; }

     public ushort Progress { get; set; }

     public MissionRewardType RewardType { get; set; }

     public uint Reward { get; set; }
  }
}
