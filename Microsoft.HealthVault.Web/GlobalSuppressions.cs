// This file lists existing suppressions
// Feel free to delete lines when you fix the underlying issue.
// You should never add to this file; if you must suppress an FxCop warning, put the suppression inline.
// If you absolutely MUST add to this file, you can run 'oacr fxcop <project> /target <assembly>'
// where <project> is HealthVault or HealthVault_SDK and assembly is the dll you're building, including extension.
// This will launch the fxcop gui. Click Analyze, then rightclick the error and select Copy As -> Module level suppression. Paste here.

using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage("Microsoft.Security.Web", "CA3007:ReviewCodeForOpenRedirectVulnerabilities", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#RedirectToShellUrl(System.Web.HttpContext,Microsoft.Health.ShellRedirectParameters)")]
[module: SuppressMessage("Microsoft.Security.Web", "CA3002:ReviewCodeForXssVulnerabilities", Scope = "member", Target = "Microsoft.Health.Web.HealthRecordItemDataGrid.#OnRowDataBound(System.Object,System.Web.UI.WebControls.GridViewRowEventArgs)")]
[module: SuppressMessage("Microsoft.Security.Web", "CA3007:ReviewCodeForOpenRedirectVulnerabilities", Scope = "member", Target = "Microsoft.Health.Web.HealthServiceActionPage.#OnActionUnknown(System.String,System.String)")]
[module: SuppressMessage("Microsoft.Security.Cryptography", "CA5357:RijndaelCannotBeUsed", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#GetEncryptionAlgorithm()")]
[module: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Microsoft.Health.Web.HealthServiceActionPage.#OnActionUnknown(System.String,System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Scope = "type", Target = "Microsoft.Health.Web.WebApplicationConfiguration")]
[module: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#Compress(System.String,System.Int32&)", MessageId = "1#")]
[module: SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", Scope = "member", Target = "Microsoft.Health.Web.HealthServiceActionPage.#Page_Load(System.Object,System.EventArgs)", MessageId = "System.String.ToUpper")]
[module: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#ExceptionToFullString(System.Exception)", MessageId = "System.Text.StringBuilder.AppendFormat(System.String,System.Object[])")]
[module: SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#GetActionUrlRedirectOverride(System.Web.HttpContext)", MessageId = "System.String.StartsWith(System.String)")]
[module: SuppressMessage("Microsoft.MSInternal", "CA900:AptcaAssembliesShouldBeReviewed")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "Microsoft.Health.Web.HealthServicePage.#RedirectToLogOn(System.Boolean)", MessageId = "Mra")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "Microsoft.Health.Web.HealthServicePage.#IsMra", MessageId = "Mra")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationConfiguration.#IsMra", MessageId = "Mra")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#PageOnPreLoad(System.Web.HttpContext,System.Boolean,System.Guid)", MessageId = "PreLoad")]
[module: SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#ApplicationAuthenticationCredential")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#PageOnPreLoad(System.Web.HttpContext,System.Boolean,System.Boolean,System.Guid)", MessageId = "Mra")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#PageOnPreLoad(System.Web.HttpContext,System.Boolean,System.Boolean,System.Guid)", MessageId = "PreLoad")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#PageOnPreLoad(System.Web.HttpContext,System.Boolean)", MessageId = "PreLoad")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#RedirectToLogOn(System.Web.HttpContext,System.Boolean,System.String)", MessageId = "Mra")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#RedirectToLogOn(System.Web.HttpContext,System.Boolean)", MessageId = "Mra")]
[module: SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#ApplicationConnection")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#PageOnPreLoad(System.Web.HttpContext,System.Boolean,System.Boolean)", MessageId = "Mra")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#PageOnPreLoad(System.Web.HttpContext,System.Boolean,System.Boolean)", MessageId = "PreLoad")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#RedirectToLogOn(System.Web.HttpContext,System.Boolean,System.String,System.String)", MessageId = "Mra")]
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "member", Target = "Microsoft.Health.Web.HealthRecordItemDataGrid.#ShowIsPersonalFlag", MessageId = "Flag")]
[module: SuppressMessage("Microsoft.Performance", "CA1824:MarkAssembliesWithNeutralResourcesLanguage")]
[module: SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.Health.Web.HealthServicePage.#RedirectToShellUrl(System.String,System.String,System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.Health.Web.HealthServicePage.#RedirectToShellUrl(System.String,System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.Health.Web.HealthServicePage.#RedirectToLogOn(System.Boolean)")]
[module: SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.Health.Web.HealthServicePage.#RedirectToShellUrl(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Microsoft.Health.Web.HealthWebApplicationConfiguration.#.cctor()")]
[module: SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#HandleTokenOnUrl(System.Web.HttpContext,System.Boolean,System.Guid)", MessageId = "System.Int32.TryParse(System.String,System.Int32@)")]
[module: SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#HandleTokenOnUrl(System.Web.HttpContext,System.Boolean,System.Guid)", MessageId = "isLoginRequired")]
[module: SuppressMessage("Microsoft.Security.Web.Configuration", "CA3105:SetViewStateUserKey", Scope = "member", Target = "Microsoft.Health.Web.HealthServicePage.#OnInit(System.EventArgs)")]
[module: SuppressMessage("Microsoft.Security.Web.Configuration", "CA3105:SetViewStateUserKey", Scope = "type", Target = "Microsoft.Health.Web.HealthServiceActionPage")]
[module: SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.Health.Web.GridViewIsSignedTemplate.#InstantiateIn(System.Web.UI.Control)")]
[module: SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#DeserializePersonInfo(System.String)")]
[module: SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#DecompressInternal(System.Byte[],System.Int32)")]
[module: SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#MarshalCookieVersion2(System.String)")]
[module: SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#GetEncryptionAlgorithm()")]
[module: SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#CompressInternal(System.String,System.Int32&)")]
[module: SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.Health.Web.WebApplicationUtilities.#UnmarshalCookieVersion2(System.String)")]
[module: SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.Health.Web.GridViewAuditActionTemplate.#InstantiateIn(System.Web.UI.Control)")]
[module: SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.Health.Web.HealthRecordItemDataGrid.#AddActionLinksToContainer(System.Web.UI.Control,System.Int32,Microsoft.Health.Web.ActionTemplate)")]