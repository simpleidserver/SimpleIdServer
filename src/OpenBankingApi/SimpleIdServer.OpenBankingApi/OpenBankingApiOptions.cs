namespace SimpleIdServer.OpenBankingApi
{
    public class OpenBankingApiOptions
    {
        public OpenBankingApiOptions()
        {
            TakeSnapshotEveryNbRevisions = 5;
            WaitInterval = 1000;
            CertificateAuthenticationScheme = "Certificate";
        }

        public int TakeSnapshotEveryNbRevisions { get; set; }
        public int WaitInterval { get; set; }
        public string CertificateAuthenticationScheme { get; set; }
    }
}
