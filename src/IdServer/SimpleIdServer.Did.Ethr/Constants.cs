// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Ethr.Models;
using SimpleIdServer.Did.Models;
using System.Collections.Generic;

namespace SimpleIdServer.Did.Ethr
{
    public static class Constants
    {
        public const string Type = "ethr";

        public const string DefaultContractAdr = "0x5721d5e733a2da7d805bdfb5177b4801cd86d3ae";

        public const string DefaultInfuraProjectId = "405e16111db4419e8d94431737f8ba53";

        public const string DefaultSource = "mainnet";

        public static ICollection<NetworkConfiguration> StandardNetworkConfigurations = new List<NetworkConfiguration>
        {
            new NetworkConfiguration { Name = "mainnet", RpcUrl = "https://mainnet.infura.io/v3/{0}", ContractAdr = DefaultContractAdr },
            new NetworkConfiguration { Name = "aurora", RpcUrl = "https://aurora-mainnet.infura.io/v3/{0}", ContractAdr = DefaultContractAdr },
            new NetworkConfiguration { Name = "sepolia", RpcUrl = "https://rpc.sepolia.org", ContractAdr = DefaultContractAdr }
        };

        public const string StandardSepc256K1RecoveryContext = "https://w3id.org/security/suites/secp256k1recovery-2020/v2";

        public static Dictionary<string, string> LegacyAttrTypes = new Dictionary<string, string>
        {
            { "sigAuth", Did.Constants.LegacyAttributeTypes.SignatureAuthentication2018 },
            { "veriKey", Did.Constants.LegacyAttributeTypes.VerificationKey2018 },
            { "enc", Did.Constants.LegacyAttributeTypes.KeyAgreementKey2019 }
        };

        public static Dictionary<string, string> LegacyAlgos = new Dictionary<string, string>
        {
            { Did.Constants.VerificationMethodTypes.Secp256k1VerificationKey2018, Did.Constants.VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019 },
            { "Ed25519SignatureAuthentication2018", Did.Constants.VerificationMethodTypes.Ed25519VerificationKey2018 },
            { "Secp256k1SignatureAuthentication2018", Did.Constants.VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019 },
            { Did.Constants.VerificationMethodTypes.RSAVerificationKey2018, Did.Constants.VerificationMethodTypes.RSAVerificationKey2018 },
            { Did.Constants.VerificationMethodTypes.Ed25519VerificationKey2018, Did.Constants.VerificationMethodTypes.Ed25519VerificationKey2018 },
            { Did.Constants.VerificationMethodTypes.X25519KeyAgreementKey2019, Did.Constants.VerificationMethodTypes.X25519KeyAgreementKey2019 }
        };

        public static Dictionary<KeyPurposes, string> KeyPurposeNames = new Dictionary<KeyPurposes, string>
        {
            { KeyPurposes.SigAuthentication, "sigAuth" },
            { KeyPurposes.VerificationKey, "veriKey" },
            { KeyPurposes.Encryption, "enc" }
        };
    }
}
