// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;

namespace SimpleIdServer.DPoP
{
    public class DPoPValidationResult
    {
        private DPoPValidationResult(JsonWebToken jwt)
        {
            Jwt = jwt;
            IsValid = true;
        }

        private DPoPValidationResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
            IsValid = false;
        }

        public string ErrorMessage { get; private set; }
        public bool IsValid { get; private set; }
        public JsonWebToken Jwt { get; private set; }

        public static DPoPValidationResult Error(string errorMessage) => new DPoPValidationResult(errorMessage);

        public static DPoPValidationResult Ok(JsonWebToken jwt) => new DPoPValidationResult(jwt);
    }
}
