using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using SimpleIdServer.IdServer.Startup.EthereumDID.ContractDefinition;

namespace SimpleIdServer.IdServer.Startup.EthereumDID
{
    public partial class EthereumDIDService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, EthereumDIDDeployment ethereumDIDDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<EthereumDIDDeployment>().SendRequestAndWaitForReceiptAsync(ethereumDIDDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, EthereumDIDDeployment ethereumDIDDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<EthereumDIDDeployment>().SendRequestAsync(ethereumDIDDeployment);
        }

        public static async Task<EthereumDIDService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, EthereumDIDDeployment ethereumDIDDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, ethereumDIDDeployment, cancellationTokenSource);
            return new EthereumDIDService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public EthereumDIDService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public EthereumDIDService(Nethereum.Web3.IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> RevokeAttributeRequestAsync(RevokeAttributeFunction revokeAttributeFunction)
        {
             return ContractHandler.SendRequestAsync(revokeAttributeFunction);
        }

        public Task<TransactionReceipt> RevokeAttributeRequestAndWaitForReceiptAsync(RevokeAttributeFunction revokeAttributeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeAttributeFunction, cancellationToken);
        }

        public Task<string> RevokeAttributeRequestAsync(string identity, byte[] name, byte[] value)
        {
            var revokeAttributeFunction = new RevokeAttributeFunction();
                revokeAttributeFunction.Identity = identity;
                revokeAttributeFunction.Name = name;
                revokeAttributeFunction.Value = value;
            
             return ContractHandler.SendRequestAsync(revokeAttributeFunction);
        }

        public Task<TransactionReceipt> RevokeAttributeRequestAndWaitForReceiptAsync(string identity, byte[] name, byte[] value, CancellationTokenSource cancellationToken = null)
        {
            var revokeAttributeFunction = new RevokeAttributeFunction();
                revokeAttributeFunction.Identity = identity;
                revokeAttributeFunction.Name = name;
                revokeAttributeFunction.Value = value;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeAttributeFunction, cancellationToken);
        }

        public Task<string> OwnersQueryAsync(OwnersFunction ownersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnersFunction, string>(ownersFunction, blockParameter);
        }

        
        public Task<string> OwnersQueryAsync(string returnValue1, BlockParameter blockParameter = null)
        {
            var ownersFunction = new OwnersFunction();
                ownersFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<OwnersFunction, string>(ownersFunction, blockParameter);
        }

        public Task<BigInteger> DelegatesQueryAsync(DelegatesFunction delegatesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DelegatesFunction, BigInteger>(delegatesFunction, blockParameter);
        }

        
        public Task<BigInteger> DelegatesQueryAsync(string returnValue1, byte[] returnValue2, string returnValue3, BlockParameter blockParameter = null)
        {
            var delegatesFunction = new DelegatesFunction();
                delegatesFunction.ReturnValue1 = returnValue1;
                delegatesFunction.ReturnValue2 = returnValue2;
                delegatesFunction.ReturnValue3 = returnValue3;
            
            return ContractHandler.QueryAsync<DelegatesFunction, BigInteger>(delegatesFunction, blockParameter);
        }

        public Task<string> SetAttributeSignedRequestAsync(SetAttributeSignedFunction setAttributeSignedFunction)
        {
             return ContractHandler.SendRequestAsync(setAttributeSignedFunction);
        }

        public Task<TransactionReceipt> SetAttributeSignedRequestAndWaitForReceiptAsync(SetAttributeSignedFunction setAttributeSignedFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setAttributeSignedFunction, cancellationToken);
        }

        public Task<string> SetAttributeSignedRequestAsync(string identity, byte sigV, byte[] sigR, byte[] sigS, byte[] name, byte[] value, BigInteger validity)
        {
            var setAttributeSignedFunction = new SetAttributeSignedFunction();
                setAttributeSignedFunction.Identity = identity;
                setAttributeSignedFunction.SigV = sigV;
                setAttributeSignedFunction.SigR = sigR;
                setAttributeSignedFunction.SigS = sigS;
                setAttributeSignedFunction.Name = name;
                setAttributeSignedFunction.Value = value;
                setAttributeSignedFunction.Validity = validity;
            
             return ContractHandler.SendRequestAsync(setAttributeSignedFunction);
        }

        public Task<TransactionReceipt> SetAttributeSignedRequestAndWaitForReceiptAsync(string identity, byte sigV, byte[] sigR, byte[] sigS, byte[] name, byte[] value, BigInteger validity, CancellationTokenSource cancellationToken = null)
        {
            var setAttributeSignedFunction = new SetAttributeSignedFunction();
                setAttributeSignedFunction.Identity = identity;
                setAttributeSignedFunction.SigV = sigV;
                setAttributeSignedFunction.SigR = sigR;
                setAttributeSignedFunction.SigS = sigS;
                setAttributeSignedFunction.Name = name;
                setAttributeSignedFunction.Value = value;
                setAttributeSignedFunction.Validity = validity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setAttributeSignedFunction, cancellationToken);
        }

        public Task<string> ChangeOwnerSignedRequestAsync(ChangeOwnerSignedFunction changeOwnerSignedFunction)
        {
             return ContractHandler.SendRequestAsync(changeOwnerSignedFunction);
        }

        public Task<TransactionReceipt> ChangeOwnerSignedRequestAndWaitForReceiptAsync(ChangeOwnerSignedFunction changeOwnerSignedFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(changeOwnerSignedFunction, cancellationToken);
        }

        public Task<string> ChangeOwnerSignedRequestAsync(string identity, byte sigV, byte[] sigR, byte[] sigS, string newOwner)
        {
            var changeOwnerSignedFunction = new ChangeOwnerSignedFunction();
                changeOwnerSignedFunction.Identity = identity;
                changeOwnerSignedFunction.SigV = sigV;
                changeOwnerSignedFunction.SigR = sigR;
                changeOwnerSignedFunction.SigS = sigS;
                changeOwnerSignedFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAsync(changeOwnerSignedFunction);
        }

        public Task<TransactionReceipt> ChangeOwnerSignedRequestAndWaitForReceiptAsync(string identity, byte sigV, byte[] sigR, byte[] sigS, string newOwner, CancellationTokenSource cancellationToken = null)
        {
            var changeOwnerSignedFunction = new ChangeOwnerSignedFunction();
                changeOwnerSignedFunction.Identity = identity;
                changeOwnerSignedFunction.SigV = sigV;
                changeOwnerSignedFunction.SigR = sigR;
                changeOwnerSignedFunction.SigS = sigS;
                changeOwnerSignedFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(changeOwnerSignedFunction, cancellationToken);
        }

        public Task<bool> ValidDelegateQueryAsync(ValidDelegateFunction validDelegateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ValidDelegateFunction, bool>(validDelegateFunction, blockParameter);
        }

        
        public Task<bool> ValidDelegateQueryAsync(string identity, byte[] delegateType, string @delegate, BlockParameter blockParameter = null)
        {
            var validDelegateFunction = new ValidDelegateFunction();
                validDelegateFunction.Identity = identity;
                validDelegateFunction.DelegateType = delegateType;
                validDelegateFunction.Delegate = @delegate;
            
            return ContractHandler.QueryAsync<ValidDelegateFunction, bool>(validDelegateFunction, blockParameter);
        }

        public Task<BigInteger> NonceQueryAsync(NonceFunction nonceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NonceFunction, BigInteger>(nonceFunction, blockParameter);
        }

        
        public Task<BigInteger> NonceQueryAsync(string returnValue1, BlockParameter blockParameter = null)
        {
            var nonceFunction = new NonceFunction();
                nonceFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<NonceFunction, BigInteger>(nonceFunction, blockParameter);
        }

        public Task<string> SetAttributeRequestAsync(SetAttributeFunction setAttributeFunction)
        {
             return ContractHandler.SendRequestAsync(setAttributeFunction);
        }

        public Task<TransactionReceipt> SetAttributeRequestAndWaitForReceiptAsync(SetAttributeFunction setAttributeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setAttributeFunction, cancellationToken);
        }

        public Task<string> SetAttributeRequestAsync(string identity, byte[] name, byte[] value, BigInteger validity)
        {
            var setAttributeFunction = new SetAttributeFunction();
                setAttributeFunction.Identity = identity;
                setAttributeFunction.Name = name;
                setAttributeFunction.Value = value;
                setAttributeFunction.Validity = validity;
            
             return ContractHandler.SendRequestAsync(setAttributeFunction);
        }

        public Task<TransactionReceipt> SetAttributeRequestAndWaitForReceiptAsync(string identity, byte[] name, byte[] value, BigInteger validity, CancellationTokenSource cancellationToken = null)
        {
            var setAttributeFunction = new SetAttributeFunction();
                setAttributeFunction.Identity = identity;
                setAttributeFunction.Name = name;
                setAttributeFunction.Value = value;
                setAttributeFunction.Validity = validity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setAttributeFunction, cancellationToken);
        }

        public Task<string> RevokeDelegateRequestAsync(RevokeDelegateFunction revokeDelegateFunction)
        {
             return ContractHandler.SendRequestAsync(revokeDelegateFunction);
        }

        public Task<TransactionReceipt> RevokeDelegateRequestAndWaitForReceiptAsync(RevokeDelegateFunction revokeDelegateFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeDelegateFunction, cancellationToken);
        }

        public Task<string> RevokeDelegateRequestAsync(string identity, byte[] delegateType, string @delegate)
        {
            var revokeDelegateFunction = new RevokeDelegateFunction();
                revokeDelegateFunction.Identity = identity;
                revokeDelegateFunction.DelegateType = delegateType;
                revokeDelegateFunction.Delegate = @delegate;
            
             return ContractHandler.SendRequestAsync(revokeDelegateFunction);
        }

        public Task<TransactionReceipt> RevokeDelegateRequestAndWaitForReceiptAsync(string identity, byte[] delegateType, string @delegate, CancellationTokenSource cancellationToken = null)
        {
            var revokeDelegateFunction = new RevokeDelegateFunction();
                revokeDelegateFunction.Identity = identity;
                revokeDelegateFunction.DelegateType = delegateType;
                revokeDelegateFunction.Delegate = @delegate;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeDelegateFunction, cancellationToken);
        }

        public Task<string> IdentityOwnerQueryAsync(IdentityOwnerFunction identityOwnerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IdentityOwnerFunction, string>(identityOwnerFunction, blockParameter);
        }

        
        public Task<string> IdentityOwnerQueryAsync(string identity, BlockParameter blockParameter = null)
        {
            var identityOwnerFunction = new IdentityOwnerFunction();
                identityOwnerFunction.Identity = identity;
            
            return ContractHandler.QueryAsync<IdentityOwnerFunction, string>(identityOwnerFunction, blockParameter);
        }

        public Task<string> RevokeDelegateSignedRequestAsync(RevokeDelegateSignedFunction revokeDelegateSignedFunction)
        {
             return ContractHandler.SendRequestAsync(revokeDelegateSignedFunction);
        }

        public Task<TransactionReceipt> RevokeDelegateSignedRequestAndWaitForReceiptAsync(RevokeDelegateSignedFunction revokeDelegateSignedFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeDelegateSignedFunction, cancellationToken);
        }

        public Task<string> RevokeDelegateSignedRequestAsync(string identity, byte sigV, byte[] sigR, byte[] sigS, byte[] delegateType, string @delegate)
        {
            var revokeDelegateSignedFunction = new RevokeDelegateSignedFunction();
                revokeDelegateSignedFunction.Identity = identity;
                revokeDelegateSignedFunction.SigV = sigV;
                revokeDelegateSignedFunction.SigR = sigR;
                revokeDelegateSignedFunction.SigS = sigS;
                revokeDelegateSignedFunction.DelegateType = delegateType;
                revokeDelegateSignedFunction.Delegate = @delegate;
            
             return ContractHandler.SendRequestAsync(revokeDelegateSignedFunction);
        }

        public Task<TransactionReceipt> RevokeDelegateSignedRequestAndWaitForReceiptAsync(string identity, byte sigV, byte[] sigR, byte[] sigS, byte[] delegateType, string @delegate, CancellationTokenSource cancellationToken = null)
        {
            var revokeDelegateSignedFunction = new RevokeDelegateSignedFunction();
                revokeDelegateSignedFunction.Identity = identity;
                revokeDelegateSignedFunction.SigV = sigV;
                revokeDelegateSignedFunction.SigR = sigR;
                revokeDelegateSignedFunction.SigS = sigS;
                revokeDelegateSignedFunction.DelegateType = delegateType;
                revokeDelegateSignedFunction.Delegate = @delegate;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeDelegateSignedFunction, cancellationToken);
        }

        public Task<string> AddDelegateSignedRequestAsync(AddDelegateSignedFunction addDelegateSignedFunction)
        {
             return ContractHandler.SendRequestAsync(addDelegateSignedFunction);
        }

        public Task<TransactionReceipt> AddDelegateSignedRequestAndWaitForReceiptAsync(AddDelegateSignedFunction addDelegateSignedFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addDelegateSignedFunction, cancellationToken);
        }

        public Task<string> AddDelegateSignedRequestAsync(string identity, byte sigV, byte[] sigR, byte[] sigS, byte[] delegateType, string @delegate, BigInteger validity)
        {
            var addDelegateSignedFunction = new AddDelegateSignedFunction();
                addDelegateSignedFunction.Identity = identity;
                addDelegateSignedFunction.SigV = sigV;
                addDelegateSignedFunction.SigR = sigR;
                addDelegateSignedFunction.SigS = sigS;
                addDelegateSignedFunction.DelegateType = delegateType;
                addDelegateSignedFunction.Delegate = @delegate;
                addDelegateSignedFunction.Validity = validity;
            
             return ContractHandler.SendRequestAsync(addDelegateSignedFunction);
        }

        public Task<TransactionReceipt> AddDelegateSignedRequestAndWaitForReceiptAsync(string identity, byte sigV, byte[] sigR, byte[] sigS, byte[] delegateType, string @delegate, BigInteger validity, CancellationTokenSource cancellationToken = null)
        {
            var addDelegateSignedFunction = new AddDelegateSignedFunction();
                addDelegateSignedFunction.Identity = identity;
                addDelegateSignedFunction.SigV = sigV;
                addDelegateSignedFunction.SigR = sigR;
                addDelegateSignedFunction.SigS = sigS;
                addDelegateSignedFunction.DelegateType = delegateType;
                addDelegateSignedFunction.Delegate = @delegate;
                addDelegateSignedFunction.Validity = validity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addDelegateSignedFunction, cancellationToken);
        }

        public Task<string> AddDelegateRequestAsync(AddDelegateFunction addDelegateFunction)
        {
             return ContractHandler.SendRequestAsync(addDelegateFunction);
        }

        public Task<TransactionReceipt> AddDelegateRequestAndWaitForReceiptAsync(AddDelegateFunction addDelegateFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addDelegateFunction, cancellationToken);
        }

        public Task<string> AddDelegateRequestAsync(string identity, byte[] delegateType, string @delegate, BigInteger validity)
        {
            var addDelegateFunction = new AddDelegateFunction();
                addDelegateFunction.Identity = identity;
                addDelegateFunction.DelegateType = delegateType;
                addDelegateFunction.Delegate = @delegate;
                addDelegateFunction.Validity = validity;
            
             return ContractHandler.SendRequestAsync(addDelegateFunction);
        }

        public Task<TransactionReceipt> AddDelegateRequestAndWaitForReceiptAsync(string identity, byte[] delegateType, string @delegate, BigInteger validity, CancellationTokenSource cancellationToken = null)
        {
            var addDelegateFunction = new AddDelegateFunction();
                addDelegateFunction.Identity = identity;
                addDelegateFunction.DelegateType = delegateType;
                addDelegateFunction.Delegate = @delegate;
                addDelegateFunction.Validity = validity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addDelegateFunction, cancellationToken);
        }

        public Task<string> RevokeAttributeSignedRequestAsync(RevokeAttributeSignedFunction revokeAttributeSignedFunction)
        {
             return ContractHandler.SendRequestAsync(revokeAttributeSignedFunction);
        }

        public Task<TransactionReceipt> RevokeAttributeSignedRequestAndWaitForReceiptAsync(RevokeAttributeSignedFunction revokeAttributeSignedFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeAttributeSignedFunction, cancellationToken);
        }

        public Task<string> RevokeAttributeSignedRequestAsync(string identity, byte sigV, byte[] sigR, byte[] sigS, byte[] name, byte[] value)
        {
            var revokeAttributeSignedFunction = new RevokeAttributeSignedFunction();
                revokeAttributeSignedFunction.Identity = identity;
                revokeAttributeSignedFunction.SigV = sigV;
                revokeAttributeSignedFunction.SigR = sigR;
                revokeAttributeSignedFunction.SigS = sigS;
                revokeAttributeSignedFunction.Name = name;
                revokeAttributeSignedFunction.Value = value;
            
             return ContractHandler.SendRequestAsync(revokeAttributeSignedFunction);
        }

        public Task<TransactionReceipt> RevokeAttributeSignedRequestAndWaitForReceiptAsync(string identity, byte sigV, byte[] sigR, byte[] sigS, byte[] name, byte[] value, CancellationTokenSource cancellationToken = null)
        {
            var revokeAttributeSignedFunction = new RevokeAttributeSignedFunction();
                revokeAttributeSignedFunction.Identity = identity;
                revokeAttributeSignedFunction.SigV = sigV;
                revokeAttributeSignedFunction.SigR = sigR;
                revokeAttributeSignedFunction.SigS = sigS;
                revokeAttributeSignedFunction.Name = name;
                revokeAttributeSignedFunction.Value = value;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeAttributeSignedFunction, cancellationToken);
        }

        public Task<string> ChangeOwnerRequestAsync(ChangeOwnerFunction changeOwnerFunction)
        {
             return ContractHandler.SendRequestAsync(changeOwnerFunction);
        }

        public Task<TransactionReceipt> ChangeOwnerRequestAndWaitForReceiptAsync(ChangeOwnerFunction changeOwnerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(changeOwnerFunction, cancellationToken);
        }

        public Task<string> ChangeOwnerRequestAsync(string identity, string newOwner)
        {
            var changeOwnerFunction = new ChangeOwnerFunction();
                changeOwnerFunction.Identity = identity;
                changeOwnerFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAsync(changeOwnerFunction);
        }

        public Task<TransactionReceipt> ChangeOwnerRequestAndWaitForReceiptAsync(string identity, string newOwner, CancellationTokenSource cancellationToken = null)
        {
            var changeOwnerFunction = new ChangeOwnerFunction();
                changeOwnerFunction.Identity = identity;
                changeOwnerFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(changeOwnerFunction, cancellationToken);
        }

        public Task<BigInteger> ChangedQueryAsync(ChangedFunction changedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ChangedFunction, BigInteger>(changedFunction, blockParameter);
        }

        
        public Task<BigInteger> ChangedQueryAsync(string returnValue1, BlockParameter blockParameter = null)
        {
            var changedFunction = new ChangedFunction();
                changedFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<ChangedFunction, BigInteger>(changedFunction, blockParameter);
        }
    }
}
