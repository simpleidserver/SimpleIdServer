namespace SimpleIdServer.OpenBankingApi
{
    public class OpenBankingApiOptions
    {
        public OpenBankingApiOptions()
        {
            TakeSnapshotEveryNbRevisions = 5;
            WaitInterval = 1000;
            CertificateAuthenticationScheme = "Certificate";
            JwtBearerAuthenticationScheme = "Bearer";
            AccountsScope = "accounts";
            OpenBankingApiConsentClaimName = "openbanking_intent_id";
            IsRequestRequired = true;
        }

        public int TakeSnapshotEveryNbRevisions { get; set; }
        public int WaitInterval { get; set; }
        public string CertificateAuthenticationScheme { get; set; }
        public string JwtBearerAuthenticationScheme { get; set; }
        /// <summary>
        /// Scope used to access to the accounts.
        /// </summary>
        public string AccountsScope { get; set; }
        /// <summary>
        /// Name of the consent defined by openbankingapi.
        /// </summary>
        public string OpenBankingApiConsentClaimName { get; set; }
        /// <summary>
        /// Throw an exception if request or request_uri is not passed in the request.
        /// </summary>
        public bool IsRequestRequired { get; set; }
    }
}
