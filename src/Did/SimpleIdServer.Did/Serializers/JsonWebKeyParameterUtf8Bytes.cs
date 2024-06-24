using System;

namespace SimpleIdServer.Did.Serializers;

public readonly struct JsonWebKeyParameterUtf8Bytes
{
    public static ReadOnlySpan<byte> Alg => "alg"u8;

    public static ReadOnlySpan<byte> Crv => "crv"u8;

    public static ReadOnlySpan<byte> D => "d"u8;

    public static ReadOnlySpan<byte> DP => "dp"u8;

    public static ReadOnlySpan<byte> DQ => "dq"u8;

    public static ReadOnlySpan<byte> E => "e"u8;

    public static ReadOnlySpan<byte> K => "k"u8;

    public static ReadOnlySpan<byte> KeyOps => "key_ops"u8;

    public static ReadOnlySpan<byte> Keys => "keys"u8;

    public static ReadOnlySpan<byte> Kid => "kid"u8;

    public static ReadOnlySpan<byte> Kty => "kty"u8;

    public static ReadOnlySpan<byte> N => "n"u8;

    public static ReadOnlySpan<byte> Oth => "oth"u8;

    public static ReadOnlySpan<byte> P => "p"u8;

    public static ReadOnlySpan<byte> Q => "q"u8;

    public static ReadOnlySpan<byte> QI => "qi"u8;

    public static ReadOnlySpan<byte> Use => "use"u8;

    public static ReadOnlySpan<byte> X5c => "x5c"u8;

    public static ReadOnlySpan<byte> X5t => "x5t"u8;

    public static ReadOnlySpan<byte> X5tS256 => "x5t#S256"u8;

    public static ReadOnlySpan<byte> X5u => "x5u"u8;

    public static ReadOnlySpan<byte> X => "x"u8;

    public static ReadOnlySpan<byte> Y => "y"u8;
}
