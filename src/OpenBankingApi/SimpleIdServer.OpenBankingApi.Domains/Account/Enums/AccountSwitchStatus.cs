namespace SimpleIdServer.OpenBankingApi.Domains.Account.Enums
{
    public class AccountSwitchStatus : Enumeration
    {
        /// <summary>
        /// Indicator to show that the account has not been switched to another ASPSP.
        /// </summary>
        public static AccountSwitchStatus NotSwitched = new AccountSwitchStatus(0, "NotSwitched");
        /// <summary>
        /// Indicator to show that the account has been switched and the switching process is complete.
        /// </summary>
        public static AccountSwitchStatus SwitchCompleted = new AccountSwitchStatus(1, "SwitchCompleted");

        private AccountSwitchStatus(int id, string name) : base(id, name) { }
    }
}
