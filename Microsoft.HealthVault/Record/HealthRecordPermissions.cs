// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.ObjectModel;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Record
{
    /// <summary>
    /// Provides the permission information that the current
    /// authenticated person has for the record when using the current application.
    /// </summary>
    ///
    public class HealthRecordPermissions
    {
        /// <summary>
        /// Constructor for HealthRecordPermissions.
        /// </summary>
        public HealthRecordPermissions()
        {
            ItemTypePermissions = new Collection<ThingTypePermission>();
        }

        /// <summary>
        /// A collection of <see cref="ThingTypePermission" />(s) describing
        /// the permissions for current person record in the context of the application.
        /// </summary>
        public Collection<ThingTypePermission> ItemTypePermissions { get; private set; }

        /// <summary>
        /// Gets or sets whether the current record has opted in for
        /// Meaningful Use reporting.
        /// </summary>
        ///
        /// <remarks>
        /// If set to true, the current record has explicitly opted into Meaningful Use reporting.
        /// If set to false, the current record has explicitly opted out of Meaningful Use reporting.
        /// If no value, the current record has not explicitly opted in or out of Meaningful Use reporting.
        /// </remarks>
        public bool? MeaningfulUseOptIn { get; set; }
    }
}
