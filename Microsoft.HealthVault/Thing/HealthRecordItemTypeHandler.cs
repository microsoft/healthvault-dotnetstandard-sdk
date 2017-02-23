// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Thing
{
    internal class HealthRecordItemTypeHandler
    {
        internal HealthRecordItemTypeHandler(Type thingTypeClass)
        {
            this.ItemTypeClass = thingTypeClass;
        }

        internal HealthRecordItemTypeHandler(Guid typeId, Type thingTypeClass)
            : this(thingTypeClass)
        {
            this.TypeId = typeId;
        }

        internal Type ItemTypeClass { get; }

        internal Guid TypeId { get; } = Guid.Empty;
    }
}
