// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Thing;
using NodaTime;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// Methods to interact with the platform.
    /// </summary>
    public interface IPlatformClient : IClient
    {
        /// <summary>
        /// Gets the instance where a HealthVault account should be created
        /// for the specified account location.
        /// </summary>
        ///
        /// <param name="preferredLocation">
        /// A user's preferred geographical location, used to select the best instance
        /// in which to create a new HealthVault account. If there is a location associated
        /// with the credential that will be used to log into the account, that location
        /// should be used.
        /// </param>
        ///
        /// <remarks>
        /// If no suitable instance can be found, a null value is returned. This can happen,
        /// for example, if the account location is not supported by HealthVault.
        ///
        /// Currently the returned instance IDs all parse to integers, but that is not
        /// guaranteed and should not be relied upon.
        /// </remarks>
        ///
        /// <returns>
        /// A <see cref="HealthServiceInstance"/> object represents the selected instance,
        /// or null if no suitable instance exists.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="preferredLocation"/> is <b>null</b>.
        /// </exception>
        ///
        Task<HealthServiceInstance> SelectInstanceAsync(Location preferredLocation);

        /// <summary>
        /// Gets information about the HealthVault service.
        /// </summary>
        ///
        /// <remarks>
        /// Gets the latest information about the HealthVault service. This
        /// includes:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        /// </remarks>
        ///
        /// <returns>
        /// A <see cref="ServiceInfo"/> instance that contains the service version, SDK
        /// assemblies versions and URLs, method information, and so on.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        Task<ServiceInfo> GetServiceDefinitionAsync();

        /// <summary>
        /// Gets information about the HealthVault service only if it has been updated since
        /// the specified update time.
        /// </summary>
        ///
        /// <param name="lastUpdatedTime">
        /// The time of the last update to an existing cached copy of <see cref="ServiceInfo"/>.
        /// </param>
        ///
        /// <remarks>
        /// Gets the latest information about the HealthVault service, if there were updates
        /// since the specified <paramref name="lastUpdatedTime"/>.  If there were no updates
        /// the method returns <b>null</b>.
        /// This includes:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        /// </remarks>
        ///
        /// <returns>
        /// If there were updates to the service information since the specified <paramref name="lastUpdatedTime"/>,
        /// a <see cref="ServiceInfo"/> instance that contains the service version, SDK
        /// assemblies versions and URLs, method information, and so on.  Otherwise, if there were no updates,
        /// returns <b>null</b>.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        Task<ServiceInfo> GetServiceDefinitionAsync(Instant lastUpdatedTime);

        /// <summary>
        /// Gets information about the HealthVault service corresponding to the specified
        /// categories.
        /// </summary>
        ///
        /// <param name="responseSections">
        /// A bitmask of one or more <see cref="ServiceInfoSections"/> which specify the
        /// categories of information to be populated in the <see cref="ServiceInfo"/>.
        /// </param>
        ///
        /// <remarks>
        /// Gets the latest information about the HealthVault service. Depending on the specified
        /// <paramref name="responseSections"/>, this will include some or all of:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        ///
        /// Retrieving only the sections you need will give a faster response time than
        /// downloading the full response.
        /// </remarks>
        ///
        /// <returns>
        /// A <see cref="ServiceInfo"/> instance that contains some or all of the service version,
        /// SDK assemblies versions and URLs, method information, and so on, depending on which
        /// information categories were specified.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        Task<ServiceInfo> GetServiceDefinitionAsync(ServiceInfoSections responseSections);

        /// <summary>
        /// Gets information about the HealthVault service corresponding to the specified
        /// categories if the requested information has been updated since the specified
        /// update time.
        /// </summary>
        ///
        /// <param name="responseSections">
        /// A bitmask of one or more <see cref="ServiceInfoSections"/> which specify the
        /// categories of information to be populated in the <see cref="ServiceInfo"/>.
        /// </param>
        ///
        /// <param name="lastUpdatedTime">
        /// The time of the last update to an existing cached copy of <see cref="ServiceInfo"/>.
        /// </param>
        ///
        /// <remarks>
        /// Gets the latest information about the HealthVault service, if there were updates
        /// since the specified <paramref name="lastUpdatedTime"/>.  If there were no updates
        /// the method returns <b>null</b>.
        /// Depending on the specified
        /// <paramref name="responseSections"/>, this will include some or all of:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        ///
        /// Retrieving only the sections you need will give a faster response time than
        /// downloading the full response.
        /// </remarks>
        ///
        /// <returns>
        /// If there were updates to the service information since the specified <paramref name="lastUpdatedTime"/>,
        /// a <see cref="ServiceInfo"/> instance that contains some or all of the service version,
        /// SDK  assemblies versions and URLs, method information, and so on, depending on which
        /// information categories were specified.  Otherwise, if there were no updates, returns
        /// <b>null</b>.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception>
        ///
        Task<ServiceInfo> GetServiceDefinitionAsync(ServiceInfoSections responseSections, Instant lastUpdatedTime);

        /// <summary>
        /// Gets the definitions for one or more thing type definitions
        /// supported by HealthVault.
        /// </summary>
        ///
        /// <param name="typeIds">
        /// A collection of health item type IDs whose details are being requested. Null
        /// indicates that all health item types should be returned.
        /// </param>
        ///
        /// <param name="sections">
        /// A collection of ThingTypeSections enumeration values that indicate the type
        /// of details to be returned for the specified health item records(s).
        /// </param>
        ///
        /// <param name="imageTypes">
        /// A collection of strings that identify which health item record images should be
        /// retrieved.
        ///
        /// This requests an image of the specified mime type should be returned. For example,
        /// to request a GIF image, "image/gif" should be specified. For icons, "image/vnd.microsoft.icon"
        /// should be specified. Note, not all health item records will have all image types and
        /// some may not have any images at all.
        ///
        /// If '*' is specified, all image types will be returned.
        /// </param>
        ///
        /// <param name="lastClientRefreshDate">
        /// An <see cref="Instant"/> that specifies the time of the last refresh
        /// made by the client.
        /// </param>
        ///
        /// <returns>
        /// The type definitions for the specified types, or empty if the
        /// <paramref name="typeIds"/> parameter does not represent a known unique
        /// type identifier.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="typeIds"/> is <b>non null</b> and empty, or
        /// <paramref name="typeIds"/> is <b>non null</b> and member in <paramref name="typeIds"/> is
        /// <see cref="System.Guid.Empty"/>.
        /// </exception>
        ///
        Task<IDictionary<Guid, ThingTypeDefinition>> GetHealthRecordItemTypeDefinitionAsync(
            IList<Guid> typeIds,
            ThingTypeSections sections,
            IList<string> imageTypes,
            Instant? lastClientRefreshDate);

        /// <summary>
        /// Creates a new application instance. This is the first step in the SODA authentication flow.
        /// </summary>
        /// <returns>Information about the newly created application instance.</returns>
        Task<ApplicationCreationInfo> NewApplicationCreationInfoAsync();

        /// <summary>
        /// Gets information about people authorized for an application.
        /// </summary>
        ///
        /// <remarks>
        /// The returned IEnumerable iterator will access the HealthVault service
        /// across the network. The default <see cref="GetAuthorizedPeopleSettings"/>
        /// values are used.
        /// </remarks>
        ///
        /// <returns>
        /// An IEnumerable iterator of <see cref="PersonInfo"/> objects representing
        /// people authorized for the application.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        IEnumerable<Task<PersonInfo>> GetAuthorizedPeople();

        /// <summary>
        /// Gets information about people authorized for an application.
        /// </summary>
        ///
        /// <remarks>
        /// The returned IEnumerable iterator will access the HealthVault service
        /// across the network. See <see cref="GetAuthorizedPeopleSettings"/> for applicable
        /// settings.
        /// </remarks>
        ///
        /// <param name="settings">
        /// The <see cref="GetAuthorizedPeopleSettings" /> object used to configure the
        /// IEnumerable iterator returned by this method.
        /// </param>
        ///
        /// <returns>
        /// An IEnumerable iterator of <see cref="PersonInfo"/> objects representing
        /// people authorized for the application.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error. The retrieval can be retried from the
        /// current position by calling this method again and using the last successfully
        /// retrieved person Id for <see cref="GetAuthorizedPeopleSettings.StartingPersonId"/>.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null.
        /// </exception>
        ///
        IEnumerable<Task<PersonInfo>> GetAuthorizedPeople(GetAuthorizedPeopleSettings settings);

        /// <summary>
        /// Removes authorization for the given record.
        /// </summary>
        /// <param name="recordId">The record to remove authorization for.</param>
        Task RemoveApplicationRecordAuthorizationAsync(Guid recordId);
    }
}
