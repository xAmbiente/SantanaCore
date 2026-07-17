using System;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
    [Dto]
    public class UserDataDto
    {
        public UserDataDto()
        {
            TDScore = new TDUserDataScoreDto();
            DMScore = new DMUserDataScoreDto();
            ChaserSurvivability = new ChaserUserDataScoreDto();
            CaptainScore = new CPTUserDataScoreDto();

            TDStats = new TDUserDataDto();
            DMStats = new DMUserDataDto();
            ChaserStats = new ChaserUserDataDto();
            BattleRoyalStats = new BRUserDataDto();
            CaptainStats = new CPTUserDataDto();
            SiegeStats = new SiegeUserDataDto();
            ArenaStats = new ArenaUserDataDto();

            Clothes = Array.Empty<UserDataItemDto>();
            Weapons = Array.Empty<UserDataItemDto>();
            Skills = Array.Empty<UserDataItemDto>();
        }

        public string Nickname { get; set; }

        public ulong AccountId { get; set; }

        public int TotalExp { get; set; }

        public int ClanId { get; set; }

        public string ClanIcon { get; set; }

        public string ClanName { get; set; }

        public uint Level { get; set; }

        [Sec]
        public TimeSpan GameTime { get; set; }

        public int TotalMatches { get; set; }

        public int MatchesWon { get; set; }

        public int MatchesLost { get; set; }

        public int Unk4 { get; set; }

        public int Unk5 { get; set; }

        public int Unk6 { get; set; }

        public float Unk7 { get; set; }

        public TDUserDataScoreDto TDScore { get; set; }

        public DMUserDataScoreDto DMScore { get; set; }

        public ChaserUserDataScoreDto ChaserSurvivability { get; set; }

        public float BRScore { get; set; }

        public CPTUserDataScoreDto CaptainScore { get; set; }

        public float SiegeScore { get; set; }

        public float ArenaScore { get; set; }

        public TDUserDataDto TDStats { get; set; }

        public DMUserDataDto DMStats { get; set; }

        public ChaserUserDataDto ChaserStats { get; set; }

        public BRUserDataDto BattleRoyalStats { get; set; }

        public CPTUserDataDto CaptainStats { get; set; }

        public SiegeUserDataDto SiegeStats { get; set; }

        public ArenaUserDataDto ArenaStats { get; set; }

        public CharacterGender Gender { get; set; }

        public UserDataItemDto[] Clothes { get; set; }

        public UserDataItemDto[] Weapons { get; set; }

        public UserDataItemDto[] Skills { get; set; }
    }
}
