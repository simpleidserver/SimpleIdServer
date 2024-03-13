// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Serialization;

namespace SimpleIdServer.IdServer.VerifiablePresentation;

public static class Constants
{
    public const string AMR = "vp";

    public static class Endpoints
    {
        public const string PresentationDefinitions = "presentationdefs";
        public const string VpAuthorize = "vpauthorize";
    }
}
