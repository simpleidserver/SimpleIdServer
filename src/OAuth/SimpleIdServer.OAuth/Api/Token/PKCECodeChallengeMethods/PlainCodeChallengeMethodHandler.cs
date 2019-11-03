// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OAuth.Api.Token.PKCECodeChallengeMethods
{
    public class PlainCodeChallengeMethodHandler : ICodeChallengeMethodHandler
    {
        public static string DEFAULT_NAME = "plain";

        public string Name => DEFAULT_NAME;

        public string Calculate(string codeVerifier)
        {
            return codeVerifier;
        }
    }
}
