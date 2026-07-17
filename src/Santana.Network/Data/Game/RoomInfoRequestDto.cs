using System;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
    [Dto]
    public class RoomInfoRequestDto
    {
        public RoomInfoRequestDto()
        {
            MasterName = "";
            Unk3 = "";
        }

        public string MasterName { get; set; }

         public uint MasterLevel { get; set; }

        public string Unk3 { get; set; }

         public bool IsMasterInClan { get; set; }

         public uint ScoreLimit { get; set; }

        [Sec]
        public TimeSpan TimeLimit { get; set; }

         public GameState State { get; set; }

         public int Unk8 { get; set; }

         public int Unk9 { get; set; }

         public int Unk10 { get; set; }

         public int Unk11 { get; set; }

         public int Unk12 { get; set; }

         public int Unk13 { get; set; }

         public int Unk14 { get; set; }
    }
}
