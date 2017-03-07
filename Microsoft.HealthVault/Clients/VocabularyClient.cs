using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Vocabulary;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// Imlementation of <seealso cref="IVocabularyClient"/>
    /// </summary>
    public class VocabularyClient : IVocabularyClient
    {
        public HealthServiceConnection Connection { get; set; }

        public Guid CorrelationId { get; set; }

        public Guid LastResponseId { get; }

        public async Task<IReadOnlyCollection<VocabularyKey>> GetVocabularyKeysAsync()
        {
            return await HealthVaultPlatformVocabulary.Current.GetVocabularyKeys(this.Connection);
        }

        public async Task<Vocabulary.Vocabulary> GetVocabularyAsync(string vocabularyId, bool cultureIsFixed = false)
        {
            return (await this.GetVocabulariesAsync(new[] { vocabularyId }, cultureIsFixed).ConfigureAwait(false)).FirstOrDefault();
        }

        public async Task<IList<Vocabulary.Vocabulary>> GetVocabulariesAsync(IList<string> vocabularyKeys, bool cultureIsFixed = false)
        {
            IList<VocabularyKey> keys = new List<VocabularyKey>();
            foreach (var key in vocabularyKeys)
            {
                keys.Add(new VocabularyKey(key));
            }

            return await HealthVaultPlatformVocabulary.Current.GetVocabularyAsync(
                this.Connection,
                keys,
                cultureIsFixed).ConfigureAwait(false);
        }

        public async Task<ReadOnlyCollection<VocabularyKey>> SearchVocabularyAsync(string searchValue, VocabularySearchType searchType, int? maxResults)
        {
            return (await HealthVaultPlatformVocabulary.Current.SearchVocabularyAsync(this.Connection, null, searchValue, searchType, maxResults).ConfigureAwait(false)).MatchingKeys;
        }
    }
}