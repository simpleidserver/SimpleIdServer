using SimpleIdServer.OpenBankingApi.Domains.Account;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OpenBankingApi.Accounts.Results
{
    public class AccountsResult
    {
        public IEnumerable<AccountRecordResult> Account { get; set; }
    }
    
    public class AccountRecordResult
    {
        public string AccountId { get; set; }
        public DateTime? StatusUpdateDateTime { get; set; }
        public string Currency { get; set; }
        public string AccountType { get; set; }
        public string AccountSubType { get; set; }
        public string Description { get; set; }
        public string Nickname { get; set; }
        public DateTime? OpeningDate { get; set; }
        public DateTime? MaturityDate { get; set; }
        public string SwitchStatus { get; set; }
        public ICollection<CashAccountResult> Accounts { get; set; }
        public ServicerResult Servicer { get; set; }

        public static AccountRecordResult ToResult(AccountAggregate account, IEnumerable<AccountAccessConsentPermission> permissions)
        {
            var result = new AccountRecordResult
            {
                AccountId = account.AggregateId,
                AccountSubType = account.AccountSubType?.Name,
                AccountType = account.AccountType?.Name,
                Currency = account.Currency,
                Description = account.Description,
                MaturityDate = account.MaturityDate,
                Nickname = account.Nickname,
                OpeningDate = account.OpeningDate,
                StatusUpdateDateTime = account.StatusUpdateDateTime,
                SwitchStatus = account.SwitchStatus?.Name
            };

            if (permissions.Contains(AccountAccessConsentPermission.ReadAccountsDetail))
            {
                result.Servicer = account.Servicer == null ? null : ServicerResult.ToResult(account.Servicer);
                result.Accounts = account.Accounts.Select(a => CashAccountResult.ToResult(a)).ToList();
            }

            return result;
        }
    }

    public class CashAccountResult
    {
        public string SchemeName { get; set; }
        public string Identification { get; set; }
        public string Name { get; set; }
        public string SecondaryIdentification { get; set; }

        public static CashAccountResult ToResult(CashAccount cashAccount)
        {
            return new CashAccountResult
            {
                Identification = cashAccount.Identification,
                Name = cashAccount.Name,
                SchemeName = cashAccount.SchemeName?.Name,
                SecondaryIdentification = cashAccount.SecondaryIdentification
            };
        }
    }

    public class ServicerResult
    {
        public string SchemeName { get; set; }
        public string Identification { get; set; }

        public static ServicerResult ToResult(Servicer servicer)
        {
            return new ServicerResult
            {
                Identification = servicer.Identification,
                SchemeName = servicer.SchemeName?.Name
            };
        }
    }
}