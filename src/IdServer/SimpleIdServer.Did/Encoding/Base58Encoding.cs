// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Helpers;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace SimpleIdServer.Did.Encoding
{
    // Implements https://en.bitcoin.it/wiki/Base58Check_encoding
    public static class Base58Encoding
    {
        public const int CheckSumSizeInBytes = 4;

        public static byte[] AddCheckSum(byte[] data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<byte[]>().Length == data.Length + CheckSumSizeInBytes);
            byte[] checkSum = GetCheckSum(data);
            byte[] dataWithCheckSum = ArrayHelpers.ConcatArrays(data, checkSum);
            return dataWithCheckSum;
        }

        //Returns null if the checksum is invalid
        public static byte[] VerifyAndRemoveCheckSum(byte[] data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<byte[]>() == null || Contract.Result<byte[]>().Length + CheckSumSizeInBytes == data.Length);
            byte[] result = ArrayHelpers.SubArray(data, 0, data.Length - CheckSumSizeInBytes);
            byte[] givenCheckSum = ArrayHelpers.SubArray(data, data.Length - CheckSumSizeInBytes);
            byte[] correctCheckSum = GetCheckSum(result);
            if (givenCheckSum.SequenceEqual(correctCheckSum))
                return result;
            else
                return null;
        }

        private const string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static string Encode(byte[] data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<string>() != null);

            // Decode byte[] to BigInteger
            BigInteger intData = 0;
            for (int i = 0; i < data.Length; i++)
            {
                intData = intData * 256 + data[i];
            }

            // Encode BigInteger to Base58 string
            string result = "";
            while (intData > 0)
            {
                int remainder = (int)(intData % 58);
                intData /= 58;
                result = Digits[remainder] + result;
            }

            // Append `1` for each leading 0 byte
            for (int i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }
            return result;
        }

        public static string EncodeWithCheckSum(byte[] data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return Encode(AddCheckSum(data));
        }

        public static byte[] Decode(string s)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Ensures(Contract.Result<byte[]>() != null);

            // Decode Base58 string to BigInteger 
            BigInteger intData = 0;
            for (int i = 0; i < s.Length; i++)
            {
                int digit = Digits.IndexOf(s[i]); //Slow
                if (digit < 0)
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", s[i], i));
                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            // Leading zero bytes get encoded as leading `1` characters
            int leadingZeroCount = s.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
                intData.ToByteArray()
                .Reverse()// to big endian
                .SkipWhile(b => b == 0);//strip sign byte
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();
            return result;
        }

        // Throws `FormatException` if s is not a valid Base58 string, or the checksum is invalid
        public static byte[] DecodeWithCheckSum(string s)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Ensures(Contract.Result<byte[]>() != null);
            var dataWithCheckSum = Decode(s);
            var dataWithoutCheckSum = VerifyAndRemoveCheckSum(dataWithCheckSum);
            if (dataWithoutCheckSum == null)
                throw new FormatException("Base58 checksum is invalid");
            return dataWithoutCheckSum;
        }

        private static byte[] GetCheckSum(byte[] data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<byte[]>() != null);

            SHA256 sha256 = new SHA256Managed();
            byte[] hash1 = sha256.ComputeHash(data);
            byte[] hash2 = sha256.ComputeHash(hash1);

            var result = new byte[CheckSumSizeInBytes];
            Buffer.BlockCopy(hash2, 0, result, 0, result.Length);

            return result;
        }
    }
}
