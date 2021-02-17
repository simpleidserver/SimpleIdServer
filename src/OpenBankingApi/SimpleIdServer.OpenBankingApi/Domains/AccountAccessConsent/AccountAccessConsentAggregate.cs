using System;

namespace SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent
{
    public class AccountAccessConsentAggregate : BaseAggregate
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        
        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

        public override void Handle(dynamic evt)
        {
            throw new System.NotImplementedException();
        }
    }
}
