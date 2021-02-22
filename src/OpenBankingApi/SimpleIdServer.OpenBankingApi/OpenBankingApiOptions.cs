namespace SimpleIdServer.OpenBankingApi
{
    public class OpenBankingApiOptions
    {
        public OpenBankingApiOptions()
        {
            TakeSnapshotEveryNbRevisions = 5;
            WaitInterval = 1000;
            CertificateAuthenticationScheme = "Certificate";
            AccountsScope = "accounts";
            OpenBankingApiConsentClaimName = "openbanking_intent_id";
        }

        public int TakeSnapshotEveryNbRevisions { get; set; }
        public int WaitInterval { get; set; }
        public string CertificateAuthenticationScheme { get; set; }
        /// <summary>
        /// Scope used to access to the accounts.
        /// </summary>
        public string AccountsScope { get; set; }
        /// <summary>
        /// Name of the consent defined by openbankingapi.
        /// </summary>
        public string OpenBankingApiConsentClaimName { get; set; }
    }
}
