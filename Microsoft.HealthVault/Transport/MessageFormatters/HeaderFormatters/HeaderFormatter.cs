// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Transport.MessageFormatters.InfoFormatters;

namespace Microsoft.HealthVault.Transport.MessageFormatters.HeaderFormatters
{
    internal class HeaderFormatter : IMessageFormatter
    {
        private readonly HealthVaultMethods method;
        private readonly string methodVersion;
        private readonly string cultureCode;
        private readonly string version;
        private readonly Guid recordId;
        private readonly Guid targetPersonId;
        private readonly string transform;
        private readonly TimeSpan? ttl;
        private readonly IAuthSessionOrAppId authSessionOrAppId;
        private readonly InfoFormatter infoFormatter;
        private byte[] cachedContent;

        public HeaderFormatter(
            HealthVaultMethods method, 
            string methodVersion, 
            string cultureCode,
            string version,
            Guid recordId,
            Guid targetPersonId,
            string transform,
            TimeSpan? ttl,
            IAuthSessionOrAppId authSessionOrAppId,
            InfoFormatter infoFormatter)
        {
            this.method = method;
            this.methodVersion = methodVersion;
            this.cultureCode = cultureCode;
            this.version = version;
            this.recordId = recordId;
            this.targetPersonId = targetPersonId;
            this.transform = transform;
            this.ttl = ttl;
            this.authSessionOrAppId = authSessionOrAppId;
            this.infoFormatter = infoFormatter;
        }

        public void Write(XmlWriter writer)
        {
            if (this.cachedContent == null)
            {
                using (MemoryStream stream = new MemoryStream())
                using (XmlWriter innerWriter = XmlWriter.Create(stream, SDKHelper.XmlUtf8WriterSettings))
                {
                    this.WriteHeader(innerWriter);
                    innerWriter.Flush();
                    this.cachedContent = stream.ToArray();
                }
            }

            writer.WriteRaw(Encoding.UTF8.GetString(this.cachedContent));
        }

        private void WriteHeader(XmlWriter writer)
        {
            using (new TagWriter(writer, "header"))
            {
                // <method>
                writer.WriteElementString("method", this.method.ToString());

                if (this.methodVersion != null)
                {
                    // <method-version>
                    writer.WriteElementString("method-version", this.methodVersion);
                }

                if (this.targetPersonId != Guid.Empty)
                {
                    // <target-person-id>
                    writer.WriteElementString("target-person-id", this.targetPersonId.ToString());
                }

                if (this.recordId != Guid.Empty)
                {
                    // <record-id>
                    writer.WriteElementString("record-id", this.recordId.ToString());
                }

                this.authSessionOrAppId.Write(writer);

                if (this.cultureCode != null)
                {
                    writer.WriteElementString("culture-code", this.cultureCode);
                }

                if (this.transform != null)
                {
                    writer.WriteElementString("final-xsl", this.transform);
                }

                writer.WriteElementString("msg-time", SDKHelper.XmlFromNow());
                writer.WriteElementString("msg-ttl", ((int) this.ttl.GetValueOrDefault().TotalSeconds).ToString(CultureInfo.InvariantCulture));

                writer.WriteElementString("version", this.version);

                this.authSessionOrAppId.WriteHash(writer, this.infoFormatter);
            }
        }
    }
}