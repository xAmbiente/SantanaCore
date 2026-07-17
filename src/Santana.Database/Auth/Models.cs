using Santana.Database.Game;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Santana.Database.Auth
{
    [Table("accounts")]
    public class AccountDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public byte SecurityLevel { get; set; }
        public string AuthToken { get; set; }
        public string newToken { get; set; }
        public string OldLoginToken { get; set; }
        public string LoginToken { get; set; }
        public string LastLogin { get; set; }
        public string Hwid { get; set; }
        public string Verify { get; set; }
        public string Color { get; set; }
        public string ChatColor { get; set; }
        public bool IsConnected { get; set; }
        public string email { get; set; }
        public string password_token { get; set; }
        public int p_time { get; set; }
        public int Status { get; set; }

        public IList<BanDto> Bans { get; set; } = new List<BanDto>();
        public IList<LoginHistoryDto> LoginHistory { get; set; } = new List<LoginHistoryDto>();
        public IList<NicknameHistoryDto> NicknameHistory { get; set; } = new List<NicknameHistoryDto>();
        public IList<IPBanDto> IPBans { get; set; } = new List<IPBanDto>();
        public IList<LoginAPIDto> LoginAPI { get; set; } = new List<LoginAPIDto>();

    }

    [Table("hwid_bans")]
    public class HwidBanDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Hwid { get; set; }
    }

    [Table("bans")]
    public class BanDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Account))]
        public int AccountId { get; set; }

        public AccountDto Account { get; set; }

        public long Date { get; set; }
        public long? Duration { get; set; }
        public string Reason { get; set; }
        public string Log { get; set; }
    }

    [Table("login_history")]
    public class LoginHistoryDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Account))]
        public int AccountId { get; set; }

        public AccountDto Account { get; set; }

        public long Date { get; set; }
        public string IP { get; set; }
    }

    [Table("auth_history")]
    public class AuthHistoryDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Account))]
        public int AccountId { get; set; }

        public AccountDto Account { get; set; }

        public long Date { get; set; }
        public string Hwid { get; set; }
    }

    [Table("nickname_history")]
    public class NicknameHistoryDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Account))] public int AccountId { get; set; }

        public AccountDto Account { get; set; }

        public string OldName { get; set; }
        public string NewNickname { get; set; }

        public long? ExpireDate { get; set; }
    }

    [Table("register_limit")]
    public class RegisterLimitDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Hwid { get; set; }
    }

    [Table("login_api")]
    public class LoginAPIDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string API { get; set; }
    }

    [Table("ip_ban")]
    public class IPBanDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Player))]
        public ulong PlayerId { get; set; }
        public PlayerDto Player { get; set; }

        public string IP { get; set; }

        public long Date { get; set; }
    }

}
