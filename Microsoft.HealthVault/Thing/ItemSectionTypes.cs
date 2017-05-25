// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// (c) Microsoft. All rights reserved

using System;

namespace Microsoft.HealthVault.Thing
{
    public static class ItemSections
    {
        public static string Core => "core";

        public static string Audits => "audits";

        public static string Blobs => "blobpayload";

        public static string Tags => "tags";

        public static string Permissions => "effectivepermissions";

        public static string Signatures => "digitalsignatures";
    }

    [Flags]
    public enum ItemSectionTypes
    {
        None = 0,

        // Return Data
        Data = 0x01,

        // Standard Item Type information
        Core = 0x02,
        Audits = 0x04,
        Tags = 0x08,
        Blobs = 0x10,
        EffectivePermissions = 0x20,
        Signatures = 0x40,

        // What people typically want to retrieve
        Standard = Data | Core,

        // Everything
        All = Data | Core | Audits | Tags | Blobs | EffectivePermissions | Signatures
    }
}