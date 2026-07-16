using System;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
    [Dto]
    public class PlayerAccountInfoDto
    {
        public PlayerAccountInfoDto()
        {
            Nickname = "";
            DMStats = new DMStatsDto();
            TDStats = new TDStatsDto();
            ChaserStats = new ChaserStatsDto();
            BRStats = new BRStatsDto();
            CPTStats = new CPTStatsDto();
            SiegeStats = new SiegeStatsDto();
            ArenaStats = new ArenaStatsDto();
        }

         public uint TotalMatches { get; set; }

         public uint Unk1 { get; set; }

         public uint MatchesWon { get; set; }

         public uint MatchesLost { get; set; }

         public uint MatchesLost2 { get; set; }

         public uint Unk2 { get; set; }

        [Sec] 
        public TimeSpan GameTime { get; set; }

         public bool IsGM { get; set; }

         public uint Unk3 { get; set; }

         public byte Level { get; set; }

         public byte Unk4 { get; set; }

         public int TotalExp { get; set; }

         public uint CombiMasterExp { get; set; }

         public uint PEN { get; set; }

         public uint TutorialState { get; set; }

        public string Nickname { get; set; }

         public uint Unk5 { get; set; }

         public DMStatsDto DMStats { get; set; }

         public TDStatsDto TDStats { get; set; }

         public ChaserStatsDto ChaserStats { get; set; }

         public BRStatsDto BRStats { get; set; }

         public CPTStatsDto CPTStats { get; set; }

         public SiegeStatsDto SiegeStats { get; set; }
         public ArenaStatsDto ArenaStats { get; set; }

         public uint Unk6 { get; set; }

         public uint Unk7 { get; set; }

         public uint Unk8 { get; set; }

         public uint Unk9 { get; set; }

         public uint Unk10 { get; set; }
    }
}
