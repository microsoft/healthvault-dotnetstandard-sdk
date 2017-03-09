using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Record;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// An interface for the HealthVault person client. Used to access information and records associated with the currently athenticated user.
    /// </summary>
    public interface IPersonClient : IClient
    {
        #region ApplicationSettings

        /// <summary>
        /// Gets the application settings for the current application and
        /// person.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <returns>
        /// The complete set application settings including the XML settings, selected record ID, etc.
        /// </returns>
        ///
        Task<ApplicationSettings> GetApplicationSettingsAsync();

        /// <summary>
        /// Sets the application settings for the current application and
        /// person.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <param name="applicationSettings">
        /// The application settings XML.
        /// </param>
        ///
        /// <remarks>
        /// This may be <b>null</b> if no application settings have been
        /// stored for the application or user.
        /// </remarks>
        ///
        Task SetApplicationSettingsAsync(IXPathNavigable applicationSettings);

        /// <summary>
        /// Sets the application settings for the current application and
        /// person.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <param name="requestParameters">
        /// The request parameters.
        /// </param>
        ///
        /// <remarks>
        /// This may be <b>null</b> if no application settings have been
        /// stored for the application or user.
        /// </remarks>
        ///
        Task SetApplicationSettingsAsync(string requestParameters);

        #endregion

        #region GetPersonInfo

        /// <summary>
        /// Gets the information about the person specified.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <returns>
        /// Information about the person's HealthVault account.
        /// </returns>
        ///
        /// <remarks>
        /// This method always calls the HealthVault service to get the latest
        /// information. It is recommended that the calling application cache
        /// the return value and only call this method again if it needs to
        /// refresh the cache.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        Task<PersonInfo> GetPersonInfoAsync();

        #endregion GetPersonInfo

        #region GetAuthorizedRecords

        /// <summary>
        /// Gets the <see cref="HealthRecordInfo"/> for the records identified
        /// by the specified <paramref name="recordIds"/>.
        /// </summary>
        ///
        /// <param name="connection">The connection to use to perform the operation. This connection
        /// must be authenticated. </param>
        ///
        /// <param name="recordIds">
        /// The unique identifiers for the records to retrieve.
        /// </param>
        ///
        /// <returns>
        /// A collection of the records matching the specified record
        /// identifiers and authorized for the authenticated person.
        /// </returns>
        ///
        /// <remarks>
        /// This method is useful in cases where the application is storing
        /// record identifiers and needs access to the functionality provided
        /// by the object model.
        /// </remarks>
        ///
        Task<Collection<HealthRecordInfo>> GetAuthorizedRecordsAsync(IList<Guid> recordIds);

        #endregion GetAuthorizedRecords
    }
}
