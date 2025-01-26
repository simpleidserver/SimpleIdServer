// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.AuthFaker;
using SimpleIdServer.IdServer.AuthFaker.Vp;
using SimpleIdServer.IdServer.AuthFaker.VpRegistration;
using System.Text.Json;

var qrCode = QrCodeHelper.ReadQrCode(new Options
{
    Url = "https://localhost:5001",
    BrowserId = "9a89ba59-3f6e-4845-a38c-aadaf3409b1e",
}).Result;
if (qrCode.StartsWith("openid4vp://"))
{
    new VpCommand().Execute(qrCode).Wait();
    return;
}

var qrCodeResult = JsonSerializer.Deserialize<QRCodeResult>(qrCode);
await new MobileCommand().Execute(qrCodeResult);