// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.AuthFaker;
using SimpleIdServer.IdServer.AuthFaker.Vp;
using SimpleIdServer.IdServer.AuthFaker.VpRegistration;
using System.Text.Json;

var qrCode = QrCodeHelper.ReadQrCode(new Options
{
    Url = "https://localhost:5001",
    BrowserId = "ecbcb5b9-3dd8-427c-8b9a-1c1a2785fb94",
}).Result;
if (qrCode.StartsWith("openid4vp://"))
{
    new VpCommand().Execute(qrCode).Wait();
    return;
}

var qrCodeResult = JsonSerializer.Deserialize<QRCodeResult>(qrCode);
await new MobileCommand().Execute(qrCodeResult);