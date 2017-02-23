using System;
using System.Security.Cryptography;

namespace Microsoft.HealthVault.Authentication
{
    // TODO: CryptographicUnexpectedOperationException is avaiable in .NET 2.0
    // temporary fix till then with our own implementation
    public class CryptographicUnexpectedOperationException : CryptographicException
    {
        public CryptographicUnexpectedOperationException()
        {
        }

        public CryptographicUnexpectedOperationException(string message)
            : base(message)
        {
        }

        public CryptographicUnexpectedOperationException(int hr)
            : base(hr)
        {
        }

        public CryptographicUnexpectedOperationException(string format, string insert)
            : base(format, insert)
        {
        }

        public CryptographicUnexpectedOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
