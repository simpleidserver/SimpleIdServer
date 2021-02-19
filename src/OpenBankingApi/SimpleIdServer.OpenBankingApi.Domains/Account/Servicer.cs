using SimpleIdServer.OpenBankingApi.Domains.Account.Enums;
using System;

namespace SimpleIdServer.OpenBankingApi.Domains.Account
{
    public class Servicer : ValueObject, ICloneable
    {
        /// <summary>
        /// Name of the identification scheme, in a coded form as published in an external list.
        /// </summary>
        public ExternalAccountIdentificationTypes SchemeName { get; set; }
        /// <summary>
        /// Unique and unambiguous identification of the servicing institution.
        /// </summary>
        public string Identification { get; set; }

        public object Clone()
        {
            return new Servicer
            {
                SchemeName = SchemeName,
                Identification = Identification
            };
        }
    }
}
