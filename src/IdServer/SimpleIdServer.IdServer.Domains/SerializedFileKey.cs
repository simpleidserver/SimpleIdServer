// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class SerializedFileKey
    {
        public string Id { get; set; } = null!;
        public string KeyId { get; set; } = null!;
        public string Usage { get; set; } = null!;
        public string Alg { get; set; } = null!;
        public string? Enc { get; set; }
        public string? PublicKeyPem { get; set; } = null;
        public string? PrivateKeyPem { get; set; } = null;
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public bool IsSymmetric { get; set; } = false;
        public byte[]? Key { get; set; } = null;
        public ICollection<Realm> Realms { get; set; } = new List<Realm>();
    }
}
