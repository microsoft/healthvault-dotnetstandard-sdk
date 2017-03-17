// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Information related to a care plan and tasks, goals associated with the care plan.
    /// </summary>
    ///
    /// <remarks>
    /// The care plan contains tasks and goals.
    /// </remarks>
    ///
    public class CarePlan : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CarePlan"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public CarePlan()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CarePlan"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        /// <param name="name">
        /// Name of the care plan.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <b>null</b>, empty or contains only whitespace.
        /// </exception>
        ///
        public CarePlan(string name)
        : base(TypeId)
        {
            this.Name = name;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("415c95e0-0533-4d9c-ac73-91dc5031186c");

        /// <summary>
        /// Populates this <see cref="CarePlan"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the CarePlan data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="typeSpecificXml"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a CarePlan node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            Validator.ThrowIfArgumentNull(typeSpecificXml, nameof(typeSpecificXml), Resources.ParseXmlNavNull);

            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("care-plan");

            Validator.ThrowInvalidIfNull(itemNav, Resources.CarePlanUnexpectedNode);

            this.name = itemNav.SelectSingleNode("name").Value;
            this.startDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "start-date");
            this.endDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "end-date");
            this.status = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "status");
            this.carePlanManager = XPathHelper.GetOptNavValue<PersonItem>(itemNav, "care-plan-manager");

            // collections
            this.careTeam = XPathHelper.ParseXmlCollection<PersonItem>(itemNav, "care-team/person");
            this.tasks = XPathHelper.ParseXmlCollection<CarePlanTask>(itemNav, "tasks/task");
            this.goalGroups = XPathHelper.ParseXmlCollection<CarePlanGoalGroup>(itemNav, "goal-groups/goal-group");
        }

        /// <summary>
        /// Writes the XML representation of the CarePlan into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer into which the CarePlan should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Name"/> is <b>null</b> or empty or contains only whitespace.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfArgumentNull(writer, nameof(writer), Resources.WriteXmlNullWriter);

            if (string.IsNullOrEmpty(this.name) || string.IsNullOrEmpty(this.name.Trim()))
            {
                throw new ThingSerializationException(Resources.CarePlanNameNullOrEmpty);
            }

            writer.WriteStartElement("care-plan");
            {
                writer.WriteElementString("name", this.name);
                XmlWriterHelper.WriteOpt(writer, "start-date", this.startDate);
                XmlWriterHelper.WriteOpt(writer, "end-date", this.endDate);
                XmlWriterHelper.WriteOpt(writer, "status", this.status);

                XmlWriterHelper.WriteXmlCollection(writer, "care-team", this.careTeam, "person");

                XmlWriterHelper.WriteOpt(writer, "care-plan-manager", this.carePlanManager);
                XmlWriterHelper.WriteXmlCollection(writer, "tasks", this.tasks, "task");
                XmlWriterHelper.WriteXmlCollection(writer, "goal-groups", this.goalGroups, "goal-group");
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets name of the care plan.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about name the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "Name");
                Validator.ThrowIfStringNullOrEmpty(value, "Name");
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "Name");

                this.name = value;
            }
        }

        private string name;

        /// <summary>
        /// Gets or sets start date of the care plan.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about startDate the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public ApproximateDateTime StartDate
        {
            get
            {
                return this.startDate;
            }

            set
            {
                this.startDate = value;
            }
        }

        private ApproximateDateTime startDate;

        /// <summary>
        /// Gets or sets end date of the care plan.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about endDate the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public ApproximateDateTime EndDate
        {
            get
            {
                return this.endDate;
            }

            set
            {
                this.endDate = value;
            }
        }

        private ApproximateDateTime endDate;

        /// <summary>
        /// Gets or sets status of the care plan.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about status the value should be set to <b>null</b>.
        /// </remarks>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.status = value;
            }
        }

        private CodableValue status;

        /// <summary>
        /// Gets or sets list of person contacts associated with the care plan.
        /// </summary>
        ///
        public Collection<PersonItem> CareTeam => this.careTeam;

        private Collection<PersonItem> careTeam =
            new Collection<PersonItem>();

        /// <summary>
        /// Gets or sets person managing the care plan.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about carePlanManager the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public PersonItem CarePlanManager
        {
            get
            {
                return this.carePlanManager;
            }

            set
            {
                this.carePlanManager = value;
            }
        }

        private PersonItem carePlanManager;

        /// <summary>
        /// Gets or sets list of tasks associated with the care plan.
        /// </summary>
        ///
        public Collection<CarePlanTask> Tasks => this.tasks;

        private Collection<CarePlanTask> tasks =
            new Collection<CarePlanTask>();

        /// <summary>
        /// Gets or sets list of goals associated with the care plan.
        /// </summary>
        ///
        public Collection<CarePlanGoalGroup> GoalGroups => this.goalGroups;

        private Collection<CarePlanGoalGroup> goalGroups =
            new Collection<CarePlanGoalGroup>();

        /// <summary>
        /// Gets a string representation of the CarePlan.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the CarePlan.
        /// </returns>
        ///
        public override string ToString()
        {
            if (this.status == null)
            {
                return this.name;
            }

            return string.Format(
                CultureInfo.CurrentUICulture,
                Resources.CarePlanFormat,
                this.name,
                this.status);
        }
    }
}
