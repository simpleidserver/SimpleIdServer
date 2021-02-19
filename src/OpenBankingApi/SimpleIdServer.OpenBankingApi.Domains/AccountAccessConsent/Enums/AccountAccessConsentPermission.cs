namespace SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums
{
    public class AccountAccessConsentPermission : Enumeration
    {
        public static AccountAccessConsentPermission ReadAccountsBasic = new AccountAccessConsentPermission(0, "ReadAccountsBasic");
        public static AccountAccessConsentPermission ReadAccountsDetail = new AccountAccessConsentPermission(1, "ReadAccountsDetail");
        public static AccountAccessConsentPermission ReadBalances = new AccountAccessConsentPermission(2, "ReadBalances");
        public static AccountAccessConsentPermission ReadBeneficiariesBasic = new AccountAccessConsentPermission(3, "ReadBeneficiariesBasic");
        public static AccountAccessConsentPermission ReadBeneficiariesDetail = new AccountAccessConsentPermission(4, "ReadBeneficiariesDetail");
        public static AccountAccessConsentPermission ReadDirectDebits = new AccountAccessConsentPermission(5, "ReadDirectDebits");
        public static AccountAccessConsentPermission ReadOffers = new AccountAccessConsentPermission(6, "ReadOffers");
        public static AccountAccessConsentPermission ReadPAN = new AccountAccessConsentPermission(7, "ReadPAN");
        public static AccountAccessConsentPermission ReadParty = new AccountAccessConsentPermission(8, "ReadParty");
        public static AccountAccessConsentPermission ReadPartyPSU = new AccountAccessConsentPermission(9, "ReadPartyPSU");
        public static AccountAccessConsentPermission ReadProducts = new AccountAccessConsentPermission(10, "ReadProducts");
        public static AccountAccessConsentPermission ReadScheduledPaymentsBasic = new AccountAccessConsentPermission(11, "ReadScheduledPaymentsBasic");
        public static AccountAccessConsentPermission ReadScheduledPaymentsDetail = new AccountAccessConsentPermission(12, "ReadScheduledPaymentsDetail");
        public static AccountAccessConsentPermission ReadStandingOrdersBasic = new AccountAccessConsentPermission(13, "ReadStandingOrdersBasic");
        public static AccountAccessConsentPermission ReadStandingOrdersDetail = new AccountAccessConsentPermission(14, "ReadStandingOrdersDetail");
        public static AccountAccessConsentPermission ReadStatementsBasic = new AccountAccessConsentPermission(15, "ReadStatementsBasic");
        public static AccountAccessConsentPermission ReadStatementsDetail = new AccountAccessConsentPermission(16, "ReadStatementsDetail");
        public static AccountAccessConsentPermission ReadTransactionsBasic = new AccountAccessConsentPermission(17, "ReadTransactionsBasic");
        public static AccountAccessConsentPermission ReadTransactionsCredits = new AccountAccessConsentPermission(18, "ReadTransactionsCredits");
        public static AccountAccessConsentPermission ReadTransactionsDebits = new AccountAccessConsentPermission(19, "ReadTransactionsDebits");
        public static AccountAccessConsentPermission ReadTransactionsDetail = new AccountAccessConsentPermission(20, "ReadTransactionsDetail");

        private AccountAccessConsentPermission(int id, string name) : base(id, name)
        {
        }
    }
}
