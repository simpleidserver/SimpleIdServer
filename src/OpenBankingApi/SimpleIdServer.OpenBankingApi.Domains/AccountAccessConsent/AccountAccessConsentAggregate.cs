// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license informati
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Events;
using SimpleIdServer.OpenBankingApi.Domains.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent
{
    public class AccountAccessConsentAggregate : BaseAggregate
    {
        private AccountAccessConsentAggregate()
        {
            Permissions = new List<AccountAccessConsentPermission>();
        }

        /// <summary>
        /// Date and time at which the resource was created.
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// Specifies the status of consent resource in code form.
        /// </summary>
        public AccountAccessConsentStatus Status { get; set; }
        /// <summary>
        /// Date and time at which the resource status was updated.
        /// </summary>
        public DateTime StatusUpdateDateTime { get; set; }
        /// <summary>
        /// Specifies the Open Banking account access data types. 
        /// This is a list of the data clusters being consented by the PSU, and requested for authorisation with the ASPSP.
        /// </summary>
        public ICollection<AccountAccessConsentPermission> Permissions { get; set; }
        /// <summary>
        /// Specified date and time the permissions will expire. 
        /// If this is not populated, the permissions will be open ended.
        /// </summary>
        public DateTime? ExpirationDateTime { get; set; }
        /// <summary>
        /// Specified start date and time for the transaction query period. 
        /// If this is not populated, the start date will be open ended, and data will be returned from the earliest available transaction.
        /// </summary>
        public DateTime? TransactionFromDateTime { get; set; }
        /// <summary>
        /// Specified end date and time for the transaction query period. 
        /// If this is not populated, the end date will be open ended, and data will be returned to the latest available transaction.
        /// </summary>
        public DateTime? TransactionToDateTime { get; set; }
        /// <summary>
        /// The Risk section is sent by the initiating party to the ASPSP. 
        /// It is used to specify additional details for risk scoring for Account Info.
        /// </summary>
        public string Risk { get; set; }
        /// <summary>
        /// Get account ids.
        /// </summary>
        public IEnumerable<string> AccountIds { get; set; }

        public void Confirm(IEnumerable<string> accountIds)
        {
            var evt = new AccountAccessConsentConfirmedEvent(Guid.NewGuid().ToString(), AggregateId, Version + 1, accountIds, DateTime.UtcNow);
            Handle(evt);
            DomainEvents.Add(evt);
        }

        public void Reject()
        {
            var evt = new AccountAccessConsentRejectedEvent(Guid.NewGuid().ToString(), AggregateId, Version + 1, DateTime.UtcNow);
            Handle(evt);
            DomainEvents.Add(evt);
        }

        public static AccountAccessConsentAggregate Build(ICollection<DomainEvent> domainEvents)
        {
            var result = new AccountAccessConsentAggregate();
            foreach(var domainEvt in domainEvents)
            {
                result.Handle(domainEvt);
            }

            return result;
        }

        public static AccountAccessConsentAggregate Create(ICollection<string> permissions, DateTime? expirationDateTime, DateTime? transactionFromDateTime, DateTime? transactionToDateTime, string risk)
        {
            var result = new AccountAccessConsentAggregate();
            var evt = new AccountAccessConsentAddedEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 0, permissions, expirationDateTime, transactionFromDateTime, transactionToDateTime, risk);
            result.Handle(evt);
            result.DomainEvents.Add(evt);
            return result;
        }

        public override object Clone()
        {
            return new AccountAccessConsentAggregate
            {

            };
        }

        public override void Handle(dynamic evt)
        {
            Handle(evt);
        }

        private void Handle(AccountAccessConsentAddedEvent evt)
        {
            if (evt.TransactionFromDateTime != null && evt.TransactionToDateTime != null && evt.TransactionFromDateTime.Value > evt.TransactionToDateTime.Value)
            {
                throw new BusinessRuleValidationException(Global.TransactionStartDateSuperiorToEndDate);
            }

            var unknownPermissions = new List<string>();
            var permissions = new List<AccountAccessConsentPermission>();
            if (evt.Permissions.Any())
            {
                foreach(var permission in evt.Permissions)
                {
                    try
                    {
                        permissions.Add(Enumeration.FromDisplayName<AccountAccessConsentPermission>(permission));
                    }
                    catch(InvalidOperationException)
                    {
                        unknownPermissions.Add(permission);
                    }
                }
            }

            if (unknownPermissions.Any())
            {
                throw new BusinessRuleValidationException(string.Format(Global.PermissionsAreUnknown, string.Join(",", unknownPermissions)));
            }

            AggregateId = evt.AggregateId;
            Status = AccountAccessConsentStatus.AwaitingAuthorisation;
            Permissions = permissions;
            ExpirationDateTime = evt.ExpirationDateTime;
            TransactionFromDateTime = evt.TransactionFromDateTime;
            TransactionToDateTime = evt.TransactionToDateTime;
            Risk = evt.Risk;
        }

        private void Handle(AccountAccessConsentConfirmedEvent evt)
        {
            AccountIds = evt.AccountIds;
            Status = AccountAccessConsentStatus.Authorised;
            StatusUpdateDateTime = evt.UpdateDateTime;
            Version = evt.Version;
        }

        private void Handle(AccountAccessConsentRejectedEvent evt)
        {
            Status = AccountAccessConsentStatus.Rejected;
            StatusUpdateDateTime = evt.UpdateDateTime;
            Version = evt.Version;
        }
    }
}
