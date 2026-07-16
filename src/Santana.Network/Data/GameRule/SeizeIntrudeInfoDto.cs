using System;
using System.Collections.Generic;
using System.Text;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
    [Dto]
    public class SeizeIntrudeInfoDto
    {
         public ushort Base { get; set; }
         public byte BaseOwner { get; set; }
         public ushort Percentage { get; set; }
         public ushort PercentageGoal { get; set; }
         public ushort Unk1 { get; set; }
    }
}
