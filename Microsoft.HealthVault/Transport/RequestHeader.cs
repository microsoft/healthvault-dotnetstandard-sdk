// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.HealthVault.Transport
{
    internal class RequestHeader
    {
        public string Method { get; set; }

        public int MethodVersion { get; set; }

        public string TargetPersonId { get; set; }

        public string RecordId { get; set; }

        public string AppId { get; set; }

        public AuthSession AuthSession { get; set; }

        public string CultureCode { get; set; }

        public string MessageTime { get; set; }

        public int MessageTtl { get; set; }

        public string Version { get; set; }

        public InfoHash InfoHash { get; set; }

        public bool HasAuthSession => AuthSession != null;

        public bool HasOfflinePersonInfo => HasAuthSession && AuthSession.Person != null;

        public bool HasUserAuthToken => HasAuthSession && !string.IsNullOrEmpty(AuthSession.UserAuthToken);
    }
}
