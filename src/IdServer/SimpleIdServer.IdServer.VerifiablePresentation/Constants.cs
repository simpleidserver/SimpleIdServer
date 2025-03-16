// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.VerifiablePresentation;

public static class Constants
{
    public const string AMR = "vp";

    public static class Endpoints
    {
        public const string PresentationDefinitions = "presentationdefs";
        public const string VpAuthorize = "vpauthorize";
        public const string VpRegister = "vpregister";
        public const string VpAuthorizeCallback = $"{VpAuthorize}/cb";
        public const string VpAuthorizeQrCode = $"{VpAuthorize}/qr";
        public const string VpRegisterStatus = $"{VpRegister}/status";
        public const string VpEndRegister = $"{VpRegister}/end";
        public const string VpAuthorizePost = $"{IdServer.Config.DefaultEndpoints.Authorization}/post";
    }
}
