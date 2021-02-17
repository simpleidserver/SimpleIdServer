namespace SimpleIdServer.OpenBankingApi.Domains.Account
{
    public class AccountTypes : Enumeration
    {
        public static AccountTypes Business = new AccountTypes(0, "Business");
        public static AccountTypes Personnal = new AccountTypes(1, "Personnal");

        private AccountTypes(int id, string name) : base(id, name)
        {
        }
    }
}
