using System;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
    [Dto]
    public class SeizeUpdateInfoDto
    {
        public SeizeUpdateInfoDto()
        {
            Base = 0;
            IsCaptured = 0;
            BaseOwner = 0;
            CurrentCaptureTeam = 0;
            Percentage = 0;
            PercentageGoal = 50000;
            CapturePoints = 0;
            AssistPoints = 0;
            Unk1 = 0;
            Player = 0;

            Assists = Array.Empty<ulong>();
        }

         public ushort Base { get; set; }

         public uint IsCaptured { get; set; }

         public byte BaseOwner { get; set; }

         public byte CurrentCaptureTeam { get; set; }

         public ushort Percentage { get; set; }

         public ushort PercentageGoal { get; set; }

         public uint CapturePoints { get; set; }

         public uint AssistPoints { get; set; }

         public int Unk1 { get; set; }

         public ulong Player { get; set; }

        public ulong[] Assists { get; set; }

         public ushort Points { get; set; }
    }
}
