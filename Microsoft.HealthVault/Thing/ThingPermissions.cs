// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Provides values that indicate the rights associated with access to a
    /// thing.
    /// </summary>
    ///
    [Flags]
    public enum ThingPermissions
    {
        /// <summary>
        /// The person or group has no permissions.
        /// This is not the same as denial of permissions. It means that the
        /// rule is not currently granting any permissions.
        /// </summary>
        ///
        None = 0x0,

        /// <summary>
        /// The person or group has read access to the set of health record
        /// items.
        /// </summary>
        ///
        Read = 0x1,

        /// <summary>
        /// The person or group has update access to the set of health record
        /// items.
        /// </summary>
        ///
        Update = 0x2,

        /// <summary>
        /// The person or group can create things in the set.
        /// </summary>
        ///
        Create = 0x4,

        /// <summary>
        /// The person or group can delete things in the set.
        /// </summary>
        ///
        Delete = 0x8,

        /// <summary>
        /// The person or group has all permissions on the set.
        /// </summary>
        ///
        All = Read | Update | Create | Delete
    }
}
