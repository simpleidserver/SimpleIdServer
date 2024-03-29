﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Nethereum.Signer;
using SimpleIdServer.Did.Ethr.Models;
using SimpleIdServer.Did.Extensions;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleIdServer.Did.Ethr;

public class DidEthrExtractor
{
    public static DecentralizedIdentifierEthr Extract(string did)
    {
        // Code used to resolve the adr.
        // https://github.com/ethers-io/ethers.js/blob/6017d3d39a4d428793bddae33d82fd814cacd878/src.ts/address/address.ts
        int? version = null;
        var decentralizedIdentifier = DidExtractor.Extract(did);
        if (decentralizedIdentifier.Method != Constants.Type) throw new ArgumentException($"method must be equals to {Constants.Type}");
        var splittedIdentifier = decentralizedIdentifier.Identifier.Split(':');
        if (splittedIdentifier.Count() > 2) throw new ArgumentException("The did identifier cannot contains more than 2 parts");
        string network = null;
        var address = splittedIdentifier[0];
        if (splittedIdentifier.Count() == 2)
        {
            network = splittedIdentifier[0];
            address = splittedIdentifier[1];
        }

        var regex = new Regex(@"(\w|\d)*\?versionId=(\d)*");
        if(regex.IsMatch(address))
        {
            var splittedAdr = address.Split("?");
            var versionId = splittedAdr.Last().Replace("versionId=", string.Empty);
            version = int.Parse(versionId);
            address = splittedAdr.First();
            decentralizedIdentifier.Identifier = decentralizedIdentifier.Identifier.Split('?').First();
        }

        string pk = null;
        var identifierPayload = address.HexToByteArray();
        if (address.Count() > 42)
        {
            var publicKey = new EthECKey(identifierPayload, false);
            var tmp = address;
            address = publicKey.GetPublicAddress();
            pk = tmp.Replace("0x", string.Empty);
        }

        return new DecentralizedIdentifierEthr(decentralizedIdentifier.Scheme, decentralizedIdentifier.Method, decentralizedIdentifier.Identifier, decentralizedIdentifier.Fragment)
        {
            Address = address,
            Network = network,
            PublicKey = pk,
            Version = version
        };
    }
}
