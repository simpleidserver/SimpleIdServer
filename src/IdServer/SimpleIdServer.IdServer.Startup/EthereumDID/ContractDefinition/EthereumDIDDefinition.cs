using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace SimpleIdServer.IdServer.Startup.EthereumDID.ContractDefinition
{


    public partial class EthereumDIDDeployment : EthereumDIDDeploymentBase
    {
        public EthereumDIDDeployment() : base(BYTECODE) { }
        public EthereumDIDDeployment(string byteCode) : base(byteCode) { }
    }

    public class EthereumDIDDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "";
        public EthereumDIDDeploymentBase() : base(BYTECODE) { }
        public EthereumDIDDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class RevokeAttributeFunction : RevokeAttributeFunctionBase { }

    [Function("revokeAttribute")]
    public class RevokeAttributeFunctionBase : FunctionMessage
    {
        [Parameter("address", "identity", 1)]
        public virtual string Identity { get; set; }
        [Parameter("bytes32", "name", 2)]
        public virtual byte[] Name { get; set; }
        [Parameter("bytes", "value", 3)]
        public virtual byte[] Value { get; set; }
    }

    public partial class OwnersFunction : OwnersFunctionBase { }

    [Function("owners", "address")]
    public class OwnersFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class DelegatesFunction : DelegatesFunctionBase { }

    [Function("delegates", "uint256")]
    public class DelegatesFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
        [Parameter("bytes32", "", 2)]
        public virtual byte[] ReturnValue2 { get; set; }
        [Parameter("address", "", 3)]
        public virtual string ReturnValue3 { get; set; }
    }

    public partial class SetAttributeSignedFunction : SetAttributeSignedFunctionBase { }

    [Function("setAttributeSigned")]
    public class SetAttributeSignedFunctionBase : FunctionMessage
    {
        [Parameter("address", "identity", 1)]
        public virtual string Identity { get; set; }
        [Parameter("uint8", "sigV", 2)]
        public virtual byte SigV { get; set; }
        [Parameter("bytes32", "sigR", 3)]
        public virtual byte[] SigR { get; set; }
        [Parameter("bytes32", "sigS", 4)]
        public virtual byte[] SigS { get; set; }
        [Parameter("bytes32", "name", 5)]
        public virtual byte[] Name { get; set; }
        [Parameter("bytes", "value", 6)]
        public virtual byte[] Value { get; set; }
        [Parameter("uint256", "validity", 7)]
        public virtual BigInteger Validity { get; set; }
    }

    public partial class ChangeOwnerSignedFunction : ChangeOwnerSignedFunctionBase { }

    [Function("changeOwnerSigned")]
    public class ChangeOwnerSignedFunctionBase : FunctionMessage
    {
        [Parameter("address", "identity", 1)]
        public virtual string Identity { get; set; }
        [Parameter("uint8", "sigV", 2)]
        public virtual byte SigV { get; set; }
        [Parameter("bytes32", "sigR", 3)]
        public virtual byte[] SigR { get; set; }
        [Parameter("bytes32", "sigS", 4)]
        public virtual byte[] SigS { get; set; }
        [Parameter("address", "newOwner", 5)]
        public virtual string NewOwner { get; set; }
    }

    public partial class ValidDelegateFunction : ValidDelegateFunctionBase { }

    [Function("validDelegate", "bool")]
    public class ValidDelegateFunctionBase : FunctionMessage
    {
        [Parameter("address", "identity", 1)]
        public virtual string Identity { get; set; }
        [Parameter("bytes32", "delegateType", 2)]
        public virtual byte[] DelegateType { get; set; }
        [Parameter("address", "delegate", 3)]
        public virtual string Delegate { get; set; }
    }

    public partial class NonceFunction : NonceFunctionBase { }

    [Function("nonce", "uint256")]
    public class NonceFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class SetAttributeFunction : SetAttributeFunctionBase { }

    [Function("setAttribute")]
    public class SetAttributeFunctionBase : FunctionMessage
    {
        [Parameter("address", "identity", 1)]
        public virtual string Identity { get; set; }
        [Parameter("bytes32", "name", 2)]
        public virtual byte[] Name { get; set; }
        [Parameter("bytes", "value", 3)]
        public virtual byte[] Value { get; set; }
        [Parameter("uint256", "validity", 4)]
        public virtual BigInteger Validity { get; set; }
    }

    public partial class RevokeDelegateFunction : RevokeDelegateFunctionBase { }

    [Function("revokeDelegate")]
    public class RevokeDelegateFunctionBase : FunctionMessage
    {
        [Parameter("address", "identity", 1)]
        public virtual string Identity { get; set; }
        [Parameter("bytes32", "delegateType", 2)]
        public virtual byte[] DelegateType { get; set; }
        [Parameter("address", "delegate", 3)]
        public virtual string Delegate { get; set; }
    }

    public partial class IdentityOwnerFunction : IdentityOwnerFunctionBase { }

    [Function("identityOwner", "address")]
    public class IdentityOwnerFunctionBase : FunctionMessage
    {
        [Parameter("address", "identity", 1)]
        public virtual string Identity { get; set; }
    }

    public partial class RevokeDelegateSignedFunction : RevokeDelegateSignedFunctionBase { }

    [Function("revokeDelegateSigned")]
    public class RevokeDelegateSignedFunctionBase : FunctionMessage
    {
        [Parameter("address", "identity", 1)]
        public virtual string Identity { get; set; }
        [Parameter("uint8", "sigV", 2)]
        public virtual byte SigV { get; set; }
        [Parameter("bytes32", "sigR", 3)]
        public virtual byte[] SigR { get; set; }
        [Parameter("bytes32", "sigS", 4)]
        public virtual byte[] SigS { get; set; }
        [Parameter("bytes32", "delegateType", 5)]
        public virtual byte[] DelegateType { get; set; }
        [Parameter("address", "delegate", 6)]
        public virtual string Delegate { get; set; }
    }

    public partial class AddDelegateSignedFunction : AddDelegateSignedFunctionBase { }

    [Function("addDelegateSigned")]
    public class AddDelegateSignedFunctionBase : FunctionMessage
    {
        [Parameter("address", "identity", 1)]
        public virtual string Identity { get; set; }
        [Parameter("uint8", "sigV", 2)]
        public virtual byte SigV { get; set; }
        [Parameter("bytes32", "sigR", 3)]
        public virtual byte[] SigR { get; set; }
        [Parameter("bytes32", "sigS", 4)]
        public virtual byte[] SigS { get; set; }
        [Parameter("bytes32", "delegateType", 5)]
        public virtual byte[] DelegateType { get; set; }
        [Parameter("address", "delegate", 6)]
        public virtual string Delegate { get; set; }
        [Parameter("uint256", "validity", 7)]
        public virtual BigInteger Validity { get; set; }
    }

    public partial class AddDelegateFunction : AddDelegateFunctionBase { }

    [Function("addDelegate")]
    public class AddDelegateFunctionBase : FunctionMessage
    {
        [Parameter("address", "identity", 1)]
        public virtual string Identity { get; set; }
        [Parameter("bytes32", "delegateType", 2)]
        public virtual byte[] DelegateType { get; set; }
        [Parameter("address", "delegate", 3)]
        public virtual string Delegate { get; set; }
        [Parameter("uint256", "validity", 4)]
        public virtual BigInteger Validity { get; set; }
    }

    public partial class RevokeAttributeSignedFunction : RevokeAttributeSignedFunctionBase { }

    [Function("revokeAttributeSigned")]
    public class RevokeAttributeSignedFunctionBase : FunctionMessage
    {
        [Parameter("address", "identity", 1)]
        public virtual string Identity { get; set; }
        [Parameter("uint8", "sigV", 2)]
        public virtual byte SigV { get; set; }
        [Parameter("bytes32", "sigR", 3)]
        public virtual byte[] SigR { get; set; }
        [Parameter("bytes32", "sigS", 4)]
        public virtual byte[] SigS { get; set; }
        [Parameter("bytes32", "name", 5)]
        public virtual byte[] Name { get; set; }
        [Parameter("bytes", "value", 6)]
        public virtual byte[] Value { get; set; }
    }

    public partial class ChangeOwnerFunction : ChangeOwnerFunctionBase { }

    [Function("changeOwner")]
    public class ChangeOwnerFunctionBase : FunctionMessage
    {
        [Parameter("address", "identity", 1)]
        public virtual string Identity { get; set; }
        [Parameter("address", "newOwner", 2)]
        public virtual string NewOwner { get; set; }
    }

    public partial class ChangedFunction : ChangedFunctionBase { }

    [Function("changed", "uint256")]
    public class ChangedFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class DIDOwnerChangedEventDTO : DIDOwnerChangedEventDTOBase { }

    [Event("DIDOwnerChanged")]
    public class DIDOwnerChangedEventDTOBase : IEventDTO
    {
        [Parameter("address", "identity", 1, true )]
        public virtual string Identity { get; set; }
        [Parameter("address", "owner", 2, false )]
        public virtual string Owner { get; set; }
        [Parameter("uint256", "previousChange", 3, false )]
        public virtual BigInteger PreviousChange { get; set; }
    }

    public partial class DIDDelegateChangedEventDTO : DIDDelegateChangedEventDTOBase { }

    [Event("DIDDelegateChanged")]
    public class DIDDelegateChangedEventDTOBase : IEventDTO
    {
        [Parameter("address", "identity", 1, true )]
        public virtual string Identity { get; set; }
        [Parameter("bytes32", "delegateType", 2, false )]
        public virtual byte[] DelegateType { get; set; }
        [Parameter("address", "delegate", 3, false )]
        public virtual string Delegate { get; set; }
        [Parameter("uint256", "validTo", 4, false )]
        public virtual BigInteger ValidTo { get; set; }
        [Parameter("uint256", "previousChange", 5, false )]
        public virtual BigInteger PreviousChange { get; set; }
    }

    public partial class DIDAttributeChangedEventDTO : DIDAttributeChangedEventDTOBase { }

    [Event("DIDAttributeChanged")]
    public class DIDAttributeChangedEventDTOBase : IEventDTO
    {
        [Parameter("address", "identity", 1, true )]
        public virtual string Identity { get; set; }
        [Parameter("bytes32", "name", 2, false )]
        public virtual byte[] Name { get; set; }
        [Parameter("bytes", "value", 3, false )]
        public virtual byte[] Value { get; set; }
        [Parameter("uint256", "validTo", 4, false )]
        public virtual BigInteger ValidTo { get; set; }
        [Parameter("uint256", "previousChange", 5, false )]
        public virtual BigInteger PreviousChange { get; set; }
    }



    public partial class OwnersOutputDTO : OwnersOutputDTOBase { }

    [FunctionOutput]
    public class OwnersOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class DelegatesOutputDTO : DelegatesOutputDTOBase { }

    [FunctionOutput]
    public class DelegatesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }





    public partial class ValidDelegateOutputDTO : ValidDelegateOutputDTOBase { }

    [FunctionOutput]
    public class ValidDelegateOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    public partial class NonceOutputDTO : NonceOutputDTOBase { }

    [FunctionOutput]
    public class NonceOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }





    public partial class IdentityOwnerOutputDTO : IdentityOwnerOutputDTOBase { }

    [FunctionOutput]
    public class IdentityOwnerOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }











    public partial class ChangedOutputDTO : ChangedOutputDTOBase { }

    [FunctionOutput]
    public class ChangedOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }
}
