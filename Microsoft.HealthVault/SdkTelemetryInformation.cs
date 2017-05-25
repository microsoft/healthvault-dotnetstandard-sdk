// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Telemetry information that will be logged from clients
    /// using the SDK as part of the method call to HealthVault Platform <see cref="HealthServiceMessage.BuildRequestXml()"/>
    /// </summary>
    /// <remarks>
    /// Request from Clients are of format - {Category} / {FileVersion} {OsInformation}
    ///
    /// Category values are specified in SdkTelemetryInformationCategories
    ///
    /// For example Xamarin based android telemetry information will be:
    ///
    /// HV-Xamarin-Android / 1.0.0.0 Android 7.1
    ///
    /// </remarks>
    internal class SdkTelemetryInformation
    {
        /// <summary>
        /// Category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Assembly file version
        /// </summary>
        public string FileVersion { get; set; }

        /// <summary>
        /// Operating system information
        /// </summary>
        public string OsInformation { get; set; }
    }
}
