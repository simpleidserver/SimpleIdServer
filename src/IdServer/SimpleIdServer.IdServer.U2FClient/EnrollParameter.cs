// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.U2FClient
{
    public class EnrollParameter
    {
        public string Rp { get; set; } = "localhost";
        public byte[] Challenge { get; set; } = null;
        public string Origin { get; set; } = "https://localhost:5001";
    }
}
