// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Xml;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Transport.MessageFormatters.AuthenticationFormatters;
using Microsoft.HealthVault.Transport.MessageFormatters.HeaderFormatters;
using Microsoft.HealthVault.Transport.MessageFormatters.InfoFormatters;

namespace Microsoft.HealthVault.Transport.MessageFormatters.SessionFormatters
{
    internal abstract class SessionFormatter : IAuthSessionOrAppId
    {
        private readonly string sessionCredentialToken;

        protected SessionFormatter(string sessionCredentialToken)
        {
            this.sessionCredentialToken = sessionCredentialToken;
        }

        public virtual void Write(XmlWriter writer)
        {
            writer.WriteStartElement("auth-session");
            writer.WriteElementString("auth-token", this.sessionCredentialToken);

            this.WriteValue(writer);

            writer.WriteEndElement();
        }

        public abstract void WriteValue(XmlWriter writer);

        public virtual void WriteHash(XmlWriter writer, InfoFormatter info)
        {
            CryptoData data = Cryptographer.Hash(info.AsBytes());
            CryptoDataFormatter formatter = new CryptoDataFormatter(data);
            using (new TagWriter(writer, "info-hash"))
            {
                formatter.Write(writer);
            }
        }
    }
}