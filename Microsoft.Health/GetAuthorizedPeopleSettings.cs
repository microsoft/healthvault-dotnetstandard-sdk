// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.Health
{
    /// <summary>
    /// The settings for retrieving a set of <see cref="PersonInfo"/> objects through 
    /// <see cref="ApplicationConnection.GetAuthorizedPeople(GetAuthorizedPeopleSettings)"/>
    /// </summary>
    /// 
    /// <remarks>
    /// These settings allow specifying the behavior of data retrieval from the HealthVault 
    /// service through 
    /// <see cref="ApplicationConnection.GetAuthorizedPeople(GetAuthorizedPeopleSettings)"/>.
    /// </remarks>
    public class GetAuthorizedPeopleSettings
    {
        /// <summary>
        /// Creates a <see cref="GetAuthorizedPeopleSettings" /> object with default values.
        /// </summary>
        public GetAuthorizedPeopleSettings()
        {
        }

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
                Validator.ThrowArgumentOutOfRangeIf(
                    value < 0,
                    "BatchSize",
                    "GetAuthorizedPeopleSettingsBatchSizeNegative");
                _batchSize = value;
            }
        }
        private int _batchSize;

        /// <summary>
        /// Get or sets the DateTime in UTC that will be used to filter authorized people
        /// from the returned list according to the date that the person was authorized for the application.
        /// Calls to <see cref="ApplicationConnection.GetAuthorizedPeople(GetAuthorizedPeopleSettings)" /> 
        /// will only return people whose authorization was created after the given date and time.
        /// </summary>
        /// 
        /// <remarks>
        /// The default value is DateTime.MinValue.
        /// <br/><br/>
        /// The application is responsible for converting from local time to UTC.        
        /// </remarks>
        public DateTime AuthorizationsCreatedSince
        {
            get { return _authorizationsCreatedSince; }
            set { _authorizationsCreatedSince = value; }
        }
        private DateTime _authorizationsCreatedSince = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the person id that determines the starting point for the 
        /// <see cref="PersonInfo"/> object iterator.
        /// </summary>
        /// 
        /// <remarks>
        /// The <see cref="PersonInfo"/> iterator returned by 
        /// <see cref="ApplicationConnection.GetAuthorizedPeople(GetAuthorizedPeopleSettings)" /> 
        /// will begin with the first authorized person <em>after</em> the person whose ID is 
        /// specified by <b>StartingPersonId</b>.
        /// <br /><br />
        /// To begin with the first available authorized person, set this value to <see cref="Guid.Empty" />. 
        /// The default value is <see cref="Guid.Empty" />.
        /// <br/><br/>
        /// In case of a <see cref="HealthServiceException" /> while iterating through the results, 
        /// the last successfully retrieved person id can be specified for this value and and used 
        /// to retrieve a new iterator with   
        /// <see cref="ApplicationConnection.GetAuthorizedPeople(GetAuthorizedPeopleSettings)"/>. 
        /// The new iterator will begin with the authorized person that follows the last successfully retrieved PersonInfo.
        /// </remarks>
        /// 
        public Guid StartingPersonId
        {
            get { return _startingPersonId; }
            set { _startingPersonId = value; }
        }
        private Guid _startingPersonId;
    }
}