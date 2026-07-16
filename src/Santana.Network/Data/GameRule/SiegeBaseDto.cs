using System;
using System.Collections.Generic;
using System.Text;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
    [Dto]
    public class SiegeBaseDto
    {
        public Team Owner = Team.Neutral;
        public List<ulong> Drops = new List<ulong>();
    }
}
