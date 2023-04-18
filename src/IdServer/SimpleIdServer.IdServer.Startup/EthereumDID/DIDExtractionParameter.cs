// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Startup.EthereumDID
{
    public record DIDExtractionParameter
    {
        public DIDExtractionParameter(string url)
        {
            Url = url;
        }

        public string Url { get; set; }
    }
}
