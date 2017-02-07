// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates action plan 
    /// related object.
    /// </summary>
    public class ActionPlanWrapper : MshItemBase
    {
        #region ctor
        /// <summary>
        /// Creates a new instance of the <see cref="ActionPlanWrapper"/> 
        /// class with default values
        /// </summary>
        public ActionPlanWrapper()
            : this(null)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ActionPlanWrapper"/> class
        /// specifying wrapped object.
        /// </summary>
        /// <param name="wrappedInstance">wrapped instance</param>
        public ActionPlanWrapper(IMshItem wrappedInstance)
            : base(wrappedInstance, TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ActionPlanWrapper"/> class
        /// specifying wrapped object.
        /// </summary>
        /// <param name="wrappedTypeName">Wrapped type name</param>
        /// <param name="base64EncodedJson">Base64-encoded JSON</param>
        public ActionPlanWrapper(
            string wrappedTypeName, 
            string base64EncodedJson)
            : base(TypeId, wrappedTypeName, base64EncodedJson, Guid.Empty, Guid.Empty)
        {
        }
        #endregion

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        /// <value>
        /// A GUID.
        /// </value>
        public static new readonly Guid TypeId =
            new Guid("10291BCD-6C33-4297-86DF-167FEDEFD6D8");

        /// <summary>
        /// Root element name within thing-xml.
        /// </summary>
        protected override string RootElementName
        {
            get { return "action-plan"; }
        }
    }
}
