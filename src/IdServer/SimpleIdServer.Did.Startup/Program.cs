// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Jwt;
using SimpleIdServer.Did.Jwt.Crypto;
using SimpleIdServer.Did.Models;
using System.IO;
using System.Text.Json;

// CheckTransaction();
CheckJWT();

void CheckTransaction()
{
    var privateKey = "278a5de700e29faae8e40e366ec5012b5ec63d36ec77e8a2417154cc1d25383f";
    var plaintext = "thequickbrownfoxjumpedoverthelazyprogrammer";
    var key = new ES256KSignatureKey(null, privateKey.HexToByteArray());
    var signature = key.Sign(plaintext); // excepted = 'jsvdLwqr-O206hkegoq6pbo7LJjCaflEKHCvfohBP9U2H9EZ5Jsw0CncN17WntoUEGmxaZVF2zQjtUEXfhdyBg'
    var isValid = key.Check(plaintext, signature);
    string s = "";
}
 
void CheckJWT()
{
    const string jwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJFUzI1NksifQ.eyJpYXQiOjE0ODUzMjExMzMsImlzcyI6ImRpZDpldGhyOjB4OTBlNDVkNzViZDEyNDZlMDkyNDg3MjAxODY0N2RiYTk5NmE4ZTdiOSIsInJlcXVlc3RlZCI6WyJuYW1lIiwicGhvbmUiXX0.KIG2zUO8Quf3ucb9jIncZ1CmH0v-fAZlsKvesfsd9x4RzU0qrvinVd9d30DOeZOwdwEdXkET_wuPoOECwU0IKA";
    var didDocument = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "didDoc.json"));
    var didDoc = JsonSerializer.Deserialize<IdentityDocument>(didDocument);
    var validator = new JwtValidator();
    validator.Validate(jwt, didDoc);
}

/*
async void ExtractDID()
{
    var extractor = new IdentityDocumentExtractor(new IdentityDocumentConfigurationStore());
    var didDocument = await extractor.Extract("did:ethr:aurora:0x036d148205e34a8591dcdcea34fb7fed760f5f1eca66d254830833f755ff359ef0", CancellationToken.None);
    var json = JsonSerializer.Serialize(didDocument);
    Console.WriteLine(json);
    Console.ReadLine();
}
*/