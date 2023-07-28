// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.U2FClient
{
    public class AttestationParameter
    {
        public string Rp { get; set; } = "https://localhost:5001";
        public byte[] Challenge { get; set; } = null;
    }
}
