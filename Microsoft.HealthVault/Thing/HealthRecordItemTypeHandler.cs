// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault
{
    internal class HealthRecordItemTypeHandler
    {
        internal HealthRecordItemTypeHandler(Type thingTypeClass)
        {
            _thingTypeClass = thingTypeClass;
        }

        internal HealthRecordItemTypeHandler(Guid typeId, Type thingTypeClass)
            : this(thingTypeClass)
        {
            _typeId = typeId;
        }

        internal Type ItemTypeClass
        {
            get { return _thingTypeClass; }
        }
        private Type _thingTypeClass;

        internal Guid TypeId
        {
            get { return _typeId; }
        }
        private Guid _typeId = Guid.Empty;
    }
}
