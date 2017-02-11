// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.Health
{
    /// <summary>
    /// The section that will be or were retrieved when accessing 
    /// a <see cref="HealthRecordItem"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// To enable efficient use of the network, the HealthVault service  
    /// retrieves only those portions of a <see cref="HealthRecordItem"/> that 
    /// the application needs.
    /// These sections are identified in the <see cref="HealthRecordView"/> 
    /// when performing a search, and are specified on a <see cref="HealthRecordItem"/> 
    /// when the item is retrieved.
    /// When <see cref="HealthRecordAccessor.UpdateItems"/> is called, 
    /// only specified sections are updated.
    /// </remarks>
    /// 
    [Flags]
    public enum HealthRecordItemSections
    {
        /// <summary>
        /// No sections are retrieved or represented in the 
        /// <see cref="Microsoft.Health.HealthRecordItem"/> object.
        /// </summary>
        /// 
        None = 0x0,

        /// <summary>
        /// The Core section of the <see cref="Microsoft.Health.HealthRecordItem"/> 
        /// is retrieved.
        /// </summary>
        /// 
        Core = 0x1,

        /// <summary>
        /// The Audits section of the <see cref="Microsoft.Health.HealthRecordItem"/> 
        /// is retrieved.
        /// </summary>
        /// 
        Audits = 0x2,

        /// <summary>
        /// The EffectivePermissions section is retrieved, showing the 
        /// permission granted to the caller of the <see cref="Microsoft.Health.HealthRecordItem"/>.
        /// </summary>
        /// 
        EffectivePermissions = 0x4,

        /// <summary>
        /// The BlobPayload section of the <see cref="Microsoft.Health.HealthRecordItem"/> 
        /// is retrieved.
        /// </summary>
        /// 
        BlobPayload = 0x8,

        /// <summary>
        /// The Signature section of the <see cref="Microsoft.Health.HealthRecordItem"/> 
        /// is retrieved.
        /// </summary>
        /// 
        Signature = 0x10,

        /// <summary>
        /// The Tags section of the <see cref="Microsoft.Health.HealthRecordItem"/> 
        /// is retrieved.
        /// </summary>
        /// 
        Tags = 0x20,

        /// <summary>
        /// Gets the base XML section.
        /// </summary>
        /// 
        /// <private>
        /// This is here even though it is not exposed by HealthVault
        /// to make it easier for SDK developers to use.
        /// </private>
        /// 
        Xml = 0x1000000,

        /// <summary>
        /// All sections of the <see cref="Microsoft.Health.HealthRecordItem"/> 
        /// are retrieved.
        /// </summary>
        /// 
        All = Core | Audits | EffectivePermissions | BlobPayload | Xml | Signature | Tags,

        /// <summary>
        /// Default sections <see cref="Microsoft.Health.HealthRecordItem"/> 
        /// are retrieved. 
        /// </summary>
        /// 
        Default = Core | Xml,
    }
}
