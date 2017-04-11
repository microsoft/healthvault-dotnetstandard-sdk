// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Record;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// An interface for the HealthVault person client. Used to access information and records associated with the currently authenticated user.
    /// </summary>
    public interface IPersonClient : IClient
    {
        #region ApplicationSettings

        /// <summary>
        /// Gets the application settings for the current application and
        /// person.
        /// </summary>
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

        /// <summary>
        /// Gets information about people authorized for an application.
        /// </summary>
        ///
        /// <returns>
        /// Collection of <see cref="PersonInfo"/> objects representing 
        /// people authorized for the application.
        /// </returns>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        Task<IReadOnlyCollection<PersonInfo>> GetAuthorizedPeopleAsync();

        /// <summary>
        /// Gets the information about the person specified.
        /// </summary>
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

        #region GetAuthorizedRecords

        /// <summary>
        /// Gets the <see cref="HealthRecordInfo"/> for the records identified
        /// by the specified <paramref name="recordIds"/>.
        /// </summary>
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
