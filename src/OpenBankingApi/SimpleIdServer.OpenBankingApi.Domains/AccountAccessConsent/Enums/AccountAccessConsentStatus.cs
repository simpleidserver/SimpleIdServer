namespace SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums
{
    public class AccountAccessConsentStatus : Enumeration
    {
        /// <summary>
        /// The account access consent has been rejected.
        /// </summary>
        public static AccountAccessConsentStatus Rejected = new AccountAccessConsentStatus(0, "Rejected");
        /// <summary>
        /// The account access consent is awaiting authorisation.
        /// </summary>
        public static AccountAccessConsentStatus AwaitingAuthorisation = new AccountAccessConsentStatus(1, "AwaitingAuthorisation");
        /// <summary>
        /// The account access consent has been successfully authorised.
        /// </summary>
        public static AccountAccessConsentStatus Authorised = new AccountAccessConsentStatus(2, "Authorised");
        /// <summary>
        /// The account access consent has been revoked via the ASPSP interface.
        /// </summary>
        public static AccountAccessConsentStatus Revoked = new AccountAccessConsentStatus(3, "Rejected");

        private AccountAccessConsentStatus(int id, string name) : base(id, name)
        {
        }
    }
}
