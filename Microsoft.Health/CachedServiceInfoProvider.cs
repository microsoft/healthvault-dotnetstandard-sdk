// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using Microsoft.Health.PlatformPrimitives;

namespace Microsoft.Health
{
    /// <summary>
    /// A cached provider for service info retrieved from the HealthVault web-service.
    /// </summary>
    ///
    /// <remarks>
    /// This provider will cache the service information it obtains from Platform for
    /// a configured or specified period of time. 
    /// </remarks>
    public class CachedServiceInfoProvider : IServiceInfoProvider
    {
        private LazyCacheWithTtl<ServiceInfo> _serviceInfo;
        private DateTime _lastCheckTime;
        private ServiceInfoSections _responseSections;
        private Uri _healthServiceUrl;

        /// <summary>
        /// Initializes a new instance of the provider using the expiration period
        /// provided through configuration and retrieving the full service info response.
        /// </summary>
        /// 
        /// <remarks>
        /// The expiration period is the value of the
        /// <see cref="HealthApplicationConfiguration.ServiceInfoDefaultCacheTtl"/> configuration.
        /// </remarks>
        /// 
        public CachedServiceInfoProvider()
            : this(HealthApplicationConfiguration.Current.ServiceInfoDefaultCacheTtl)
        {
        }

        /// <summary>
        /// Initializes a new instance of the provider with a specified expiration period and
        /// retrieving the full service info response.
        /// </summary>
        /// 
        /// <param name="timeToLive">
        /// Period of time before the cached instance of service info is considered expired, and a new copy is obtained
        /// from the HealthVault web-service.
        /// </param>
        /// 
        public CachedServiceInfoProvider(TimeSpan timeToLive)
            : this(ServiceInfoSections.All, timeToLive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the provider using the expiration period
        /// provided through configuration and retrieving the service information
        /// for the specified response sections.
        /// </summary>
        /// 
        /// <param name="responseSections">
        /// The categories of information to be populated in the
        /// <see cref="ServiceInfo"/> instance, represented as the result of XOR'ing the
        /// desired categories.
        /// </param>
        /// 
        /// <remarks>
        /// The expiration period is the value of the
        /// <see cref="HealthApplicationConfiguration.ServiceInfoDefaultCacheTtl"/> configuration.
        /// </remarks>
        /// 
        public CachedServiceInfoProvider(ServiceInfoSections responseSections)
            : this(responseSections, HealthApplicationConfiguration.Current.ServiceInfoDefaultCacheTtl)
        {
        }

        /// <summary>
        /// Initializes a new instance of the provider using the expiration period
        /// provided through configuration and retrieving the full service info response
        /// using the specified HealthVault web-service URL.
        /// </summary>
        /// 
        /// <param name="healthServiceUrl">
        /// The URL for the HealthVault web-service to use to retrieve the service information.
        /// </param>
        public CachedServiceInfoProvider(Uri healthServiceUrl)
            : this(ServiceInfoSections.All, HealthApplicationConfiguration.Current.ServiceInfoDefaultCacheTtl, healthServiceUrl)
        {
        }

        /// <summary>
        /// Initializes a new instance of the provider with a specified expiration period and
        /// retrieving the service information for the specified response sections.
        /// </summary>
        /// 
        /// <param name="responseSections">
        /// The categories of information to be populated in the
        /// <see cref="ServiceInfo"/> instance, represented as the result of XOR'ing the
        /// desired categories.
        /// </param>
        /// 
        /// <param name="timeToLive">
        /// Period of time before the cached instance of service info is considered expired, and a new copy is obtained
        /// from the HealthVault web-service.
        /// </param>
        /// 
        public CachedServiceInfoProvider(ServiceInfoSections responseSections, TimeSpan timeToLive)
            : this(responseSections, timeToLive, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the provider with a specified expiration period and
        /// retrieving the service information for the specified response sections using the
        /// specified HealthVault web-service URL.
        /// </summary>
        /// 
        /// <param name="responseSections">
        /// The categories of information to be populated in the
        /// <see cref="ServiceInfo"/> instance, represented as the result of XOR'ing the
        /// desired categories.
        /// </param>
        /// 
        /// <param name="timeToLive">
        /// Period of time before the cached instance of service info is considered expired, and a new copy is obtained
        /// from the HealthVault web-service.
        /// </param>
        /// 
        /// <param name="healthServiceUrl">
        /// The URL for of the HealthVault web-service to use to retrieve the service information.
        /// </param>
        public CachedServiceInfoProvider(
            ServiceInfoSections responseSections,
            TimeSpan timeToLive,
            Uri healthServiceUrl)
        {
            _healthServiceUrl = healthServiceUrl;
            _responseSections = responseSections;
            _serviceInfo = new LazyCacheWithTtl<ServiceInfo>(GetFromService, RefreshFromService, timeToLive);
        }

        /// <summary>
        /// Returns the service information retrieved from the HealthVault web-service.
        /// </summary>
        /// 
        /// <returns>
        /// Service information retrieved from the HealthVault web-service.
        /// </returns>
        /// 
        /// <remarks>
        /// Calls to this method are thread-safe.
        /// </remarks>
        /// 
        public ServiceInfo GetServiceInfo()
        {
            return _serviceInfo.Value;
        }

        /// <summary>
        /// Calls Platform to obtain service info.
        /// </summary>
        /// 
        /// <remarks>
        /// LazyCacheWithTtl enforces that only one thread is executed here at a time, and puts a lock
        /// around this load.
        /// </remarks>
        private ServiceInfo GetFromService()
        {
            var connection = _healthServiceUrl != null
                ? new AnonymousConnection(HealthApplicationConfiguration.Current.ApplicationId, _healthServiceUrl)
                : new AnonymousConnection();

            var serviceInfo = HealthVaultPlatformInformation.Current.GetServiceDefinition(connection, _responseSections);
            _lastCheckTime = serviceInfo.LastUpdated;
            return serviceInfo;
        }

        /// <summary>
        /// Calls Platform to obtain an updated service info.
        /// 
        /// Returns the current service info if Platform returns no service updates.
        /// </summary>
        private ServiceInfo RefreshFromService(ServiceInfo currentServiceInfo)
        {
            var connection = _healthServiceUrl != null
                ? new AnonymousConnection(HealthApplicationConfiguration.Current.ApplicationId, _healthServiceUrl)
                : new AnonymousConnection();

            var serviceInfo = HealthVaultPlatformInformation.Current.GetServiceDefinition(connection, _responseSections, _lastCheckTime);
            _lastCheckTime = DateTime.Now;

            return serviceInfo ?? currentServiceInfo;
        }
    }
}