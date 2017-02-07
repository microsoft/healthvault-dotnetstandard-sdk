// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates an allergy.
    /// </summary>
    /// 
    public class Allergy : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Allergy"/> class with default 
        /// values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public Allergy()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Allergy"/> class 
        /// specifying the mandatory values.
        /// </summary>
        /// 
        /// <param name="name">
        /// The name of the allergy.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public Allergy(CodableValue name)
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
        public new static readonly Guid TypeId = 
            new Guid("52bf9104-2c5e-4f1f-a66d-552ebcc53df7");

        /// <summary>
        /// Populates this <see cref="Allergy"/> instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the allergy data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// an allergy node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            _name.Clear();
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("allergy");

            Validator.ThrowInvalidIfNull(itemNav, "AllergyUnexpectedNode");

            // <name>
            _name.ParseXml(itemNav.SelectSingleNode("name"));

            // <reaction>
            _reaction =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "reaction");

            // <first-observed>
            _firstObserved =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(
                    itemNav,
                    "first-observed");

            // <allergen-type>
            _allergen =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "allergen-type");
            
            // <allergen-code>
            _allergenCode =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "allergen-code");
            
            // <treatment-provider>
            _treatmentProvider =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "treatment-provider");

            // <treatment>
            _treatment =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "treatment");
            
            // <is-negated>
            _isNegated = 
                XPathHelper.GetOptNavValueAsBool(itemNav, "is-negated");

        }

        /// <summary>
        /// Writes the allergy data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the allergy data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Name"/> has an <b>null</b> Text property.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");
            Validator.ThrowSerializationIfNull(_name.Text, "AllergyNameMandatory");

            // <allergy>
            writer.WriteStartElement("allergy");

            // <name>
            _name.WriteXml("name", writer);

            // <reaction>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer, 
                "reaction", 
                Reaction);

            // <first-observed>
            XmlWriterHelper.WriteOpt<ApproximateDateTime>(
                writer,
                "first-observed",
                FirstObserved);

            // <allergen-type>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer, 
                "allergen-type", 
                AllergenType);
                
            // <allergen-code>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "allergen-code",
                AllergenCode);

            // <treatment-provider>
            XmlWriterHelper.WriteOpt<PersonItem>(
                writer,
                "treatment-provider",
                TreatmentProvider);
         
            // <treatment>
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "treatment",
                Treatment);

            // <is-negated>
            XmlWriterHelper.WriteOptBool(
                writer,
                "is-negated",
                _isNegated);


            // </allergy>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the allergy.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="CodableValue"/> representing the name.
        /// </value>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> on set.
        /// </exception>
        /// 
        public CodableValue Name
        {
            get { return _name; }
            set 
            {
                Validator.ThrowIfArgumentNull(value, "Name", "AllergyNameMandatory");
                _name = value; 
            }
        }
        private CodableValue _name = new CodableValue();

        /// <summary>
        /// Gets or sets the reaction the person has.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="CodableValue"/> representing the reaction.
        /// </value>
        /// 
        public CodableValue Reaction
        {
            get { return _reaction; }
            set { _reaction = value; }
        }
        private CodableValue _reaction;

        /// <summary>
        /// Gets or sets the approximate date of the first occurrence of the
        /// allergy.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="ApproximateDateTime"/> representing the date.
        /// </value>
        /// 
        /// <remarks>
        /// An approximate date must have a year and may also have the month
        /// and/or day.
        /// </remarks>
        /// 
        public ApproximateDateTime FirstObserved
        {
            get { return _firstObserved; }
            set { _firstObserved = value; }
        }
        private ApproximateDateTime _firstObserved;


        /// <summary>
        /// Gets or sets the type of allergen that causes an allergic 
        /// reaction.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="CodableValue"/> representing  the allergen category.
        /// </value>
        /// 
        /// <remarks>
        /// Examples include medication such as penicillin and sulfonamides, 
        /// food such as peanuts, shellfish, and wheat, reactions to animals such 
        /// as bee stings, dogs, or cats, plants such as ragweed or birch, and 
        /// environmental things such as smoke, smog, or dust.
        /// <br/><br/>
        /// Codes for this value are in the "allergen-type" 
        /// vocabulary.
        /// </remarks>
        /// 
        public CodableValue AllergenType
        {
            get { return _allergen; }
            set { _allergen = value; }
        }
        private CodableValue _allergen;
        
        /// <summary>
        /// Gets or sets the code for the allergen that causes an allergic 
        /// reaction.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="CodableValue"/> representing the allergen category.
        /// </value>
        /// 
        /// <remarks>
        /// Examples include medication such as penicillin and sulfonamides, 
        /// food such as peanuts, shellfish, and wheat, reactions to animals such 
        /// as bee stings, dogs, or cats, plants such as ragweed or birch, and 
        /// environmental things such as smoke, smog, or dust.
        /// <br/><br/>
        /// Codes for this value are in the "allergen-type" 
        /// vocabulary.
        /// </remarks>
        /// 
        public CodableValue AllergenCode
        {
            get { return _allergenCode; }
            set { _allergenCode = value; }
        }
        private CodableValue _allergenCode;
        
        /// <summary>
        /// Gets or sets information about the treatment provider for this allergy.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="PersonItem"/> representing the provider.
        /// </value>
        /// 
        public PersonItem TreatmentProvider
        {
            get { return _treatmentProvider; }
            set { _treatmentProvider = value; }
        }
        private PersonItem _treatmentProvider;

        /// <summary>
        /// Gets or sets a possible treatment method for this allergy.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="CodableValue"/> representing the treatment method.
        /// </value>
        /// 
        public CodableValue Treatment
        {
            get { return _treatment; }
            set { _treatment = value; }
        }
        private CodableValue _treatment;

        /// <summary>
        /// Gets or sets a value indicating whether the allergic reaction is 
        /// negated with treatment.
        /// </summary>
        /// 
        /// <remarks>
        /// <b>true</b> if the allergic reation is negated with treatment; 
        /// otherwise, <b>false</b>.
        /// </remarks>
        /// 
        public bool? IsNegated
        {
            get { return _isNegated; }
            set { _isNegated = value; }
        }
        private bool? _isNegated;

        /// <summary>
        /// Gets a string representation of the allergy item.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the allergy item.
        /// </returns>
        /// 
        public override string ToString()
        {
            return Name.Text;
        }       
    }
}
