
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
    [Dto]
    public class ArenaSyncDto
    {
        
        public uint Status { get; set; }

        
        public ulong AccountId { get; set; }

        public ArenaSyncDto()
        {
        }

        public ArenaSyncDto(uint status, ulong accountId)
        {
            Status = status;
            AccountId = accountId;
        }
    }
}
