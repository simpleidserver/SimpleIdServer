// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api.Authorization.ResponseModes;
using System.Collections.Generic;

namespace SimpleIdServer.OpenBankingApi.Api.Authorization.ResponseModes
{
    public class OpenBankingApiResponseModeHandler : ResponseModeHandler
    {
        public OpenBankingApiResponseModeHandler(IEnumerable<IOAuthResponseModeHandler> oauthResponseModeHandlers) : base(oauthResponseModeHandlers)
        {
        }

        protected override string GetDefaultResponseMode(IEnumerable<string> responseTypes)
        {
            return FragmentResponseModeHandler.NAME;
        }
    }
}
