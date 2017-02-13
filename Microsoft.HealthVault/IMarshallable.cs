// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Xml;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Defines the HealthVault interface used to perform versioned 
    /// serialization of objects.
    /// </summary>
    /// 
    public interface IMarshallable
    {
        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter that receives the object data.
        /// </param>
        /// 
        void Marshal(XmlWriter writer);

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// 
        /// <exception cref="IncompatibleVersionException">
        /// The serialized data version cannot be deserialized due to version 
        /// incompatibilities.
        /// </exception>
        /// 
        /// <param name="reader">
        /// The XmlReader from which to deserialize the 
        /// object data from.
        /// </param>
        /// 
        void Unmarshal(XmlReader reader);
    }
}