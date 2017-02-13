// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Specialized;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Defines a set of health record items for authorization 
    /// purposes whose user tags are within a specified set.
    /// </summary>
    /// 
    /// <remarks>
    /// Permissions on data in a person's health records are always included
    /// in an authorization set (whether implicitly via their type or 
    /// effective date, or explicitly by setting the system set.) This class
    /// serves as a set of health record items that have effective dates 
    /// falling within the specified range. Other types of authorization 
    /// sets include:
    /// <see cref="TypeIdSetDefinition"/>.
    /// <see cref="DateRangeSetDefinition"/>.
    /// </remarks>
    /// 
    /// <seealso cref="AuthorizationSetDefinition"/>
    /// <seealso cref="TypeIdSetDefinition"/>
    /// <seealso cref="DateRangeSetDefinition"/>
    /// 
    [Obsolete("This class will soon be removed from the SDK. " +
              "Please use HealthRecordItem.Flags to restrict item access or " +
              "HealthRecordItem.Tags to use user tags.")]
    public class UserTagSetDefinition : AuthorizationSetDefinition
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UserTagSetDefinition"/> 
        /// class with the specified user tags.
        /// </summary>
        /// 
        /// <param name="userTags">
        /// A string with comma-separated user tags.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="userTags" /> parameter is <b>null</b> or does 
        /// not contain at least one valid user tag.
        /// </exception>
        /// 
        public UserTagSetDefinition(string userTags)
            : base(SetType.UserTagSet)
        {
            _userTags = MakeUserTagList(userTags);

            Validator.ThrowArgumentExceptionIf(
                _userTags == null,
                "userTags",
                "UserTagSetEmpty");
        }

        /// <summary>
        /// Creates a name value collection of user tags from an input string.
        /// </summary>
        /// 
        /// <param name="allUserTags"></param>
        /// 
        /// <returns>
        /// A <see cref="NameValueCollection"/> whose keys and values 
        /// are both the unique user tags in the string. If the string is <b>null</b> 
        /// or empty or if there are no user tags, then <b>null</b> is returned.
        /// </returns>
        /// 
        internal static NameValueCollection MakeUserTagList(
            string allUserTags)
        {
            if (String.IsNullOrEmpty(allUserTags)) return null;

            string[] inputUserTags =
                allUserTags.Split(
                    new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries);

            NameValueCollection userTags =
                new NameValueCollection(inputUserTags.Length);

            for (int i = inputUserTags.Length - 1; i >= 0; --i)
            {
                string trimmedUserTag = inputUserTags[i].Trim();
                if (trimmedUserTag.Length > 0
                    && userTags[trimmedUserTag] == null)
                {
                    userTags.Add(trimmedUserTag, trimmedUserTag);
                }
            }

            return userTags.Count > 0 ? userTags : null;
        }

        /// <summary>
        /// Gets the user tags of the health record items in this set.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="NameValueCollection"/> of user tags.
        /// </value>
        /// 
        /// <remarks>
        /// The keys and values in the collection are identical.
        /// </remarks>
        /// 
        internal NameValueCollection UserTags
        {
            get { return _userTags; }
        }
        private NameValueCollection _userTags;

        /// <summary>
        /// Gets the XML representation of the set.
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
        /// <exception cref="InvalidOperationException">
        /// If no user tags are specified in the <see cref="UserTags"/> property.
        /// </exception>
        /// 
        public override string GetXml()
        {
            Validator.ThrowInvalidIf(UserTags.Count == 0, "UserTagSetEmpty");

            return
                "<tags>" +
                String.Join(",", UserTags.AllKeys) +
                "</tags>";
        }
    }

}
