// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography;
using System.Text;

namespace SimpleIdServer.IdServer.Domains;

public class ClientSecret
{
    private static Dictionary<HashAlgs, int> _mappingAlgToSize = new Dictionary<HashAlgs, int>
    {
        { HashAlgs.MD5, MD5.HashSizeInBytes },
        { HashAlgs.SHA1, SHA1.HashSizeInBytes },
        { HashAlgs.SHA256, SHA256.HashSizeInBytes },
        { HashAlgs.SHA384, SHA384.HashSizeInBytes },
        { HashAlgs.SHA512, SHA512.HashSizeInBytes }
    };

    private static (HashAlgs Alg, Func<string, string> HashFunc)[] _hashAlgs = new (HashAlgs Alg, Func<string, string> HashFunc)[]
    {
        (HashAlgs.SHA1, (string pwd) => Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(pwd)))),
        (HashAlgs.SHA256, (string pwd) => Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(pwd)))),
        (HashAlgs.SHA384, (string pwd) => Convert.ToBase64String(SHA384.Create().ComputeHash(Encoding.UTF8.GetBytes(pwd)))),
        (HashAlgs.SHA512, (string pwd) => Convert.ToBase64String(SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(pwd)))),
        (HashAlgs.MD5, (string pwd) => Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(pwd)))),
        (HashAlgs.PLAINTEXT, (string pwd) => pwd)
    };

    public string Id
    {
        get; set;
    }

    public string Value
    {
        get; set;
    }

    public HashAlgs Alg
    {
        get; set;
    }

    public DateTime? ExpirationDateTime
    {
        get; set;
    }

    public DateTime CreateDateTime
    {
        get; set;
    }

    public bool IsActive
    {
        get; set;
    } = true;

    public bool IsExpired
    {
        get
        {
            if (ExpirationDateTime.HasValue == false) return false;
            return ExpirationDateTime.Value < DateTime.UtcNow && IsActive;
        }
    }

    public static ClientSecret Create(string pwd, HashAlgs alg, DateTime? expirationTime = null)
    {
        var hashedPwd = _hashAlgs.Single((a) => a.Alg == alg).HashFunc(pwd);
        return new ClientSecret
        {
            Id = Guid.NewGuid().ToString(),
            Alg = alg,
            Value = hashedPwd,
            CreateDateTime = DateTime.UtcNow,
            IsActive = true,
            ExpirationDateTime = expirationTime
        };
    }
    
    public static ClientSecret Resolve(string str)
    {
        var alg = HashAlgs.PLAINTEXT;
        if (TryDecodeBase64(str, out byte[] payload))
        {
            var size = payload.Length;
            if (_mappingAlgToSize.ContainsValue(size))
            {
                alg = _mappingAlgToSize.Single(kvp => kvp.Value == size).Key;
            }

        }

        return new ClientSecret
        {
            Id = Guid.NewGuid().ToString(),
            Alg = alg,
            Value = str,
            CreateDateTime = DateTime.UtcNow
        };
    }

    public static bool TryDecodeBase64(string s, out byte[] result)
    {
        if (s.Length % 4 != 0)
        {
            result = null;
            return false;
        }

        try
        {
            result = Convert.FromBase64String(s);
            return true;
        }
        catch (FormatException)
        {
            result = null;
            return false;
        }
    }
}


public enum HashAlgs
{
    PLAINTEXT = 0,
    MD5 = 1,
    SHA1 = 2,
    SHA256 = 3,
    SHA384 = 4,
    SHA512 = 5
}