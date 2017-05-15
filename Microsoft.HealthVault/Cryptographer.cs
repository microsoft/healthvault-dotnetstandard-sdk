// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Security.Cryptography;
using Microsoft.HealthVault.Connection;

namespace Microsoft.HealthVault
{
    internal class Cryptographer : ICryptographer
    {
        CryptoData ICryptographer.Hmac(string keyMaterial, byte[] data)
        {
            return Hmac(keyMaterial, data);
        }

        CryptoData ICryptographer.Hash(byte[] data)
        {
            return Hash(data);
        }

        public static CryptoData Hash(byte[] data)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(data);

                CryptoData cryptoData = new CryptoData
                {
                    Algorithm = HealthVaultConstants.Cryptography.HashAlgorithm,
                    Value = Convert.ToBase64String(hash)
                };

                return cryptoData;
            }
        }

        public static CryptoData Hmac(string keyMaterial, byte[] data)
        {
            using (HMAC hmac = new HMACSHA256(Convert.FromBase64String(keyMaterial)))
            {
                byte[] hash = hmac.ComputeHash(data);

                CryptoData cryptoData = new CryptoData
                {
                    Algorithm = HealthVaultConstants.Cryptography.HmacAlgorithm,
                    Value = Convert.ToBase64String(hash)
                };

                return cryptoData;
            }
        }
    }
}
