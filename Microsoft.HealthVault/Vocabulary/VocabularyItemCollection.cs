// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.HealthVault.Vocabulary
{
    /// <summary>
    /// A collection of vocabulary items belonging to a particular vocabulary.
    /// </summary>
    ///
    public class VocabularyItemCollection : Vocabulary
    {
        internal VocabularyItemCollection()
        {
            _orderedItemsList = new List<VocabularyItem>(10);
        }

        /// <summary>
        /// Retrieves the vocabulary items in the intended order for this instance.
        /// </summary>
        ///
        public ReadOnlyCollection<VocabularyItem> OrderedValues => new ReadOnlyCollection<VocabularyItem>(_orderedItemsList);

        /// <summary>
        /// Retrieves the vocabulary item at the specified index based on the intended order
        /// for this instance.
        /// </summary>
        ///
        /// <param name="index">The index of the vocabulary item to retrieve</param>
        ///
        /// <returns>The vocabulary item at the requested index</returns>
        ///
        public VocabularyItem this[int index] => _orderedItemsList[index];

        internal override void AddVocabularyItem(string key, VocabularyItem item)
        {
            base.AddVocabularyItem(key, item);
            _orderedItemsList.Add(item);
        }

        private readonly List<VocabularyItem> _orderedItemsList;
    }
}
