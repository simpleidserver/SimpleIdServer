// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using CommandLine;
using SimpleIdServer.IdServer.AuthFaker;
using SimpleIdServer.IdServer.AuthFaker.Vp;
using SimpleIdServer.IdServer.AuthFaker.VpRegistration;
using System.Text.Json;

var qrCode = QrCodeHelper.ReadQrCode(new Options
{
    Url = "https://localhost:5001",
    BrowserId = "a334daea-df0a-4ef2-9d71-26c5819e770c",
}).Result;
if (qrCode.StartsWith("openid4vp://"))
{
    new VpCommand().Execute(qrCode).Wait();
    return;
}

var qrCodeResult = JsonSerializer.Deserialize<QRCodeResult>(qrCode);
await new MobileCommand().Execute(qrCodeResult);

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(async (options) =>
    {

    });