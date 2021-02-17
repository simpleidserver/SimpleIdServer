namespace SimpleIdServer.OpenBankingApi.Domains.Account.Enums
{
    public class AccountStatus : Enumeration
    {
        public static AccountStatus Deleted = new AccountStatus(0, "Deleted");
        public static AccountStatus Disabled = new AccountStatus(1, "Disabled");
        public static AccountStatus Enabled = new AccountStatus(2, "Enabled");
        public static AccountStatus Pending = new AccountStatus(3, "Pending");
        public static AccountStatus ProForma = new AccountStatus(4, "ProForma");

        private AccountStatus(int status, string name) : base(status, name) { }
    }
}
