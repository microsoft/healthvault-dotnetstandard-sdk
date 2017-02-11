// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;


namespace Microsoft.Health.Events
{
    /// <summary>
    /// The SubscriptionManager class is used to manage an application's eventing subscriptions.
    /// </summary>
    /// <remarks>
    /// A subscription allows an application to receive a notification when a specific event happens 
    /// on the HealthVault platform. For example, an application may wish to be notified when any BloodGlucose
    /// items are added to the records it is authorized to see. 
    /// 
    /// Subscriptions are described by the <see cref="Subscription"/> class and classes that are derived from it. 
    /// 
    /// The Subscription class itself contains the <see cref="CommonSubscriptionData"/> class, storing
    /// the notification channel (which defines how the information
    /// is communicated to the application) and a unique id for the subscription.
    /// 
    /// The derived class denotes the type of event that the subscription describes (for example, the
    /// <see cref="HealthRecordItemChangedSubscription"/> class subscribes to item changes in a record) and any specific
    /// information that is required for the subscription.
    /// </remarks>
    public class SubscriptionManager
    {
        /// <summary>
        /// Create an instance of the SubscriptionManager class based a connection.
        /// </summary>
        /// <param name="connection">The application connection.</param>
        public SubscriptionManager(ApplicationConnection connection)
        {
            _connection = connection;
        }

        static XmlWriter CreateWriter(StringBuilder requestParameters)
        {
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            XmlWriter writer = XmlWriter.Create(requestParameters, settings);

            return writer;
        }

        private HealthServiceRequest CreateRequest(string methodName, int version)
        {
            Validator.ThrowInvalidIfNull(_connection, "SubscriptionConnectionNull");

            return _connection.CreateRequest(methodName, version);
        }

        /// <summary>
        /// Register a subscription with the HealthVault platform.
        /// </summary>
        /// <remarks>
        /// After the subscription is registered the <see cref="CommonSubscriptionData.Id"/>
        /// property will be set to the unique identifier of the subscription.
        /// Applications may want to save this identifier to modify the subscription later. 
        /// </remarks>
        /// <param name="subscription">The subscription to register.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="subscription"/> parameter is <b>null</b>.
        /// </exception>        
        ///         
        /// <exception cref="InvalidOperationException">
        /// The <see cref="CommonSubscriptionData.Id"/> property is not <b>null</b>.
        /// </exception>        
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        public void Subscribe(Subscription subscription)
        {
            Validator.ThrowIfArgumentNull(subscription, "subscription", "SubscriptionNull");
            Validator.ThrowArgumentExceptionIf(subscription.CommonSubscriptionData == null, "subscription", "SubscriptionCommonSubscriptionDataNull");

            HealthServiceRequest request = CreateRequest("SubscribeToEvent", 1);

            Validator.ThrowInvalidIf(subscription.CommonSubscriptionData.Id != null, "SubscriptionAlreadyCreated");

            StringBuilder requestParameters = new StringBuilder();

            using (XmlWriter writer = CreateWriter(requestParameters))
            {
                subscription.Serialize(writer);

                writer.Flush();
            }

            request.Parameters = requestParameters.ToString();

            request.Execute();

            XPathNavigator infoNav =
                request.Response.InfoNavigator.SelectSingleNode(
                    SDKHelper.GetInfoXPathExpressionForMethod(
                        request.Response.InfoNavigator,
                        "SubscribeToEvent"));

            XPathNavigator subscriptionIdNavigator =
                infoNav.SelectSingleNode("subscription-id");

            subscription.CommonSubscriptionData.Id = new Guid(subscriptionIdNavigator.Value);
        }

        /// <summary>
        /// Unregister the subscription from the HealthVault platform.
        /// </summary>
        /// <param name="subscription">The subscription to unsubscribe.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="subscription"/> parameter is <b>null</b>.
        /// </exception>        
        ///         
        /// <exception cref="InvalidOperationException">
        /// The <see cref="CommonSubscriptionData.Id"/> property is <b>null</b>.
        /// </exception>        
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        public void Unsubscribe(Subscription subscription)
        {
            Validator.ThrowIfArgumentNull(subscription, "subscription", "SubscriptionNull");
            Validator.ThrowArgumentExceptionIf(subscription.CommonSubscriptionData == null, "subscription", "SubscriptionCommonSubscriptionDataNull");
            Validator.ThrowInvalidIfNull(subscription.CommonSubscriptionData.Id, "SubscriptionIdNull");

            Unsubscribe(subscription.CommonSubscriptionData.Id.Value);
        }

        /// <summary>
        /// Unregister a subscription from the HealthVault platform.
        /// </summary>
        /// <param name="subscriptionId">The unique id of the subscription.</param>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        public void Unsubscribe(Guid subscriptionId)
        {
            HealthServiceRequest request = CreateRequest("UnsubscribeToEvent", 1);

            StringBuilder requestParameters = new StringBuilder();

            using (XmlWriter writer = CreateWriter(requestParameters))
            {
                writer.WriteElementString("subscription-id", subscriptionId.ToString());
                writer.Flush();
            }

            request.Parameters = requestParameters.ToString();

            request.Execute();
        }

        /// <summary>
        /// Update an existing subscription.
        /// </summary>
        /// <param name="subscription">The subscription</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="subscription"/> parameter is <b>null</b>.
        /// </exception>        
        ///         
        /// <exception cref="InvalidOperationException">
        /// The <see cref="CommonSubscriptionData.Id"/> property is <b>null</b>.
        /// </exception>        
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        public void UpdateSubscription(Subscription subscription)
        {
            Validator.ThrowIfArgumentNull(subscription, "subscription", "SubscriptionNull");
            Validator.ThrowArgumentExceptionIf(subscription.CommonSubscriptionData == null, "subscription", "SubscriptionCommonSubscriptionDataNull");
            Validator.ThrowInvalidIfNull(subscription.CommonSubscriptionData.Id, "SubscriptionIdNull");

            HealthServiceRequest request = CreateRequest("UpdateEventSubscription", 1);

            StringBuilder requestParameters = new StringBuilder();

            using (XmlWriter writer = CreateWriter(requestParameters))
            {
                subscription.Serialize(writer);

                writer.Flush();
            }

            request.Parameters = requestParameters.ToString();

            request.Execute();
        }

        /// <summary>
        /// Retrieve a list of all the subscriptions for this application.
        /// </summary>
        /// <returns>A list of subscriptions.</returns>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>        
        public IList<Subscription> GetSubscriptions()
        {
            HealthServiceRequest request = CreateRequest("GetEventSubscriptions", 1);

            request.Execute();

            Collection<Subscription> results = new Collection<Subscription>();

            XPathNavigator infoNav =
                request.Response.InfoNavigator.SelectSingleNode(
                    SDKHelper.GetInfoXPathExpressionForMethod(
                        request.Response.InfoNavigator,
                        "GetEventSubscriptions"));

            foreach (XPathNavigator subscriptionNav in infoNav.Select("subscriptions/subscription"))
            {
                Subscription subscription = Subscription.Deserialize(subscriptionNav);
                results.Add(subscription);
            }

            return results;
        }

        /// <summary>
        /// The connection of the application. 
        /// </summary>
        public ApplicationConnection ApplicationConnection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        private ApplicationConnection _connection;
    }
}
