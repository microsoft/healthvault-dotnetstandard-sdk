// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Vocabulary
{
    /// <summary>
    /// The set of search parameters are used with the Vocabulary Search feature to specify the
    /// vocabulary etc.
    /// </summary>
    public class VocabularySearchParameters
    {
        /// <summary>
        /// Creates a vocabulary search parameter set with the <see cref="VocabularyKey"/> that is
        /// used to identify the vocabulary to search.
        /// </summary>
        /// <param name="vocabulary">
        /// A key to identify the vocabulary that is searched.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="vocabulary"/> is <b>null</b>.
        /// </exception>
        public VocabularySearchParameters(VocabularyKey vocabulary)
        {
            Validator.ThrowIfArgumentNull(vocabulary, "vocabulary", "VocabularyKeyNullOrEmpty");
            this.Vocabulary = vocabulary;
        }

        /// <summary>
        /// Gets the vocabulary key used to identify the vocabulary to be searched.
        /// </summary>
        public VocabularyKey Vocabulary { get; }

        /// <summary>
        /// Gets or sets the culture in which the vocabulary will be searched.
        /// </summary>
        /// <remarks>
        /// If the culture is not set, the current UI culture will be used by default.
        /// </remarks>
        public CultureInfo Culture
        {
            get
            {
                return this.culture ?? CultureInfo.CurrentUICulture;
            }

            set { this.culture = value; }
        }

        private CultureInfo culture;

        /// <summary>
        /// Gets or sets the maximum number of results to be returned from the search.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is a negative number.
        /// </exception>
        public int? MaxResults
        {
            get { return this.maxResults; }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value < 0,
                    "MaxResults",
                    "SearchMaxResultsInvalid");
                this.maxResults = value;
            }
        }

        private int? maxResults;
    }
}