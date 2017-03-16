using System;
using System.Xml;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// The base HealthVault thing type
    /// </summary>
    public interface IThing
    {
        /// <summary>
        /// Writes the XML representation of the information into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer into which the contact information should be
        /// written.
        /// </param>
        void WriteXml(XmlWriter writer);

        /// <summary>
        /// Gets the type identifier for the thing type.
        /// </summary>
        ///
        /// <value>
        /// A globally unique identifier for the type of the item.
        /// </value>
        ///
        /// <remarks>
        /// The types available can be queried using
        /// <see
        /// cref="ItemTypeManager.GetHealthRecordItemTypeDefinitionAsync(System.Guid,HealthServiceConnection)"/>
        /// .
        /// </remarks>
        Guid TypeId { get; }

        /// <summary>
        /// Gets the key of the thing.
        /// </summary>
        ///
        /// <value>
        /// A globally unique identifier for the item issued when the item is
        /// created and a globally unique version stamp updated every time
        /// the item is changed.
        /// </value>
        ///
        /// <remarks>
        /// This is the only property that
        /// is guaranteed to be available regardless of how
        /// <see cref="ThingBase.Sections"/> is set.
        /// </remarks>
        ///
        ThingKey Key { get; }
    }
}
