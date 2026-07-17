
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
  [Dto]
  public class ScoreAssistDto
  {
    public ScoreAssistDto()
    {
      Killer = 0;
      Assist = 0;
      Target = 0;
    }

    public ScoreAssistDto(LongPeerId killer, LongPeerId assist, LongPeerId target, AttackAttribute weapon)
    {
      Killer = killer;
      Assist = assist;
      Target = target;
      Weapon = weapon;
    }

     public LongPeerId Killer { get; set; }

     public LongPeerId Assist { get; set; }

    [Wire(Kind.Int)]
    public AttackAttribute Weapon { get; set; }

     public LongPeerId Target { get; set; }

     public byte Unk { get; set; }
  }

  [Dto]
  public class ScoreAssist2Dto
  {
    public ScoreAssist2Dto()
    {
      Killer = 0;
      Assist = 0;
      Target = 0;
    }

    public ScoreAssist2Dto(LongPeerId killer, LongPeerId assist, LongPeerId target, AttackAttribute weapon)
    {
      Killer = killer;
      Assist = assist;
      Target = target;
      Weapon = weapon;
    }

     public LongPeerId Killer { get; set; }

     public LongPeerId Assist { get; set; }

    [Wire(Kind.Int)]
    public AttackAttribute Weapon { get; set; }

     public LongPeerId Target { get; set; }
  }
}
