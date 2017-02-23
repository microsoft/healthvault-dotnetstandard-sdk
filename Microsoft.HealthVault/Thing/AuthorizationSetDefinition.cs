// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Defines the interface for interacting with sets of health record items
    /// for authorization purposes. This class is abstract.
    /// </summary>
    ///
    /// <remarks>
    /// Permissions on data in a person's health records are always included
    /// in an authorization set (whether implicitly via their type or
    /// effective date, or explicitly by setting the system set).
    /// <see cref="AuthorizationSetDefinition"/> is the base class for the
    /// different types of authorization sets.
    /// The concrete class definitions that define the sets are
    /// <see cref="DateRangeSetDefinition"/> and <see cref="TypeIdSetDefinition"/>.
    /// </remarks>
    ///
    /// <seealso cref="DateRangeSetDefinition"/>
    /// <seealso cref="TypeIdSetDefinition"/>
    ///
    public abstract class AuthorizationSetDefinition
    {
        internal AuthorizationSetDefinition(SetType setType)
        {
            this.SetType = setType;
        }

        internal SetType SetType { get; }

        /// <summary>
        /// Retrieves the XML representation of the set. This method is abstract.
        /// </summary>
        ///
        /// <returns>
        /// The XML representation of the set as a string.
        /// </returns>
        ///
        /// <remarks>
        /// The XML representation adheres to the schema required by the
        /// HealthVault methods.
        /// </remarks>
        ///
        public abstract string GetXml();

        /// <summary>
        /// Retrieves the XML representation of the set as a string.
        /// </summary>
        ///
        /// <returns>
        /// A string containing the XML representation of the set.
        /// </returns>
        ///
        /// <remarks>
        /// The XML representation adheres to the schema required by the
        /// HealthVault methods.
        /// </remarks>
        ///
        public override string ToString()
        {
            return this.GetXml();
        }
    }
}
