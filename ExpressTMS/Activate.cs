using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace ExpressTMS
{
    internal class SerialKey
    {
        internal string _KeyPartA;
        internal string _KeyPartB;
        internal string _KeyPartC;
        internal string _KeyPartD;
        internal string _BusinessName;
        internal string _KeyHashCode;
    }

    internal static class ValidateSerialKey
    {
        internal static string HexEncode(string str)
        {
            byte[] val = Encoding.UTF8.GetBytes(str);
            return Encoding.UTF8.GetString(Hex.Encode(val));
        }

        internal static string HexDecode(string str)
        {
            byte[] val = Encoding.UTF8.GetBytes(str);
            return Encoding.UTF8.GetString(Hex.Decode(val));
        }

        internal static bool ValidateSK(SerialKey key)
        {
            string HashCode = ComputerSKHash(key._KeyPartA, key._KeyPartB, key._KeyPartC, key._KeyPartD, key._BusinessName);
            if((!string.IsNullOrEmpty(HashCode)) && key._KeyHashCode == HashCode)
                return true;
            return false;
        }

        internal static string ComputerSKHash(string KeyPart1, string KeyPart2, string KeyPart3, string KeyPart4, string BusinessName)
        {
            try
            {
                string SerialKeyString = string.Format("EXPRESSTMS_GLOBAL(({0})({1})({2})({3}));", KeyPart1, KeyPart2, KeyPart3, KeyPart4, BusinessName);
                IDigest hash = new Sha1Digest();
                byte[] ciperTextBytes = new byte[hash.GetDigestSize()];
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(SerialKeyString);
                hash.BlockUpdate(plainTextBytes, 0, plainTextBytes.Length);
                hash.DoFinal(ciperTextBytes, 0);

                string KeyHash = Convert.ToBase64String(Hex.Encode(ciperTextBytes));
                return KeyHash;
            }
            catch (System.Exception) { }
            return null;
        }
    }
}
