// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using System;

namespace SimpleIdServer.Did.Formatters;

public class EcdsaSecp256k1RecoveryMethod2020Formatter : IVerificationMethodFormatter
{
    public const string JSON_LD_CONTEXT = "https://w3id.org/security/suites/secp256k1recovery-2020/v2";
    public const string TYPE = "EcdsaSecp256k1RecoveryMethod2020";

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => TYPE;

    private EcdsaSecp256k1RecoveryMethod2020FormattingTypes FormattingType { get; set; }

    private CAIP10BlockChainAccount BlockChainAccount { get; set; }

    public static EcdsaSecp256k1RecoveryMethod2020Formatter BuildBlockChainFormatter(CAIP10BlockChainAccount blockChainAccount)
    {
        return new EcdsaSecp256k1RecoveryMethod2020Formatter
        {
            FormattingType = EcdsaSecp256k1RecoveryMethod2020FormattingTypes.BLOCK_CHAIN_ACCOUNT_ID,
            BlockChainAccount = blockChainAccount
        };
    }

    public DidDocumentVerificationMethod Format(DidDocument idDocument, IAsymmetricKey signatureKey, bool includePrivateKey)
    {
        var result = new DidDocumentVerificationMethod
        {
            Id = $"{idDocument.Id}#controller",
            Type = Type,
            Controller = idDocument.Id,
        };
        switch(FormattingType)
        {
            case EcdsaSecp256k1RecoveryMethod2020FormattingTypes.BLOCK_CHAIN_ACCOUNT_ID:
                result.BlockChainAccountId = BlockChainAccount.ToString();
                break;
        }
        return result;
    }

    public IAsymmetricKey Extract(DidDocumentVerificationMethod didDocumentVerificationMethod)
    {
        throw new NotImplementedException();
    }

}

public enum EcdsaSecp256k1RecoveryMethod2020FormattingTypes
{
    BLOCK_CHAIN_ACCOUNT_ID = 0
}

/// <summary>
/// Format is defined here : https://github.com/ChainAgnostic/CAIPs/blob/main/CAIPs/caip-10.md
/// </summary>
public class CAIP10BlockChainAccount
{
    public string Namespace { get; set; }
    public string? Reference { get; set; }
    public string AccountAddress { get; set; }

    public static CAIP10BlockChainAccount BuildEthereumMainet(string accountAddress)
    {
        return new CAIP10BlockChainAccount
        {
            Namespace = "eip155",
            Reference = "1",
            AccountAddress = accountAddress
        };
    }

    public override string ToString()
    {
        var result = Namespace;
        if (!string.IsNullOrWhiteSpace(Reference)) result = $"{Namespace}:{Reference}";
        return $"{result}:{AccountAddress}";
    }
}