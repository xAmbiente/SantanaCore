
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
  [Dto]
  public class ScoreDto
  {
    public ScoreDto()
    {
      Killer = 0;
      Target = 0;
    }

    public ScoreDto(LongPeerId killer, LongPeerId target, AttackAttribute weapon)
    {
      Killer = killer;
      Target = target;
      Weapon = weapon;
    }

     public LongPeerId Killer { get; set; }

    [Wire(Kind.Int)] 
    public AttackAttribute Weapon { get; set; }

     public LongPeerId Target { get; set; }

     public byte Unk { get; set; }
  }

  [Dto]
  public class Score2Dto
  {
    public Score2Dto()
    {
      Killer = 0;
      Target = 0;
    }

    public Score2Dto(LongPeerId killer, LongPeerId target, AttackAttribute weapon)
    {
      Killer = killer;
      Target = target;
      Weapon = weapon;
    }

     public LongPeerId Killer { get; set; }

    [Wire(Kind.Int)] 
    public AttackAttribute Weapon { get; set; }

     public LongPeerId Target { get; set; }
  }
}
