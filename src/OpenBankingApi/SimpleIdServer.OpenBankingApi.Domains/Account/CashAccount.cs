using SimpleIdServer.OpenBankingApi.Domains.Account.Enums;
using System;

namespace SimpleIdServer.OpenBankingApi.Domains.Account
{
    public class CashAccount : ICloneable
    {
        /// <summary>
        /// Name of the identification scheme, in a coded form as published in an external list.
        /// </summary>
        public ExternalAccountIdentificationTypes SchemeName { get; set; }
        /// <summary>
        /// Identification assigned by an institution to identify an account. 
        /// This identification is known by the account owner.
        /// </summary>
        public string Identification { get; set; }
        /// <summary>
        /// The account name is the name or names of the account owner(s) represented at an account level, as displayed by the ASPSP's online channels. 
        /// Note, the account name is not the product name or the nickname of the account.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// This is secondary identification of the account, as assigned by the account servicing institution. This can be used by building societies to additionally identify accounts with a roll number (in addition to a sort code and account number combination).
        /// </summary>
        public string SecondaryIdentification { get; set; }

        public object Clone()
        {
            return new CashAccount
            {
                SchemeName = SchemeName,
                Identification = Identification,
                Name = Name,
                SecondaryIdentification = SecondaryIdentification
            };
        }
    }
}
