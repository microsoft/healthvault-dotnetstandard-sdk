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