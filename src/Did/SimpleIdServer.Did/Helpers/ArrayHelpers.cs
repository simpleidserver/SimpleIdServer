// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace SimpleIdServer.Did.Helpers
{
    public class ArrayHelpers
    {
        public static T[] ConcatArrays<T>(params T[][] arrays)
        {
            Contract.Requires(arrays != null);
            Contract.Requires(Contract.ForAll(arrays, (arr) => arr != null));
            Contract.Ensures(Contract.Result<T[]>() != null);
            Contract.Ensures(Contract.Result<T[]>().Length == arrays.Sum(arr => arr.Length));

            var result = new T[arrays.Sum(arr => arr.Length)];
            int offset = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                var arr = arrays[i];
                Buffer.BlockCopy(arr, 0, result, offset, arr.Length);
                offset += arr.Length;
            }
            return result;
        }

        public static T[] ConcatArrays<T>(T[] arr1, T[] arr2)
        {
            Contract.Requires(arr1 != null);
            Contract.Requires(arr2 != null);
            Contract.Ensures(Contract.Result<T[]>() != null);
            Contract.Ensures(Contract.Result<T[]>().Length == arr1.Length + arr2.Length);

            var result = new T[arr1.Length + arr2.Length];
            Buffer.BlockCopy(arr1, 0, result, 0, arr1.Length);
            Buffer.BlockCopy(arr2, 0, result, arr1.Length, arr2.Length);
            return result;
        }

        public static T[] SubArray<T>(T[] arr, int start, int length)
        {
            Contract.Requires(arr != null);
            Contract.Requires(start >= 0);
            Contract.Requires(length >= 0);
            Contract.Requires(start + length <= arr.Length);
            Contract.Ensures(Contract.Result<T[]>() != null);
            Contract.Ensures(Contract.Result<T[]>().Length == length);

            var result = new T[length];
            Buffer.BlockCopy(arr, start, result, 0, length);
            return result;
        }

        public static T[] SubArray<T>(T[] arr, int start)
        {
            Contract.Requires(arr != null);
            Contract.Requires(start >= 0);
            Contract.Requires(start <= arr.Length);
            Contract.Ensures(Contract.Result<T[]>() != null);
            Contract.Ensures(Contract.Result<T[]>().Length == arr.Length - start);

            return SubArray(arr, start, arr.Length - start);
        }
    }
}
