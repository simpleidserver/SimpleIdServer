using MediatR;
using SimpleIdServer.OpenBankingApi.AccountAccessContents.Results;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OpenBankingApi.AccountAccessContents.Commands
{
    public class AddAccountAccessContentCommand : IRequest<AccountAccessContentResult>
    {
        public AddAccountAccessContentData Data { get; set; }
    }

    public class AddAccountAccessContentData
    {
        public ICollection<string> Permissions { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public DateTime? TransactionFromDateTime { get; set; }
        public DateTime? TransactionToDateTime { get; set; }
    }
}
