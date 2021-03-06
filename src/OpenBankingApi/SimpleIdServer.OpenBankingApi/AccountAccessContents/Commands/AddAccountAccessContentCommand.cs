﻿using MediatR;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OpenBankingApi.AccountAccessContents.Results;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OpenBankingApi.AccountAccessContents.Commands
{
    public class AddAccountAccessContentCommand : IRequest<AccountAccessContentResult>
    {
        public string Token { get; set; }
        public AddAccountAccessContentData Data { get; set; }
        public JObject Risk { get; set; }
        public string Issuer { get; set; }
    }

    public class AddAccountAccessContentData
    {
        public ICollection<string> Permissions { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public DateTime? TransactionFromDateTime { get; set; }
        public DateTime? TransactionToDateTime { get; set; }
    }
}
