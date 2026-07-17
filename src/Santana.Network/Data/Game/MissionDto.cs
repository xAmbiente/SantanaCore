using System;
using System.Collections.Generic;
using System.Text;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
    [Dto]
    public class MissionSettingDto
    {

        public int GameMode { get; set; }

        public int Map { get; set; }

        public int MaxProgress { get; set; }

        public int Progress { get; set; }

        public int Unk5 { get; set; }

        public int Unk6 { get; set; }

        public int Unk7 { get; set; }

        public int Unk8 { get; set; }

        public int Unk9 { get; set; }

    }

    [Dto]
    public class MissionItemsDto
    {

        public int Slot { get; set; }

        public int Reward { get; set; }

        public int Reward2 { get; set; }

        public int Reward3 { get; set; }

        public int Unk5 { get; set; }
    }
}
