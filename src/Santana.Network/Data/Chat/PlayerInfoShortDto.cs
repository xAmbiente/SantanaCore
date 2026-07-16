
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
  [Dto]
  public class PlayerInfoShortDto
  {
    public PlayerInfoShortDto(ulong accountId, string nickname, uint totalExp, bool isGm)
    {
      AccountId = accountId;
      Nickname = nickname;
      TotalExp = totalExp;
      IsGM = isGm;
    }

    public PlayerInfoShortDto()
    {
      Nickname = "";
      IsGM = false;
    }

     public ulong AccountId { get; set; }

    
    public string Nickname { get; set; }

     public int Unk { get; set; }

     public uint TotalExp { get; set; }

     public bool IsGM { get; set; }
  }
}
