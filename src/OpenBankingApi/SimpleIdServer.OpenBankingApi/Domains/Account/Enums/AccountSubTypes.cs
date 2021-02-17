namespace SimpleIdServer.OpenBankingApi.Domains.Account.Enums
{
    public class AccountSubTypes : Enumeration
    {
        public static AccountSubTypes ChargeCard = new AccountSubTypes(0, "ChargeCard");
        public static AccountSubTypes CreditCard = new AccountSubTypes(1, "CreditCard");
        public static AccountSubTypes CurrentAccount = new AccountSubTypes(2, "CurrentAccount");
        public static AccountSubTypes EMoney = new AccountSubTypes(3, "EMoney");
        public static AccountSubTypes Loan = new AccountSubTypes(4, "Loan");
        public static AccountSubTypes Mortgage = new AccountSubTypes(5, "Mortgage");
        public static AccountSubTypes PrePaidCard = new AccountSubTypes(6, "PrePaidCard");
        public static AccountSubTypes Savings = new AccountSubTypes(7, "Savings");

        private AccountSubTypes(int id, string name) : base(id, name) { }
    }
}
