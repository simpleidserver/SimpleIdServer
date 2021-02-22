using SimpleIdServer.OpenBankingApi.Domains.Account.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OpenBankingApi.Domains.Account
{
    public class AccountAggregate : BaseAggregate
    {
        public AccountAggregate()
        {
            Accounts = new List<CashAccount>();
        }

        /// <summary>
        /// Specifies the status of account resource in code form.
        /// </summary>
        public AccountStatus Status { get; set; }
        /// <summary>
        /// Date and time at which the resource status was updated.
        /// </summary>
        public DateTime? StatusUpdateDateTime { get; set; }
        /// <summary>
        /// Identification of the currency in which the account is held.
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Specifies the type of account (personnal or business).
        /// </summary>
        public AccountTypes AccountType { get; set; }
        /// <summary>
        /// Specifies the sub type of account (product family group).
        /// </summary>
        public AccountSubTypes AccountSubType { get; set; }
        /// <summary>
        /// Specifies the description of the account type.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The nickname of the account, assigned by the account owner in order to provide an additional means of identification of the account.
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// Date on which the account and related basic services are effectively operational for the account owner.
        /// </summary>
        public DateTime? OpeningDate { get; set; }
        /// <summary>
        /// Maturity date for the account.
        /// </summary>
        public DateTime? MaturityDate { get; set; }
        /// <summary>
        /// The switch status for the account.
        /// </summary>
        public AccountSwitchStatus SwitchStatus { get; set; }
        /// <summary>
        /// Provides the details to identify an account.
        /// </summary>
        public ICollection<CashAccount> Accounts { get; set; }
        /// <summary>
        /// Party that manages the account on behalf of the account owner, that is manages the registration and booking of entries on the account, calculates balances on the account and provides information about the account.
        /// </summary>
        public Servicer Servicer { get; set; }
        /// <summary>
        /// Subject.
        /// </summary>
        public string Subject { get; set; }

        public override object Clone()
        {
            return new AccountAggregate
            {
                AggregateId = AggregateId,
                Version = Version,
                Status = Status,
                Currency = Currency,
                AccountSubType = AccountSubType,
                Description = Description,
                StatusUpdateDateTime = StatusUpdateDateTime,
                AccountType = AccountType,
                Nickname = Nickname,
                OpeningDate = OpeningDate,
                MaturityDate = MaturityDate,
                SwitchStatus = SwitchStatus,
                Accounts = Accounts.Select(a => (CashAccount)a.Clone()).ToList(),
                Servicer = (Servicer)Servicer?.Clone(),
                Subject = Subject
            };
        }

        public override void Handle(dynamic evt)
        {

        }
    }
}
