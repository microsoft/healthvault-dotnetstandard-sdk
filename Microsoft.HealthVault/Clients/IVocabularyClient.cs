using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Vocabulary;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// An interface for the HealthVault vocabulary client. Used to access vocabularies.
    /// </summary>
    public interface IVocabularyClient : IClient
    {
        /// <summary>
        /// Retrieves a collection of key information for identifying and 
        /// describing the vocabularies in the system.
        /// </summary>
        /// 
        /// <returns>
        /// A collection of keys identifying the vocabularies in the system.
        /// </returns>
        Task<IReadOnlyCollection<VocabularyKey>> GetVocabularyKeysAsync();

        /// <summary>
        /// Retrieves lists of vocabulary items for the specified vocabulary in the user's current culture.
        /// </summary>
        /// <param name="vocabularyId">the string representing the vocabulary to fetch</param>
        /// <param name="cultureIsFixed">
        /// HealthVault looks for the vocabulary items for the culture info
        /// specified by the system.
        /// If <paramref name="cultureIsFixed"/> is set to <b>false</b> and if
        /// items are not found for the specified culture, items for the
        /// default fallback culture are returned. If
        /// <paramref name="cultureIsFixed"/> is set to <b>true</b>,
        /// fallback will not occur, and if items are not found for the
        /// specified culture, empty strings are returned.
        /// </param>
        /// <returns>
        /// The specified vocabulary and its items, or empty strings.
        /// </returns>
        /// <exception cref="HealthServiceException">
        /// There is an error in the server request.
        /// <br></br>
        /// -Or- 
        /// <br></br>
        /// The requested vocabulary is not found on the server.
        /// <br></br>
        /// -Or- 
        /// <br></br>
        /// The requested vocabulary does not contain representations 
        /// for its items for the specified culture
        /// <br></br>
        /// -Or- 
        /// <br></br>
        /// There is an error loading the vocabulary.
        /// </exception>
        Task<Vocabulary.Vocabulary> GetVocabularyAsync(string vocabularyId, bool cultureIsFixed = false);

        /// <summary>
        /// Retrieves lists of vocabulary items for the specified vocabularies in the user's current culture.
        /// </summary>
        /// <param name="vocabularyKeys">
        /// A list of keys identifying the requested vocabularies.
        /// </param>
        /// <param name="cultureIsFixed">
        /// HealthVault looks for the vocabulary items for the culture info
        /// specified using <see cref="HealthServiceConnection.Culture"/>.
        /// If <paramref name="cultureIsFixed"/> is set to <b>false</b> and if
        /// items are not found for the specified culture, items for the
        /// default fallback culture are returned. If
        /// <paramref name="cultureIsFixed"/> is set to <b>true</b>,
        /// fallback will not occur, and if items are not found for the
        /// specified culture, empty strings are returned.
        /// </param>
        /// <returns>
        /// One of the specified vocabularies and its items, or empty strings.
        /// </returns>
        /// <exception cref="HealthServiceException">
        /// There is an error in the server request.
        /// <br></br>
        /// -Or- 
        /// <br></br>
        /// One of the requested vocabularies is not found on the server.
        /// <br></br>
        /// -Or- 
        /// <br></br>
        /// One of the requested vocabularies does not contain representations 
        /// for its items for the specified culture
        /// <br></br>
        /// -Or- 
        /// <br></br>
        /// There is an error loading the vocabularies.
        /// </exception>
        Task<IList<Vocabulary.Vocabulary>> GetVocabulariesAsync(IList<string> vocabularyKeys, bool cultureIsFixed = false);

        /// <summary>
        /// Searches a specific vocabulary and retrieves the matching vocabulary items.
        /// </summary>
        /// 
        /// <remarks>
        /// This method does text search matching of display text and abbreviation text
        /// for the culture defined by the <see cref="HealthServiceConnection.Culture"/>. 
        /// The <paramref name="searchValue"/> is a string of characters in the specified 
        /// culture. 
        /// </remarks>
        /// <param name="searchValue">
        ///     The search string to use.
        /// </param>
        /// <param name="searchType">
        ///     The type of search to perform.
        /// </param>
        /// <param name="maxResults">
        ///     The maximum number of results to return. If null, all matching results 
        ///     are returned, up to a maximum number defined by the service config 
        ///     value with key maxResultsPerVocabularyRetrieval.
        /// </param>
        /// <returns>
        /// A <see cref="VocabularyItemCollection"/> populated with entries matching 
        /// the search criteria.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="searchValue"/> is <b>null</b> or empty or greater 
        /// than <b>255</b> characters.
        /// <br></br>
        /// -Or-
        /// <br></br>
        /// if <paramref name="searchType"/> is not a known 
        /// <see cref="VocabularySearchType"/> value.        
        /// <br></br>
        /// -Or-
        /// <br></br>
        /// when <paramref name="maxResults"/> is defined but has a value less than 1.        
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// There is an error in the server request.         
        /// <br></br>
        /// -Or-        
        /// <br></br>
        /// The requested vocabulary is not found on the server.
        /// <br></br>
        /// -Or- 
        /// The requested search culture is not supported. 
        /// </exception>        
        Task<ReadOnlyCollection<VocabularyKey>> SearchVocabularyAsync(string searchValue, VocabularySearchType searchType, int? maxResults);
    }
}
