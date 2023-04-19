// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did;
using System;
using System.Text.Json;
using System.Threading;

var extractor = new IdentityDocumentExtractor(new IdentityDocumentConfigurationStore());
var didDocument = await extractor.Extract("did:ethr:aurora:0x036d148205e34a8591dcdcea34fb7fed760f5f1eca66d254830833f755ff359ef0", CancellationToken.None);
var json = JsonSerializer.Serialize(didDocument);
Console.WriteLine(json);
Console.ReadLine();