// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


namespace Microsoft.HealthVault
{
    /// <summary>
    /// Defines the interpersonal relationship between the authorized
    /// person and the person to whom the record belongs.
    /// </summary>
    /// 
    public enum RelationshipType
    {
        /// <summary>
        /// The relationship is unknown.
        /// </summary>
        /// 
        /// <remarks>
        /// This value is only used to mark the unknown state in the
        /// <see cref="HealthRecordInfo"/> class when data has not been retrieved
        /// from the HealthVault service. Once data has been retrieved, 
        /// this state is never used.
        /// </remarks>
        /// 
        Unknown = 0,

        /// <summary>
        /// The record is for the logged-in person.
        /// </summary>
        /// 
        Self = 1,

        /// <summary>
        /// The record is for a person other than the logged-in person.
        /// </summary>
        /// 
        /// <remarks>
        /// This is generally used only for cases in which the other 
        /// relationships in the enumeration do not apply.
        /// </remarks>
        /// 
        Other = 2,

        /// <summary>
        /// The record is for the spouse of the logged-in person.
        /// </summary>
        /// 
        Spouse = 3,

        /// <summary>
        /// The record is for a child of the logged-in person.
        /// </summary>
        /// 
        Child = 5,

        /// <summary>
        /// The record is for a guardian of the logged-in person.
        /// </summary>
        /// 
        Guardian = 6,

        /// <summary>
        /// The record is for a patient of the logged-in person.
        /// </summary>
        /// 
        Patient = 7,

        /// <summary>
        /// The record is for a parent of the logged-in person.
        /// </summary>
        /// 
        Parent = 8,

        ///<summary>
        ///The record is for an application.
        ///</summary>
        ///
        Application = 9,

        ///<summary>
        /// The record is for a relative of the logged-in person.
        /// </summary>
        /// 
        Relative = 10,

        ///<summary>
        /// The record is for a domestic partner of the logged-in person.
        /// </summary>
        /// 
        DomesticPartner = 11,

        ///<summary>
        /// The record is for a pet of the logged-in person.
        /// </summary>
        Pet = 13,

        ///<summary>
        /// The record is for a son of the logged-in person
        /// </summary>
        Son = 14,

        ///<summary>
        /// The reocrd is for a daughter of the logged-in person
        /// </summary>
        Daughter = 15,
    }
}
