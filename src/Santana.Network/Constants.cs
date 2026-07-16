namespace Santana.Network
{
    public enum AuthLoginResult : byte
    {
        OK = 0,
        WrongIdorPw = 1,
        Banned = 2,
        Failed = 3,
        Failed2 = 7
    }
    public enum GameLoginResult : uint
    {
        OK = 0,
        ServerFull = 1,
        TerminateOtherConnection = 2,
        ExistingExit = 3,
        ServerFull2 = 4,
        WrongVersion = 5,
        ChooseNickname = 6,
        FailedAndRestart = 7,
        SessionTimeout = 8,
        AuthenticationFailed = 9
    }
    public enum ServerResult : uint
    {
        ServerError = 0,
        CannotFindRoom = 1,
        AlreadyPlaying = 2,
        NonExistingChannel = 3,
        ChannelLimitReached = 4,
        ChannelEnter = 5,
        ServerLimitReached = 6,
        PlayerLimitReached = 7,
        RoomChangingRules = 8,
        ChannelLeave = 9,
        PlayerNotFound = 10,
        CreateCharacterFailed = 11,
        DeleteCharacterFailed = 12,
        SelectCharacterFailed = 13,
        CreateNicknameSuccess = 14,
        NicknameUnavailable = 15,
        NicknameAvailable = 16,
        PasswordError = 17,
        WelcomeToS4World = 18,
        IPLocked = 19,
        ForbiddenToConnectFor5Min = 20,
        UserAlreadyExist = 21,
        DBError = 22,
        CreateCharacterFailed2 = 23,
        JoinChannelFailed = 24,
        RequiredChannelLicense = 25,
        WearingUnusableItem = 26,
        CannotSellWearingItem = 27,
        CantEnterRoom = 29,
        ImpossibleToEnterRoom = 30,
        CantReadClanInfo = 31,
        TaskCompensationError = 32,
        FailedToRequestTask = 33,
        ItemExchangeFailed = 34,
        ItemExchangeFailed2 = 35,
        SelectGameMode = 36,
        EnteringFailed = 38,
        CantEnterBecauseKicked = 44,
        CantEnterBecauseVoteKick = 45,
        InternetSlow = 47,
        NetworkCheck = 48,
        CantKickThisPlayer = 49,
        WeaponNotAllowed = 50,
        FailedToCreateRoom = 56
    }
    public enum ChannelInfoRequest : byte
    {
        RoomList = 3,
        RoomList2 = 4,
        ChannelList = 5
    }
    public enum ChangeTeamResult : byte
    {
        Full = 0,
        AlreadyReady = 1
    }
    public enum ClubState
    {
        NotJoined,
        AwaitingAccept,
        Joined
    }
    public enum NewClubRank
    {
        Master = 1,
        Commander,
        DeputyCommander,
        Military,
        Member,
    }
    public enum ClubRank
    {
        None = 0,
        Master = 1,
        CoMaster,
        Staff,
        Member,
        Normal,
        BadManner,
        Aclass,
        Bclass,
        Cclass
    }
    public enum ClubMemberPresenceState : uint
    {
        Offline = 0,
        Online = 1,
        Playing = 2,
        a = 3
    }
    public enum ClubArea : uint
    {
        Europe = 1,
        Germany = 2,
        France = 3,
        Spain = 4,
        Italy = 5,
        Russia = 6,
        England = 7,
        NorthAmerica = 8,
        LatinAmerica = 9
    }
    public enum ClubActivity : uint
    {
        Fellowship = 1,
        ClanBattle = 2,
        Meeting = 3,
        KnowHowTransfer = 4
    }
    public enum ClubClass : uint
    {
        A = 0,
        B = 1,
        C = 2
    }
    public enum ClubSearchType : uint
    {
        None = 0,
        Name = 1,
        OwnerName = 3
    }
    public enum ClubSearchSort : uint
    {
        None = 0,
        Members = 1,
        Class = 2,
        Points = 3
    }
    public enum ClubSearchSortType : byte
    {
        Descending = 0,
        Ascending = 1
    }
    public enum ClubCreateResult : uint
    {
        Success = 0,
        Failed = 1,
        AlreadyInClan = 2,
        PendingJoinRequest = 3,
        NameAlreadyExists = 4,
        LevelRequirementNotMet = 6
    }
    public enum ClubNameCheckResult : uint
    {
        Available = 0,
        NotInAClan = 1,
        NotAvailable = 2,
        CannotBeUsed = 3,
        BreaksRules = 4,
        TooLong = 5,
        TooShort = 6,
    }
    public enum ClubCloseResult : uint
    {
        Success = 0,
        NotInClan = 1,
        MasterRequired = 2,
        ClanNotEmpty = 3,
        Four = 4
    }
    public enum ClubJoinResult : uint
    {
        Registered = 0,
        Joined = 1,
        NotInClan = 2,
        Failed = 3,
        AlreadyRegistered = 4,
        CantRegister = 5,
        ClubFull = 6,
        LevelRequirementNotMet = 7,
        WaitingForApproval = 8,
    }
    public enum ClubLeaveResult : uint
    {
        Success = 0,
        NotInClan = 1,
        Failed = 2
    }
    public enum ClubCommand : uint
    {
        Accept = 1,
        Decline = 2,
        Kick = 3,
        Ban = 4,
        Unban = 5
    }
    public enum ClubCommandResult : uint
    {
        Success = 0,
        NotInClan = 1,
        MemberNotFound = 2,
        MemberNotFound2 = 3,
        PermissionDenied = 4,
        NoMemberSelected = 5
    }
    public enum ClubLeaveReason : uint
    {
        Leave = 1,
        Kick = 2,
        Ban = 3
    }
    public enum VoteKickMessage
    {
        Ok = 1,
        InsufficientMoney = 2,
        CurrentlyRunning = 3,
        NotEnoughtPlayerToVote = 4,
        PlayerNotInRoom = 5,
        CantKickGM = 6
    }
    public enum VoteKickDialogStyle
    {
        KickDialogWithSeconds = 1,
        KickDialogWithoutSeconds = 2,
        KickDialogCancelled = 3,
        KickDialogPlayerKicked = 4,
        KickDialogNotKicked = 5
    }
    public enum VoteKickResult
    {
        Ok,
        DontHaveARight,
        DontMeetRequirements
    }
    public enum ClanMasterChangeMessage
    {
        NotInClan = 1,
        NoMatchFound = 2,
        CannotFindMember = 3,
        MemberNotHaveAuthority = 4,
        EntrustMasterAlreadyExist = 5,
        Ok = 6
    }
    public enum ClubMessage
    {
        Ok,
        NotInClan,
        PlayerCannotBeFound,
        YouCannotRegisterMoreThanAClan,
        AlreadyInClan,
        CannotInviteToClan
    }
    public enum ClubJoinMessage
    {
        RegistrationDone,
        SuccessToEnter,
        NotInAnyClan,
        NoMatchFound,
        YouCannotRegister,
        WithdrawMessage,
        MaxPlayerLimit,
        RegistrationNotAvailable,
        WaitingClanApprovation
    }
    public enum FriendState
    {
        NotInList,
        Requesting,
        InList,
        RequestDialog,
        Unk,
        RegisteredInMyList
    }
    public enum FriendAction : uint
    {
        Add,
        Remove,
        Update,
        Decline
    }
    public enum FriendResult
    {
        Ok,
        UserNotExist,
    }
    public enum ShopInfoTypeEnum
    {
        off = 0,
        on,
        @new,
        limited,
        hot,
        @event,
    }
}
