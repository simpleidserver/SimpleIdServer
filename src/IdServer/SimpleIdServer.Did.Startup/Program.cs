// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Ethr;
using SimpleIdServer.Did.Ethr.Services;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Jwt;
using SimpleIdServer.Did.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;

const string privateKey = "0fda34d0029c91481b1f54b0b68efea94c4572c80b2902cb3a2ab722b41fc1e1";
const string publicKey = "0x6f13168149E94B7BaC11B590cbE772593eD0742f";
const string contractAdr = "0x5721d5e733a2da7d805bdfb5177b4801cd86d3ae";
const string network = "sepolia";

// TODO : 
// Create INFURA Account and store the ProjectId : https://www.infura.io/.
// Install METAMASK wallet.
// Transfer ETHR to SEPOLIA : https://goerlifaucet.com/

// CheckTransaction();
// CheckJWT();
// DisplayBalance();
// DeployContract();
// SyncIdentityDocument();
ExtractDID();

Console.ReadLine();

// https://github.com/uport-project/ethr-did-registry/blob/ebabeee54a0e33110f3516364178910761466ec7/deployments.ts
// Extract : https://etherscan.io/address/0xdca7ef03e98e0dc2b855be647c39abe984fcf21b#code
// https://github.com/decentralized-identity/ethr-did-resolver/blob/c0d036618fd17f46053a6dd736e72d1aca91f358/src/configuration.ts

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

async void DisplayBalance()
{
    var accountService = new SmartContractService(new IdentityDocumentConfigurationStore(), new DidEthrOptions());
    var balance = await accountService.UseAccount(privateKey).UseNetwork(network).GetCurrentBalance();
    Console.WriteLine($"Balance is {balance.Value}");
}

async void DeployContract()
{
    var accountService = new SmartContractService(new IdentityDocumentConfigurationStore(), new DidEthrOptions());
    var balance = await accountService.UseAccount(privateKey).UseNetwork(network).DeployContractAndGetService();
    Console.WriteLine("Contract is deployed !");
}

async void SyncIdentityDocument()
{
    // Synchronize an identity document into the blockchain.
    var identityDocument = IdentityDocumentBuilder.New($"did:ethr:{network}:{publicKey}", publicKey)
        .AddServiceEndpoint("github", "https://shorturl.at/eiDKO")
        .Build();
    var sync = new IdentityDocumentSynchronizer(new DIDRegistryServiceFactory(new IdentityDocumentConfigurationStore(), new DidEthrOptions()));
    await sync.Sync(identityDocument, publicKey, privateKey, contractAdr);
    // Each identifier has a controller address.
    // This controller address must be represented in the DID Document as a verificationMethod entry with the id set as the DID besing resolved and with the fragment #controller appênded to it.
    // ERTC1056 contract publishes three types of events for each identifier.
}

async void ExtractDID()
{
    var extractor = new IdentityDocumentExtractor(new IdentityDocumentConfigurationStore());
    var etherDidDocument = await extractor.Extract($"did:ethr:{network}:{publicKey}", CancellationToken.None);
}

async void ExtractDIDAndAddPublicKey()
{
    var extractor = new IdentityDocumentExtractor(new IdentityDocumentConfigurationStore());
    // var auroraDidDocument = await extractor.Extract("did:ethr:aurora:0x036d148205e34a8591dcdcea34fb7fed760f5f1eca66d254830833f755ff359ef0", CancellationToken.None);
    var etherDidDocument = await extractor.Extract($"did:ethr:{network}:{publicKey}", CancellationToken.None);
    // IdentityDocumentBuilder.New(etherDidDocument).AddServiceEndpoint();
    // etherDidDocument.AddAuthentication();
}

void DisplayDidDocument(IdentityDocument document)
{
    var json = JsonSerializer.Serialize(document);
    Console.WriteLine(json);
}