namespace SimpleIdServer.OpenBankingApi.Domains.Account.Enums
{
    public class ExternalAccountIdentificationTypes : Enumeration
    {
        /// <summary>
        /// Basic Bank Account Number (BBAN) - identifier used nationally by financial institutions.
        /// </summary>
        public static ExternalAccountIdentificationTypes BBAN = new ExternalAccountIdentificationTypes(0, "BBAN");
        /// <summary>
        /// An identifier used internationally by financial institutions to uniquely identify the account of a customer at a financial institution.
        /// </summary>
        public static ExternalAccountIdentificationTypes IBAN = new ExternalAccountIdentificationTypes(1, "IBAN");
        /// <summary>
        /// Primary Account Number - identifier scheme used to identify a card account.
        /// </summary>
        public static ExternalAccountIdentificationTypes PAN = new ExternalAccountIdentificationTypes(2, "PAN");
        /// <summary>
        /// Paym Scheme to make payments via mobile.
        /// </summary>
        public static ExternalAccountIdentificationTypes Paym = new ExternalAccountIdentificationTypes(3, "Paym");
        /// <summary>
        /// Sort Code and Account Number - identifier scheme used in the UK by financial institutions to identify the account of a customer.
        /// </summary>
        public static ExternalAccountIdentificationTypes SortCodeAccountNumber = new ExternalAccountIdentificationTypes(4, "SortCodeAccountNumber");

        private ExternalAccountIdentificationTypes(int id, string name) : base(id, name) { }
    }
}