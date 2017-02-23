// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Security;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// A <see cref="TemplateField"/> implementation used on the
    /// <see cref="HealthRecordItemDataGrid"/> to override Signed column.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [SecurityCritical]
    internal class GridViewIsSignedTemplate : ITemplate
    {
        /// <summary>
        /// Constructs a GridViewIsSignedTemplate with the specified
        /// template type, column name, header text and the column value override.
        /// </summary>
        ///
        /// <param name="type">
        /// Tells the template which type of a controls should be added to the
        /// container based on the health record item type.
        /// </param>
        ///
        /// <param name="columnName">
        /// The name of the column the template applies to.
        /// </param>
        ///
        /// <param name="headerText">
        /// The header text of the column.
        /// </param>
        ///
        /// <param name="isSignedOverride">
        /// The override value of the column.
        /// </param>
        ///
        [SecurityCritical]
        internal GridViewIsSignedTemplate(
            TemplateType type,
            string columnName,
            string headerText,
            string isSignedOverride)
        {
            _columnName = columnName;
            _headerText = headerText;
            _isSignedOverride = isSignedOverride;
            _type = type;
        }

        /// <summary>
        /// Defines the Control object that child controls and templates belong
        /// to. These child controls are in turn defined within an inline
        /// template.
        /// </summary>
        ///
        /// <param name="container">
        /// The <see cref="Control"/> object to contain the instances of the
        /// controls from the inline template.
        /// </param>
        ///
        [SecuritySafeCritical]
        public void InstantiateIn(Control container)
        {
            Validator.ThrowIfArgumentNull(container, "container", "ArgumentNull");

            switch (_type)
            {
                case TemplateType.Header:
                    container.Controls.Add(new LiteralControl(_headerText));
                    break;
                case TemplateType.Item:
                    LiteralControl literalValue = new LiteralControl();
                    literalValue.DataBinding += new EventHandler(IsSigned_DataBind);
                    container.Controls.Add(literalValue);
                    break;
                default:
                    return;
            }
        }

        [SecurityCritical]
        private void IsSigned_DataBind(object sender, EventArgs e)
        {
            LiteralControl literalValue = (LiteralControl)sender;
            GridViewRow container = (GridViewRow)literalValue.NamingContainer;

            bool dataValue = (bool)DataBinder.Eval(container.DataItem, _columnName);

            literalValue.Text =
                dataValue ? _isSignedOverride : GridViewIsSignedTemplate.SpaceEntity;
        }

        private string _columnName;
        private string _headerText;
        private string _isSignedOverride;
        private TemplateType _type;
        internal const string SpaceEntity = "&nbsp;";
    }
}
