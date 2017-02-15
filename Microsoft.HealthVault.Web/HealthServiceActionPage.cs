// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Security;
using System.Security.Permissions;
using System.Web;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// A redirector page to be used as the "action" page for Microsoft
    /// HealthVault applications.
    /// </summary>
    ///
    /// <remarks>
    /// All HealthVault applications are required to expose a set
    /// of URLs for information and functionality that they expose.
    /// For instance, all applications must expose a Service Agreement,
    /// privacy statement, a home page, help, etc.<br/>
    /// <br/>
    /// This page acts as a simple redirector for these action pages such
    /// that the application can easily configure these action pages through
    /// their web.config file.<br/>
    /// <br/>
    /// To use this page, create an action.aspx file at the URL that was
    /// specified for the "action URL" when your application was registered.
    /// That action.aspx file should point to this class for it's
    /// implementation. In your web.config file, add entries for each of the
    /// action URLs that your application supports using WCPage_Action as the
    /// prefix for the key. For example, for the Service Agreement action URL
    /// create a setting in the web.config with key
    /// WCPage_ActionServiceAgreement and value containing the URL to your
    /// application Service Agreement.<br/>
    /// <br/>
    /// The page should also contain some text stating that the application
    /// doesn't support the particular action in case the redirect doesn't
    /// occur. The action that is being requested can be found in the
    /// <see cref="Action"/> property and the action query string can be found
    /// the <see cref="ActionQueryString"/> property.
    /// </remarks>
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [SecurityCritical]
    public class HealthServiceActionPage : HealthServicePage
    {
        /// <summary>
        /// Reads the target from the query string calls the appropriate virtual method to
        /// perform the action.
        /// </summary>
        ///
        /// <param name="sender">
        /// The sender of the message.
        /// </param>
        ///
        /// <param name="e">
        /// The arguments of the message.
        /// </param>
        ///
        /// <remarks>
        /// In most cases the action performed is to redirect the user's browser to a URL defined
        /// by the web.config file or if that isn't defined and the <see cref="ActionQueryString"/>
        /// is a URL to that URL instead.
        /// </remarks>
        protected void Page_Load(Object sender, EventArgs e)
        {
            _action = Request.QueryString[Target];
            _actionQueryString = Request.QueryString[TargetQueryString];

            if (!String.IsNullOrEmpty(_action))
            {
                _action = _action.ToUpper();

                switch (_action)
                {
                    case "HOME":
                        OnActionHome(_action, _actionQueryString);
                        break;

                    case "SERVICEAGREEMENT":
                        OnActionServiceAgreement(_action, _actionQueryString);
                        break;

                    case "HELP":
                        OnActionHelp(_action, _actionQueryString);
                        break;

                    case "APPAUTHREJECT":
                        OnActionApplicationAuthorizationRejected(_action, _actionQueryString);
                        break;

                    case "APPAUTHSUCCESS":
                        OnActionApplicationAuthorizationSuccessful(_action, _actionQueryString);
                        break;

                    case "APPAUTHFAILURE":
                        OnActionApplicationAuthorizationFailed(_action, _actionQueryString);
                        break;

                    case "APPAUTHINVALIDRECORD":
                        OnActionApplicationAuthorizationInvalidRecord(_action, _actionQueryString);
                        break;

                    case "CREATERECORDFAILURE":
                        OnActionCreateRecordFailure(_action, _actionQueryString);
                        break;

                    case "CREATERECORDCANCELED":
                        OnActionCreateRecordCanceled(_action, _actionQueryString);
                        break;

                    case "SELECTEDRECORDCHANGED":
                        OnActionSelectedRecordChanged(_action, _actionQueryString);
                        break;

                    case "SHARERECORDSUCCESS":
                        OnActionShareRecordSucceeded(_action, _actionQueryString);
                        break;

                    case "SHARERECORDFAILED":
                        OnActionShareRecordFailed(_action, _actionQueryString);
                        break;

                    case "PRIVACY":
                        OnActionPrivacy(_action, _actionQueryString);
                        break;

                    case "SIGNOUT":
                        OnActionSignOut(_action, _actionQueryString);
                        break;

                    case "RECONCILECOMPLETE":
                        OnActionReconcileComplete(_action, _actionQueryString);
                        break;

                    case "RECONCILEFAILURE":
                        OnActionReconcileFailure(_action, _actionQueryString);
                        break;

                    case "RECONCILECANCELED":
                        OnActionReconcileCanceled(_action, _actionQueryString);
                        break;

                    default:
                        OnActionUnknown(_action, _actionQueryString);
                        break;
                }
            }
        }

        /// <summary>
        /// Redirects to the application's home page using the WCPage_ActionHome configuration
        /// value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "HOME".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionHome(string action, string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's service agreement page using the
        /// WCPage_ActionServiceAgreement configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "SERVICEAGREEMENT".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionServiceAgreement(string action, string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's help page using the
        /// WCPage_ActionHelp configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "HELP".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionHelp(string action, string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's authorization page using the
        /// WCPage_ActionAppAuthReject configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "APPAUTHREJECT".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionApplicationAuthorizationRejected(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's authorization page using the
        /// WCPage_ActionAppAuthSuccess configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "APPAUTHSUCCESS".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionApplicationAuthorizationSuccessful(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's authorization page using the
        /// WCPage_ActionAppAuthFailure configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "APPAUTHFAILURE".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionApplicationAuthorizationFailed(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's authorization page using the
        /// WCPage_ActionAppAuthInvalidRecord configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "APPAUTHINVALIDRECORD".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionApplicationAuthorizationInvalidRecord(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's record management page using the
        /// WCPage_ActionCreateRecordFailure configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "CREATERECORDFAILURE".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionCreateRecordFailure(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's record management page using the
        /// WCPage_ActionCreateRecordCanceled configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "CREATERECORDCANCELED".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionCreateRecordCanceled(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's record management page using the
        /// WCPage_ActionSelectedRecordChanged configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "SELECTEDRECORDCHANGED".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method refreshes the user's cookie by calling
        /// <see cref="HealthServicePage.RefreshAndPersist()"/> and then calls
        /// <see cref="OnActionUnknown"/> to redirect to the URL defined by the web.config file
        /// or the <see cref="ActionQueryString"/>. To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionSelectedRecordChanged(
            string action,
            string actionQueryString)
        {
            if (IsLoggedIn)
            {
                RefreshAndPersist();
            }
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's record management page using the
        /// WCPage_ActionShareRecordSuccess configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "SHARERECORDSUCCESS".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionShareRecordSucceeded(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's record management page using the
        /// WCPage_ActionShareRecordFailed configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "SHARERECORDFAILED".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionShareRecordFailed(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's privacy page using the
        /// WCPage_ActionPrivacy configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "PRIVACY".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionPrivacy(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's signout page using the
        /// WCPage_ActionSignOut configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "SIGNOUT".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionSignOut(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's clinical document page using the
        /// WCPage_ActionReconcileSuccess configuration value in the web.config.
        /// </summary>
        ///
        /// <remarks>
        /// Deprecated.  This handler is no longer used.
        /// </remarks>
        ///
        protected virtual void OnActionReconcileSuccess(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's clinical document page using the
        /// WCPage_ActionReconcileSuccess configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "RECONCILECOMPLETE".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionReconcileComplete(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's clinical document page using the
        /// WCPage_ActionReconcileFailure configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "RECONCILEFAILURE".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionReconcileFailure(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the application's clinical document page using the
        /// WCPage_ActionReconcileCanceled configuration value in the web.config.
        /// </summary>
        ///
        /// <param name="action">
        /// Should always be "RECONCILECANCELED".
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The value of the "actionqs" query string parameter.
        /// </param>
        ///
        /// <remarks>
        /// The default implementation of this method calls <see cref="OnActionUnknown"/> to
        /// redirect to the URL defined by the web.config file or the <see cref="ActionQueryString"/>.
        /// To change this behavior the
        /// application can override this method and provide a different implementation.
        /// <see cref="GetTargetLocation"/> can be called to get the URL from the web.config file
        /// if needed.
        /// </remarks>
        ///
        protected virtual void OnActionReconcileCanceled(
            string action,
            string actionQueryString)
        {
            OnActionUnknown(action, actionQueryString);
        }

        /// <summary>
        /// Redirects to the specified action page if specified in the web.config file.
        /// </summary>
        ///
        /// <param name="action">
        /// The action that is being redirected to.
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The action query string parameters to pass on to the action page.
        /// </param>
        ///
        /// <remarks>
        /// If the target action specified to the action page is not one of the recognized actions
        /// it can still be handled by the application by specifying a URL to redirect to in the
        /// web.config file. If the web.config file doesn't have a redirect URL for the action and
        /// the <see cref="ActionQueryString"/> contains only a URL, the user's browser will be
        /// directed to that URL instead. The key should be WCPage_Action with the action name
        /// appended. For example, WCPage_ActionAppAuthSuccess can be used to specify a URL for
        /// the AppAuthSuccess action.
        /// </remarks>
        ///
        protected virtual void OnActionUnknown(string action, string actionQueryString)
        {
            string targetLocation;
            try
            {
                targetLocation =
                    GetTargetLocation(
                        action,
                        actionQueryString);
            }
            catch (Exception)
            {
                // Take any exception to mean that the application doesn't
                // support the specified action.
                targetLocation = null;
            }

            if (targetLocation != null)
            {
                Response.Redirect(targetLocation);
            }
            else
            {
                if (IsRequestOkayToRedirect(
                    actionQueryString,
                    Request.Url.Host,
                    HealthWebApplicationConfiguration.Current.AllowedRedirectSites))
                {
                    Response.Redirect(actionQueryString);
                }
            }
        }

        private static bool IsRequestOkayToRedirect(
            string actionQueryString,
            string requestHost,
            string allowedRedirectSites)
        {
            // relative urls are always allowed...
            if (Uri.IsWellFormedUriString(actionQueryString, UriKind.Relative))
            {
                if (actionQueryString.StartsWith("//", StringComparison.Ordinal))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            // the same site as the request is allowed...
            if (Uri.IsWellFormedUriString(actionQueryString, UriKind.Absolute))
            {
                Uri uri = new Uri(actionQueryString);

                if (requestHost.Equals(uri.Host, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                // sites that are on the allowed list are allowed...
                if (allowedRedirectSites != null)
                {
                    foreach (string site in allowedRedirectSites.Split(','))
                    {
                        if (site.Equals(uri.Host, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the URL of the location and the query string for the
        /// specified action.
        /// </summary>
        ///
        /// <param name="action">
        /// The action identifier used to tell the application which
        /// action page should be shown.
        /// </param>
        ///
        /// <param name="actionQueryString">
        /// The query string parameters that should be passed to the
        /// target action URL.
        /// </param>
        ///
        /// <returns>
        /// The full URL including query string of the action page to
        /// redirect to.
        /// </returns>
        ///
        /// <remarks>
        /// The default implementation reads the action URL from the
        /// web.config file using the key "WCPage_Action" +
        /// <paramref name="action"/>.<br/>
        /// <br/>
        /// The currently supported actions are:
        /// <ul>
        ///     <li>Home</li>
        ///     <li>ServiceAgreement</li>
        ///     <li>Help</li>
        ///     <li>AppAuthSuccess</li>
        ///     <li>AppAuthFailure</li>
        ///     <li>AppAuthReject</li>
        ///     <li>AppAuthInvalidRecord</li>
        ///     <li>CreateRecordFailure</li>
        ///     <li>CreateRecordCanceled</li>
        ///     <li>ReconcileComplete</li>
        ///     <li>ReconcileFailure</li>
        ///     <li>ReconcileCanceled</li>
        ///     <li>SelectedRecordChanged</li>
        ///     <li>SignOut</li>
        ///     <li>ShareRecordSuccess</li>
        ///     <li>ShareRecordFailed</li>
        ///     <li>Privacy</li>
        /// </ul>
        /// </remarks>
        ///
        protected virtual string GetTargetLocation(
            string action,
            string actionQueryString)
        {
            string targetLocation = null;

            if (action != null)
            {
                _action = action;

                targetLocation =
                    HealthWebApplicationConfiguration.Current.GetActionUrl(_action).OriginalString;

                if (!String.IsNullOrEmpty(actionQueryString))
                {
                    if ('?' != actionQueryString[0])
                    {
                        targetLocation = targetLocation + "?" + actionQueryString;
                    }
                    else
                    {
                        targetLocation = targetLocation + actionQueryString;
                    }
                }
            }

            return targetLocation;
        }

        private const string Target = "target";
        private const string TargetQueryString = "actionqs";

        /// <summary>
        /// Gets the action that was specified in the query string.
        /// </summary>
        ///
        /// <remarks>
        /// The currently supported actions are:
        /// <ul>
        ///     <li>Home</li>
        ///     <li>ServiceAgreement</li>
        ///     <li>Help</li>
        ///     <li>AppAuthSuccess</li>
        ///     <li>AppAuthFailure</li>
        ///     <li>AppAuthReject</li>
        ///     <li>AppAuthInvalidRecord</li>
        ///     <li>CreateRecordFailure</li>
        ///     <li>CreateRecordCanceled</li>
        ///     <li>ReconcileComplete</li>
        ///     <li>ReconcileFailure</li>
        ///     <li>ReconcileCanceled</li>
        ///     <li>SelectedRecordChanged</li>
        ///     <li>SignOut</li>
        ///     <li>ShareRecordSuccess</li>
        ///     <li>ShareRecordFailed</li>
        ///     <li>Privacy</li>
        /// </ul>
        /// </remarks>
        ///
        public string Action => _action;

        private string _action;

        /// <summary>
        /// Gets the "actionqs" query string parameter that was specified on the request.
        /// </summary>
        ///
        public string ActionQueryString => _actionQueryString;

        private string _actionQueryString;

        /// <summary>
        /// The action page does not require log on.
        /// </summary>
        protected override bool LogOnRequired
        {
            [SecurityCritical]
            get
            {
                return false;
            }
        }
    }
}
