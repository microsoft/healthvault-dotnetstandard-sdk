// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// An ASP.NET server control for viewing health record items.
    /// </summary>
    ///
    /// <remarks>
    /// The HealthRecordItemDataGrid works with the
    /// <see cref="Microsoft.Health.HealthRecordItemDataTable"/> to show a
    /// paged list of the health record items matching the specified
    /// search criteria.<br/>
    /// <br/>
    /// This control can only be used inside a <see cref="HealthServicePage"/>.
    /// </remarks>
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [SecurityCritical]
    public class HealthRecordItemDataGrid : Control, INamingContainer
    {
        /// <summary>
        /// Constructs a HealthRecordItemDataGrid instance with default values.
        /// </summary>
        ///
        /// <remarks>
        /// It is usually not necessary to explicitly instantiate the data
        /// grid. Instead, specify the grid in an .aspx file.
        /// </remarks>
        ///
        public HealthRecordItemDataGrid()
        {
            // We are initializing the GridView in the constructor
            // so that the pages that consume HealthRecordItemDataGrid can have
            // access to the grid view's properties. If we do it on
            // CreateChildControls(), it happens after Load, so we would
            // get uninitialized exceptions.
            gridView = new GridView();
            gridView.ID = "gridView";
            gridView.DataKeyNames = new string[] { "wc-id" };
            this.Controls.Add(gridView);
            gridView.PageSize =
                HealthWebApplicationConfiguration.Current.DataGridItemsPerPage;
        }

        /// <summary>
        /// Overrides the base class OnPreRender to populate the data from
        /// HealthVault.
        /// </summary>
        ///
        /// <param name="e">
        /// The event arguments.
        /// </param>
        [SecuritySafeCritical]
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            gridView.EnableViewState = this.EnableViewState;

            if (this.Visible &&
                (!Page.IsPostBack ||
                 this.DataChanged ||
                 !this.EnableViewState))
            {
                PopulateGridView();
            }

            if (this.FinalPreRender != null)
                this.FinalPreRender(this, e);
        }

        /// <summary>
        /// Called by the ASP.NET page to notify server controls that use
        /// composition-based implementation to create any child controls they
        /// contain in preparation for posting back or rendering.
        /// </summary>
        ///
        [SecuritySafeCritical]
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            // Set the GridView properties
            gridView.AutoGenerateColumns = false;
            gridView.RowDataBound +=
                new GridViewRowEventHandler(OnRowDataBound);

            if (!String.IsNullOrEmpty(_alternatingRowCssClass))
            {
                gridView.AlternatingRowStyle.CssClass =
                    _alternatingRowCssClass;
            }

            gridView.PageIndexChanging +=
                new GridViewPageEventHandler(GridView_PageIndexChanging);
            gridView.AllowPaging = true;

            if (Page.IsPostBack)
            {
                AddActionLinksOnPostback();
            }
        }

        #region Client side events

        /// <summary>
        /// Gets or sets n event handler that gets fired when an event occurs
        /// within a row of the data grid.
        /// </summary>
        ///
        /// <remarks>
        /// The JavaScript row event handler should be in the form of:
        /// function Foo(thingId, thingTypeId, thingVersion, eventType, row, event)
        /// thingId = the Id of the thing
        /// thingTypeId = the type id of the thing type
        /// thingVersion = the version stamp of the thing type
        /// eventType = onmouseover, onmouseout, onclick
        /// row = the row index that the event occurred on.
        /// event = javascript event
        /// </remarks>
        ///
        public string JavaScriptRowEventHandler
        {
            get { return _jsRowEventHandler; }
            set { _jsRowEventHandler = value; }
        }
        private string _jsRowEventHandler;

        private void OnRowDataBound(
            object sender,
            GridViewRowEventArgs e)
        {
            BindJavaScriptRowEventHandler(e.Row);

            if (e.Row.RowType == DataControlRowType.DataRow &&
                ShowIsPersonalFlag)
            {
                int rowIndex = gridView.PageIndex * gridView.PageSize + e.Row.RowIndex;
                Object isPersonalValue = _wcDataTable.Rows[rowIndex]["wc-ispersonal"];
                bool isPersonal = false;
                if (isPersonalValue != null)
                {
                    string isPersonalAsString = isPersonalValue as string;
                    if (isPersonalAsString != null)
                    {
                        isPersonal = String.Equals(isPersonalAsString, "true", StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        isPersonal = (bool)isPersonalValue;
                    }
                }

                if (isPersonal)
                {
                    // Note: e.Row.Cells[0].Text has already been encoded by the BoundField, so
                    // it is safe to append it as is to the personal text. Setting the text now
                    // will NOT invoke another HtmlEncode, so personal text can be html and will
                    // render properly
                    e.Row.Cells[0].Text = IsPersonalText + e.Row.Cells[0].Text;
                }
            }
        }

        private const string IsPersonalText = "<span id=\"personalText\" >Personal</span> ";

        private void BindJavaScriptRowEventHandler(GridViewRow row)
        {
            // If nothing is set or row is null return
            if (String.IsNullOrEmpty(_jsRowEventHandler) ||
                row == null || row.Cells.Count == 0)
            {
                return;
            }

            // If this row is a table header cell we don't want these
            // events on the header row
            if (row.Cells[0] is DataControlFieldHeaderCell)
            {
                return;
            }

            DataRowView dataRow = row.DataItem as DataRowView;
            if (dataRow == null ||
                !dataRow.DataView.Table.Columns.Contains("wc-id"))
            {
                return;
            }

            string thingId =
                dataRow.Row["wc-id"].ToString();

            string thingTypeId =
                dataRow.Row["wc-typeid"].ToString();

            string version = String.Empty;
            if (dataRow.DataView.Table.Columns.Contains("wc-version"))
                version = dataRow.Row["wc-version"].ToString();

            // Add the 3 events we support onto the row.
            string mouseOverScript =
                String.Join(
                    String.Empty,
                    new string[] {
                        _jsRowEventHandler,
                        "('",
                        thingId,
                        "','",
                        thingTypeId,
                        "','",
                        version,
                        "','onmouseover', this, event);"});

            string mouseOutScript =
                String.Join(
                    String.Empty,
                    new string[] {
                        _jsRowEventHandler,
                        "('",
                        thingId,
                        "','",
                        thingTypeId,
                        "','",
                        version,
                        "','onmouseout', this, event);"});

            string onClickScript =
                String.Join(
                    String.Empty,
                    new string[] {
                        _jsRowEventHandler,
                        "('",
                        thingId,
                        "','",
                        thingTypeId,
                        "','",
                        version,
                        "','onclick', this, event);"});

            row.Attributes.Add("onmouseover", mouseOverScript);
            row.Attributes.Add("onmouseout", mouseOutScript);
            row.Attributes.Add("onclick", onClickScript);
        }

        #endregion Client side events

        #region Server-side actions

        /// <summary>
        /// Gets or sets the header text for the action links column.
        /// </summary>
        ///
        /// <remarks>
        /// This value is ignored unless <see cref="ActionLabels"/> and
        /// <see cref="ActionCommands"/> are also set.<br/>
        /// <br/>
        /// Actions are server-side events that can be configured through
        /// attributes of the HealthRecordItemDataGrid element in an aspx
        /// file. Actions consist of text which gets displayed as a link,
        /// a command identifier, and an action event handler to call to
        /// handle the action when clicked. When an action is specified a
        /// column is automatically added to the data grid and the action
        /// link is added to the column.<br/>
        /// <br/>
        /// The ActionHeaderText is the text used for the column that is
        /// automatically added.
        /// </remarks>
        ///
        public string ActionHeaderText
        {
            get { return _actionHeaderText; }
            set { _actionHeaderText = value; }
        }
        private string _actionHeaderText;

        /// <summary>
        /// Gets or sets the action links separator.
        /// </summary>
        ///
        /// <remarks>
        /// This value is ignored unless <see cref="ActionLabels"/> and
        /// <see cref="ActionCommands"/> are also set.<br/>
        /// <br/>
        /// Actions are server-side events that can be configured through
        /// attributes of the HealthRecordItemDataGrid element in an aspx
        /// file. Actions consist of text which gets displayed as a link,
        /// a command identifier, and an action event handler to call to
        /// handle the action when clicked. When an action is specified a
        /// column is automatically added to the data grid and the action
        /// link is added to the column.<br/>
        /// <br/>
        /// The ActionSpacer is the HTML used to separate multiple actions
        /// in the added column. The default value is a single space.
        /// </remarks>
        ///
        public string ActionSpacer
        {
            get { return _actionSpacer; }
            set { _actionSpacer = value; }
        }
        private string _actionSpacer = " ";

        /// <summary>
        /// Gets or sets the action link text.
        /// </summary>
        ///
        /// <remarks>
        /// This value is ignored unless
        /// <see cref="ActionCommands"/> is also set.<br/>
        /// <br/>
        /// Actions are server-side events that can be configured through
        /// attributes of the HealthRecordItemDataGrid element in an aspx
        /// file. Actions consist of text which gets displayed as a link,
        /// a command identifier, and an action event handler to call to
        /// handle the action when clicked. When an action is specified a
        /// column is automatically added to the data grid and the action
        /// link is added to the column.<br/>
        /// <br/>
        /// The ActionLabels is a comma separated list that must have the same
        /// number of values as the <see cref="ActionCommands"/> property.
        /// Each label corresponds to the command at the same index in the
        /// <see cref="ActionCommands"/> property.
        /// </remarks>
        ///
        public string ActionLabels
        {
            get { return _csvActionLabels; }
            set
            {
                _csvActionLabels = value;
                _actionLabels = _csvActionLabels.Split(',');
            }
        }
        private string _csvActionLabels;
        private string[] _actionLabels;

        /// <summary>
        /// Gets or sets the action commands.
        /// </summary>
        ///
        /// <remarks>
        /// This value is ignored unless
        /// <see cref="ActionLabels"/> is also set.<br/>
        /// <br/>
        /// Actions are server-side events that can be configured through
        /// attributes of the HealthRecordItemDataGrid element in an aspx
        /// file. Actions consist of text which gets displayed as a link,
        /// a command identifier, and an action event handler to call to
        /// handle the action when clicked. When an action is specified a
        /// column is automatically added to the data grid and the action
        /// link is added to the column.<br/>
        /// <br/>
        /// The ActionCommands is a comma separated list that must have the same
        /// number of values as the <see cref="ActionLabels"/> property.
        /// Each command corresponds to the label at the same index in the
        /// <see cref="ActionLabels"/> property. When the action link is
        /// clicked <see cref="Action"/> event is fired with the action command
        /// as an argument.
        /// </remarks>
        ///
        public string ActionCommands
        {
            get { return _csvActionCommands; }
            set
            {
                _csvActionCommands = value;
                _actionCommands = _csvActionCommands.Split(',');
            }
        }
        private string _csvActionCommands;
        private string[] _actionCommands;

        /// <summary>
        /// The event handler for client-side action events.
        /// </summary>
        ///
        /// <remarks>
        /// If present, this javascript function is called when action links
        /// are clicked before the server-side <see cref="Action"/>
        /// handler.<br/>
        /// <br/>
        /// The function should return true to proceed, or false to abort
        /// calling the server-side handler.<br/>
        /// <br/>
        /// prototype is handler(event, action, thing_key)<br/>
        /// <ul>
        /// <li>event = javascript event</li>
        /// <li>action = action command that was clicked</li>
        /// <li>key = thing id and version stamp, comma-separated</li>
        /// </ul>
        /// </remarks>
        ///
        public string OnClientAction
        {
            get { return (_clientActionFunction); }
            set { _clientActionFunction = value; }
        }
        private string _clientActionFunction;

        /// <summary>
        /// The event handler for the server-side action event.
        /// </summary>
        ///
        /// <remarks>
        /// The Action event is fired in response to an action link being
        /// clicked. The Action event can be handled by associating a method
        /// with the Action property in the HealthRecordItemDataGrid element
        /// in the aspx page.<br/>
        /// <br/>
        /// The action command is passed to the event handler as a parameter.
        /// </remarks>
        ///
        public event CommandEventHandler Action;

        /// <summary>
        /// The event handler for a "last chance" look at the grid before
        /// it is rendered.
        /// </summary>
        ///
        /// <remarks>
        /// This event is fired just before rendering. The grid is full and
        /// all actions are complete.<br/>
        /// <br/>
        /// Note that using this event requires looking directly into the
        /// contents of the control! This is not guaranteed to be supported
        /// in future versions of the SDK and therefore you should use it
        /// at your own risk.
        /// </remarks>
        ///
        public event EventHandler FinalPreRender;

        /// <summary>
        /// The default event handler for the server-side action event.
        /// </summary>
        ///
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        ///
        /// <param name="e">
        /// A <see cref="System.Web.UI.WebControls.CommandEventArgs"/> that
        /// contains the event data.
        /// </param>
        ///
        /// <remarks>
        /// This method handles the built-in actions like "_wcDelete" and then
        /// passes on any unknown commands to the <see cref="Action"/>
        /// event handler.
        /// </remarks>
        ///
        public void HandleAction(Object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case BuiltinActionDelete:
                    string[] identifiers =
                        e.CommandArgument.ToString().Split(',');
                    this.DeleteThing(
                        new Guid(identifiers[0]),
                        new Guid(identifiers[1]));
                    break;

                default:
                    if (Action != null)
                    {
                        Action(sender, e);
                    }
                    break;
            }
        }

        private const string BuiltinActionDelete = "_wcDelete";

        private void DeleteThing(Guid thingId, Guid versionStamp)
        {
            Record.RemoveItem(new HealthRecordItemKey(thingId, versionStamp));

            this.DataChanged = true;
        }

        [SecurityCritical]
        private void AddActionLinksOnDataBind()
        {
            if (HaveValidActions())
            {
                Validator.ThrowInvalidIf(!Page.EnableViewState, "ActionLinksRequireViewState");

                ActionTemplate actionTemplate = new ActionTemplate(this);

                TemplateField fld = new TemplateField();
                fld.HeaderText =
                    (_actionHeaderText == null) ? "" : _actionHeaderText;
                fld.ItemTemplate = actionTemplate;

                this.gridView.Columns.Add(fld);
            }
        }

        [SecurityCritical]
        private void AddActionLinksOnPostback()
        {
            if (this.HaveValidActions())
            {
                for (int i = 0; i < this.gridView.Rows.Count; ++i)
                {
                    GridViewRow row = this.gridView.Rows[i];
                    TableCell cell = row.Cells[row.Cells.Count - 1];
                    AddActionLinksToContainer(cell, i, null);
                }
            }
        }

        private bool HaveValidActions()
        {
            bool result = true;

            if (_actionLabels == null || _actionCommands == null)
            {
                result = false;
            }
            else
            {
                Validator.ThrowArgumentExceptionIf(
                    _actionLabels.Length != _actionCommands.Length,
                    "actionLabels",
                    "ActionParamLengthsNotEqual");
            }
            return result;
        }

        [SecurityCritical]
        internal void AddActionLinksToContainer(
            Control container,
            int rowIndex,
            ActionTemplate actionTemplate)
        {
            string linkSpacer = _actionSpacer;
            if (linkSpacer == null)
            {
                linkSpacer = " ";
            }

            for (int i = 0; i < _actionLabels.Length; ++i)
            {
                if (i > 0)
                {
                    Literal litSpacer = new Literal();
                    litSpacer.Text = linkSpacer;
                    container.Controls.Add(litSpacer);
                }

                LinkButton link = new LinkButton();
                link.CausesValidation = false;
                link.EnableViewState = true;
                link.Text = _actionLabels[i];
                link.CommandName = _actionCommands[i];
                link.Command += new CommandEventHandler(HandleAction);

                if (_clientActionFunction != null)
                {
                    link.OnClientClick =
                        "return(" + _clientActionFunction +
                        "(event,'" + _actionCommands[i] + "','[KEY]'));";
                }

                if (actionTemplate != null)
                {
                    // data binding ... pick up from there
                    link.DataBinding +=
                        new EventHandler(actionTemplate.OnLinkDataBinding);
                }
                else
                {
                    // restoring from view state ... pick up from DataKeys
                    string key =
                        this.gridView.DataKeys[rowIndex].Value.ToString();

                    link.CommandArgument = key;

                    if (!String.IsNullOrEmpty(link.OnClientClick))
                    {
                        link.OnClientClick =
                            link.OnClientClick.Replace("[KEY]", key);
                    }
                }

                container.Controls.Add(link);
            }
        }

        #endregion Server-side actions

        /// <summary>
        /// Renders the HTML for the HealthRecordItemDataGrid.
        /// </summary>
        ///
        /// <remarks>
        /// The control consists of 3 main elements:
        /// <div class="CssClass">
        ///    <div>
        ///       <table>GridView</table>
        ///    </div>
        ///    <div>Error Message</div>
        /// </div>
        /// </remarks>
        [SecuritySafeCritical]
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            // Verify that the control is nested within a form.
            // This is required for postbacks
            if (Page != null)
            {
                Page.VerifyRenderingInServerForm(this);
            }

            writer.WriteBeginTag("div");

            writer.WriteAttribute("id", this.ClientID);

            if (!String.IsNullOrEmpty(_cssClass))
            {
                writer.WriteAttribute("class", _cssClass);
            }

            writer.Write(HtmlTextWriter.TagRightChar);

            RenderChildren(writer);

            if (_showErrorMessages)
            {
                if (_numResults == 0)
                {
                    writer.WriteBeginTag("div");
                    writer.Write(HtmlTextWriter.TagRightChar);

                    writer.Write(
                        ResourceRetriever.GetResourceString(
                            "NoResultsFound"));
                    writer.WriteEndTag("div");
                }

                if (_isFiltered)
                {
                    writer.WriteBeginTag("div");
                    writer.Write(HtmlTextWriter.TagRightChar);

                    writer.Write(
                        ResourceRetriever.GetResourceString(
                            "FilteredResults"));
                    writer.WriteEndTag("div");
                }
            }

            writer.WriteEndTag("div");
        }

        /// <summary>
        /// Populates the encapsulated GridView by building the search filters
        /// or use the FilterOverride to get health record information.
        /// </summary>
        ///
        /// <remarks>
        /// To get information on results returned, look at the
        /// <see cref="IsFiltered"/> and <see cref="ResultCount"/>
        /// properties.
        /// </remarks>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the <see cref="HealthRecordItemDataGrid"/> instance is placed in
        /// a page not deriving from <see cref="HealthServicePage"/>.
        /// </exception>
        ///
        [SecurityCritical]
        protected void PopulateGridView()
        {
            // Clear out the GridView
            gridView.Columns.Clear();
            gridView.DataSource = null;
            gridView.DataBind();

            HealthRecordFilter filter = _filterOverride;

            if (filter == null)
            {
                filter = this.BuildFilterFromParams();
            }

            filter.MaxFullItemsReturnedPerRequest = PageSize;

            _wcDataTable = new HealthRecordItemDataTable(_tableView, filter);
            _wcDataTable.GetData(Record, PageIndex * PageSize, PageSize);

            this._showIsSignedColumn = this._showIsSignedColumn && _wcDataTable.HasSignedData;

            List<string> visibleColumns = new List<string>();
            IEnumerable dataColumns = _wcDataTable.DisplayColumns.Values;
            if (String.Equals(
                    _visibleColumns,
                    "*",
                    StringComparison.Ordinal))
            {
                dataColumns = _wcDataTable.Columns;
            }
            else
            {
                if (!String.IsNullOrEmpty(_visibleColumns))
                {
                    string[] separatedVisibleColumns = _visibleColumns.Split(',');
                    visibleColumns.AddRange(separatedVisibleColumns);
                    dataColumns = _wcDataTable.Columns;
                }
            }

            if (visibleColumns.Count > 0)
            {
                // Show the columns in order and using a header name if
                // supplied
                foreach (string visibleColumnName in visibleColumns)
                {
                    string[] columnSplit = visibleColumnName.Split(':');

                    string columnName = columnSplit[0];
                    string columnHeader = columnName;

                    if (columnSplit.Length > 1)
                    {
                        columnHeader = columnSplit[1];
                    }

                    foreach (DataColumn cdef in dataColumns)
                    {
                        if (!columnName.Equals(
                                cdef.ColumnName,
                                StringComparison.Ordinal))
                        {
                            continue;
                        }
                        AddDisplayColumn(
                            cdef,
                            columnHeader,
                            filter);
                    }
                }
            }
            else
            {
                foreach (DataColumn cdef in dataColumns)
                {
                    AddDisplayColumn(
                        cdef,
                        cdef.Caption,
                        filter);
                }
            }

            if (ShowActionLinks)
            {
                AddActionLinksOnDataBind();
            }

            gridView.DataSource = _wcDataTable;
            gridView.DataBind();

            DataChanged = false;
            _isFiltered = _wcDataTable.WasFiltered;
            _numResults = _wcDataTable.Rows.Count;
        }

        [SecurityCritical]
        private void AddDisplayColumn(
            DataColumn cdef,
            string columnHeader,
            HealthRecordFilter filter)
        {
            bool visible = true;

            ItemTypeDataColumn typeColumn =
                cdef as ItemTypeDataColumn;
            if (typeColumn != null)
            {
                visible = typeColumn.VisibleByDefault;
            }

            DataControlField fld = null;

            if (cdef.ColumnName == HealthRecordItemDataGrid.WCIsSignedAttributeName)
            {
                if (!this._showIsSignedColumn)
                {
                    visible = false;
                }
                else if (_isSignedColumnValueOverride.Length > 0)
                {
                    TemplateField tfield = new TemplateField();

                    tfield.ItemTemplate =
                        new GridViewIsSignedTemplate(
                            TemplateType.Item,
                            cdef.ColumnName,
                            columnHeader,
                            _isSignedColumnValueOverride);

                    tfield.HeaderTemplate =
                        new GridViewIsSignedTemplate(
                            TemplateType.Header,
                            cdef.ColumnName,
                            columnHeader,
                            _isSignedColumnValueOverride);

                    fld = tfield;
                    visible = true;
                }
            }

            // For auditing we want to do special columns
            if (ShowAuditColumns)
            {
                switch (cdef.ColumnName)
                {
                    case "wc-date":
                        visible = false;
                        break;

                    case "wc-source":
                        visible = false;
                        break;

                    case "wc-audit-date":
                        visible = true;
                        break;

                    case "wc-audit-personname":
                        // If the filter is filtered by a person, don't
                        // show the person column.
                        visible = filter.UpdatedPerson == Guid.Empty;
                        break;

                    case "wc-audit-appname":
                        // If the filter is filtered by a application,
                        // don't show the application column.
                        visible = filter.UpdatedApplication == Guid.Empty;
                        break;

                    case "wc-audit-action":
                        // For an action, we need to use a special
                        // templated field. The templated field overrides
                        // data binding and will convert the enum string
                        // to a string we want to show.
                        TemplateField tfield = new TemplateField();

                        tfield.ItemTemplate =
                            new GridViewAuditActionTemplate(
                                TemplateType.Item,
                                cdef.ColumnName,
                                columnHeader);

                        tfield.HeaderTemplate =
                            new GridViewAuditActionTemplate(
                                TemplateType.Header,
                                cdef.ColumnName,
                                columnHeader);

                        fld = tfield;
                        visible = true;
                        break;
                }
            }

            if (fld == null)
            {
                BoundField genericField = new BoundField
                {
                    DataField = cdef.ColumnName,
                    HeaderText = columnHeader
                };

                fld = genericField;
            }

            fld.HeaderStyle.Wrap = false;
            fld.Visible = visible;

            if (typeColumn != null)
            {
                fld.HeaderStyle.Width = Unit.Pixel(typeColumn.ColumnWidth);
            }
            gridView.Columns.Add(fld);
        }

        private void GridView_PageIndexChanging(
            object sender,
            GridViewPageEventArgs e)
        {
            gridView.PageIndex = e.NewPageIndex;
            DataChanged = true;
        }

        /// <summary>
        /// Builds the filter used to populate the data grid. The filter is
        /// generated by looking at the TypeIds.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="Microsoft.HealthVault.HealthRecordFilter"/> that is used
        /// by the underlying
        /// <see cref="Microsoft.HealthVault.HealthRecordItemDataTable"/>
        /// to retrieve data for the data grid.
        /// </returns>
        ///
        private HealthRecordFilter BuildFilterFromParams()
        {
            HealthRecordFilter filter = new HealthRecordFilter();
            foreach (Guid typeId in TypeIds)
            {
                filter.TypeIds.Add(typeId);
            }
            return filter;
        }

        private readonly GridView gridView;

        /// <summary>
        /// Gets or sets the filter to use to get information from the
        /// health record.
        /// </summary>
        ///
        /// <remarks>
        /// The data grid can either show health record items of specific
        /// types specified by the <see cref="TypeIds"/> property or by
        /// specifying a specific filter.<br/>
        /// <br/>
        /// If this property is set the <see cref="TypeIds"/> property will
        /// be ignored.
        /// </remarks>
        ///
        public HealthRecordFilter FilterOverride
        {
            get { return _filterOverride; }
            set { _filterOverride = value; }
        }
        private HealthRecordFilter _filterOverride;

        /// <summary>
        /// Gets or sets the unique type identifier for the health record
        /// items to show in the data grid.
        /// </summary>
        ///
        /// <remarks>
        /// The data grid can either show health record items of specific
        /// types or by specifying a more detailed filter in the
        /// <see cref="FilterOverride"/> property.<br/>
        /// <br/>
        /// If the <see cref="FilterOverride"/> property is set, this property
        /// is ignored.
        /// </remarks>
        ///
        public ICollection<Guid> TypeIds => _typeIds;

        private readonly List<Guid> _typeIds = new List<Guid>();

        /// <summary>
        /// Gets the HealthVault record to use.
        /// </summary>
        ///
        /// <remarks>
        /// If the value is <b>null</b> the property will attempt to use the page property of
        /// the control to obtain the selected record. This will only work if the page is derived
        /// from <see cref="HealthServicePage"/>.
        /// </remarks>
        ///
        public HealthRecordAccessor Record
        {
            get
            {
                if (_record == null)
                {
                    HealthServicePage wcPage = this.Page as HealthServicePage;

                    if (wcPage != null)
                    {
                        return wcPage.PersonInfo.SelectedRecord;
                    }
                }
                return _record;
            }
            set { _record = value; }
        }
        private HealthRecordAccessor _record;

        /// <summary>
        /// Gets a value that indicates whether the result data has been
        /// filtered by HealthVault.
        /// </summary>
        ///
        /// <remarks>
        /// To use this property, override the
        /// <see cref="System.Web.UI.Page.OnPreRenderComplete"/>, call the
        /// base class implementation, and then check if the data was filtered.
        /// </remarks>
        ///
        public bool IsFiltered => _isFiltered;

        private bool _isFiltered;

        /// <summary>
        /// Gets or sets the type of view the data table exposes through the
        /// data grid.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is
        /// <see cref="HealthRecordItemDataTableView.Default"/>.
        /// </remarks>
        ///
        public HealthRecordItemDataTableView TableView
        {
            get { return _tableView; }
            set { _tableView = value; }
        }
        private HealthRecordItemDataTableView _tableView;

        /// <summary>
        /// Gets or sets a value which indicates to the data grid that data
        /// has changed and it should be refreshed.
        /// </summary>
        ///
        public bool DataChanged
        {
            get { return _dataChanged; }
            set { _dataChanged = value; }
        }
        private bool _dataChanged;

        /// <summary>
        /// Gets the number of results returned in the data grid.
        /// </summary>
        ///
        /// <remarks>
        /// To provide alternate UI when no results are returned, override
        /// the <see cref="System.Web.UI.Page.OnPreRenderComplete"/>, call the
        /// base class implementation, and then check this property.
        /// </remarks>
        ///
        public int ResultCount
        {
            get { return _numResults; }
        }
        private int _numResults;

        /// <summary>
        /// The columns that should be shown in the data grid.
        /// </summary>
        ///
        /// <remarks>
        /// This is a comma separated list of the columns that should be shown
        /// in the data grid. If the value is "*" all data columns will be
        /// shown. If the value is null or empty, the display
        /// columns defined for the specified item type are shown.
        /// The default value is null.
        /// </remarks>
        ///
        public string VisibleColumns
        {
            get { return _visibleColumns; }
            set { _visibleColumns = value; }
        }
        private string _visibleColumns;

        /// <summary>
        /// Gets or sets the CSS class used for the control layout.
        /// </summary>
        ///
        /// <remarks>
        /// <div class="CssClass">
        ///    <div>
        ///       <table>GridView</table>
        ///    </div>
        ///    <div>Error Message</div>
        /// </div>
        /// To control the way the controls look use CSS selector methods
        /// i.e. div.myClass div table tr td to access the td of the grid
        /// </remarks>
        ///
        public string CssClass
        {
            get { return _cssClass; }
            set { _cssClass = value; }
        }
        private string _cssClass;

        /// <summary>
        /// Gets or sets the CSS class used for alternating row styles.
        /// </summary>
        ///
        /// <remarks>
        /// To have alternating row styles for the data grid, the styles
        /// need to be controlled by a separate CSS class and will be attached
        /// to the table rows on Render.
        /// </remarks>
        ///
        public string AlternatingRowCssClass
        {
            get { return _alternatingRowCssClass; }
            set { _alternatingRowCssClass = value; }
        }
        private string _alternatingRowCssClass;

        /// <summary>
        /// Gets or sets the cell padding for the data grid.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is 0.<br/>
        /// This value must be set before <see cref="Render"/> is called.
        /// </remarks>
        ///
        public int CellPadding
        {
            get { return gridView.CellPadding; }
            set { gridView.CellPadding = value; }
        }

        /// <summary>
        /// Gets or sets the cell spacing for the data grid.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is 0.<br/>
        /// This value must be set before <see cref="Render"/> is called.
        /// </remarks>
        ///
        public int CellSpacing
        {
            get { return gridView.CellSpacing; }
            set { gridView.CellSpacing = value; }
        }

        /// <summary>
        /// Gets or sets the number or results that are shown per page.
        /// </summary>
        ///
        /// <remarks>
        /// This value can also be configured using a web.config setting
        /// named "DataGrid_ItemsPerPage".<br/>
        /// This value must be set before <see cref="Render"/> is called.
        /// </remarks>
        ///
        public int PageSize
        {
            get { return gridView.PageSize; }
            set { gridView.PageSize = value; }
        }

        /// <summary>
        /// Gets or sets the page index to be shown.
        /// </summary>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the PageIndex property is set to a value less than 0.
        /// </exception>
        ///
        /// <remarks>
        /// This value must be set before <see cref="Render"/> is called.
        /// </remarks>
        ///
        public int PageIndex
        {
            get { return gridView.PageIndex; }
            set { gridView.PageIndex = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the data grid should
        /// automatically show error messages when no results are found or
        /// when the results are filtered.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is false.
        /// </remarks>
        ///
        public bool ShowErrorMessages
        {
            get { return _showErrorMessages; }
            set { _showErrorMessages = value; }
        }
        private bool _showErrorMessages;

        /// <summary>
        /// Gets the client ID of the grid view portion of the control.
        /// </summary>
        ///
        /// <remarks>
        /// This is the ID of the &lt;table/&gt; element in the control so that
        /// it may be accessed using client side scripting or CSS.
        /// </remarks>
        ///
        public string GridViewClientId
        {
            get { return gridView.ClientID; }
        }

        /// <summary>
        /// Gets or sets a value which indicates to the data grid whether or not to show the
        /// signed column if the column is defined for the specified item type.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is false.
        /// </remarks>
        ///
        public bool ShowIsSignedColumn
        {
            get { return _showIsSignedColumn; }
            set { _showIsSignedColumn = value; }
        }
        private bool _showIsSignedColumn;

        /// <summary>
        /// Gets or sets a value shown in the signed column if the column is defined for the
        /// specified item type.
        /// </summary>
        ///
        /// <remarks>
        /// Instead of showing 'True' when item is signed, we could show an image by overriding
        /// the signed column value.
        /// </remarks>
        ///
        public string IsSignedColumnValueOverride
        {
            get { return _isSignedColumnValueOverride; }
            set { _isSignedColumnValueOverride = value; }
        }
        private string _isSignedColumnValueOverride = String.Empty;
        private const string WCIsSignedAttributeName = "wc-issigned";

        /// <summary>
        /// Gets or sets a value which indicates to the data grid whether or not to show the
        /// personal flag for items marked as personal.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is false.
        /// </remarks>
        ///
        public bool ShowIsPersonalFlag
        {
            get { return _showIsPersonalFlag; }
            set { _showIsPersonalFlag = value; }
        }
        private bool _showIsPersonalFlag;

        /// <summary>
        /// Gets or sets a value which indicates to the data grid whether or not to show the
        /// audit columns for items.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is false.
        /// </remarks>
        ///
        public bool ShowAuditColumns
        {
            get { return _showAuditColumns; }
            set { _showAuditColumns = value; }
        }
        private bool _showAuditColumns;

        /// <summary>
        /// Gets or sets a value which indicates to the data grid whether or not to show the
        /// action links in each row.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        ///
        public bool ShowActionLinks
        {
            get { return _showActionLinks; }
            set { _showActionLinks = value; }
        }
        private bool _showActionLinks = true;

        private HealthRecordItemDataTable _wcDataTable;
    }
}
