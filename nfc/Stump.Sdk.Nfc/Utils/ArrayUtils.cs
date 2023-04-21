using System;

namespace Stump.Sdk.Nfc.Utils
{
    public static class ArrayUtls
    {
        public static byte[] XOR(this byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
                throw new ArgumentException("Array 1 and Array 2 must be the same length.");

            byte[] result = new byte[arr1.Length];

            for (int i = 0; i < arr1.Length; ++i)
                result[i] = (byte)(arr1[i] ^ arr2[i]);

            return result;
        }
    }
}
