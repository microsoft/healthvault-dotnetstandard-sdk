// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault.Person
{
    /// <summary>
    /// The settings for retrieving a set of <see cref="PersonInfo"/> objects through
    /// <see cref="Microsoft.HealthVault.Clients.IPlatformClient.GetAuthorizedPeople(GetAuthorizedPeopleSettings)"/>
    /// </summary>
    ///
    /// <remarks>
    /// These settings allow specifying the behavior of data retrieval from the HealthVault
    /// service through
    /// <see cref="Microsoft.HealthVault.Clients.IPlatformClient.GetAuthorizedPeople(GetAuthorizedPeopleSettings)"/>.
    /// </remarks>
    public class GetAuthorizedPeopleSettings
    {
        /// <summary>
        /// Gets or sets the number of <see cref="PersonInfo"/> objects to retrieve when a network
        /// call is made to the HealthVault service to retrieve the next batch.
        /// </summary>
        ///
        /// <remarks>
        /// The maximum batch size is limited by the HealthVault platform.
        /// <br /><br />
        /// Specify 0 to set the batch size to the service configuration value
        /// 'defaultPersonInfosPerRetrieval'.
        /// <br /><br />
        /// The default value is 0.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// An attempt was made to set the batch size to a negative number.
        /// </exception>
        ///
        public int BatchSize
        {
            get { return _batchSize; }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), Resources.GetAuthorizedPeopleSettingsBatchSizeNegative);
                }

                _batchSize = value;
            }
        }

        private int _batchSize;

        /// <summary>
        /// Get or sets the DateTime in UTC that will be used to filter authorized people
        /// from the returned list according to the date that the person was authorized for the application.
        /// Calls to <see cref="Microsoft.HealthVault.Clients.IPlatformClient.GetAuthorizedPeople(GetAuthorizedPeopleSettings)" />
        /// will only return people whose authorization was created after the given date and time.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is DateTime.MinValue.
        /// <br/><br/>
        /// The application is responsible for converting from local time to UTC.
        /// </remarks>
        public DateTime AuthorizationsCreatedSince { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the person id that determines the starting point for the
        /// <see cref="PersonInfo"/> object iterator.
        /// </summary>
        ///
        /// <remarks>
        /// The <see cref="PersonInfo"/> iterator returned by
        /// <see cref="Microsoft.HealthVault.Clients.IPlatformClient.GetAuthorizedPeople(GetAuthorizedPeopleSettings)" />
        /// will begin with the first authorized person <em>after</em> the person whose ID is
        /// specified by <b>StartingPersonId</b>.
        /// <br /><br />
        /// To begin with the first available authorized person, set this value to <see cref="Guid.Empty" />.
        /// The default value is <see cref="Guid.Empty" />.
        /// <br/><br/>
        /// In case of a <see cref="HealthServiceException" /> while iterating through the results,
        /// the last successfully retrieved person id can be specified for this value and used
        /// to retrieve a new iterator with
        /// <see cref="Microsoft.HealthVault.Clients.IPlatformClient.GetAuthorizedPeople(GetAuthorizedPeopleSettings)"/>.
        /// The new iterator will begin with the authorized person that follows the last successfully retrieved PersonInfo.
        /// </remarks>
        ///
        public Guid StartingPersonId { get; set; }
    }
}