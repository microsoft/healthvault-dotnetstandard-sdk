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
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a question and answer stored as a thing.
    /// </summary>
    ///
    public class QuestionAnswer : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="QuestionAnswer"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public QuestionAnswer()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="QuestionAnswer"/> class with the specified
        /// date and time and the question that was asked.
        /// </summary>
        ///
        /// <param name="when">
        /// The date and time the question was asked.
        /// </param>
        ///
        /// <param name="question">
        /// The question that was asked.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/> or <paramref name="question"/> is <b>null</b>.
        /// </exception>
        ///
        public QuestionAnswer(HealthServiceDateTime when, CodableValue question)
            : base(TypeId)
        {
            this.When = when;
            this.Question = question;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="QuestionAnswer"/> class with the specified
        /// date and time and the question that was asked.
        /// </summary>
        ///
        /// <param name="when">
        /// The date and time the question was asked.
        /// </param>
        ///
        /// <param name="question">
        /// The question that was asked.
        /// </param>
        ///
        /// <param name="answerChoice">
        /// The possible answers to the question. See <see cref="AnswerChoice"/> for more information
        /// and the preferred vocabulary.
        /// </param>
        ///
        /// <param name="answer">
        /// The answer to the question.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/> or <paramref name="question"/> is <b>null</b>.
        /// </exception>
        ///
        public QuestionAnswer(
            HealthServiceDateTime when,
            CodableValue question,
            IList<CodableValue> answerChoice,
            IList<CodableValue> answer)
            : this(when, question)
        {
            if (answerChoice != null)
            {
                foreach (CodableValue choice in answerChoice)
                {
                    this.answerChoice.Add(choice);
                }
            }

            if (answer != null)
            {
                foreach (CodableValue answerValue in answer)
                {
                    this.answer.Add(answerValue);
                }
            }
        }

        /// <summary>
        /// The unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("55d33791-58de-4cae-8c78-819e12ba5059");

        /// <summary>
        /// Populates this question/answer instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the question/answer data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a question-answer node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("question-answer");

            Validator.ThrowInvalidIfNull(itemNav, Resources.QuestionAnswerUnexpectedNode);

            // <when>
            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            // <question>
            this.question = new CodableValue();
            this.question.ParseXml(itemNav.SelectSingleNode("question"));

            // <answer-choice>
            this.answerChoice.Clear();

            XPathNodeIterator choiceIterator = itemNav.Select("answer-choice");
            foreach (XPathNavigator choiceNav in choiceIterator)
            {
                CodableValue choice = new CodableValue();
                choice.ParseXml(choiceNav);
                this.answerChoice.Add(choice);
            }

            // <answer>
            this.answer.Clear();

            XPathNodeIterator answerIterator = itemNav.Select("answer");
            foreach (XPathNavigator answerNav in answerIterator)
            {
                CodableValue answer = new CodableValue();
                answer.ParseXml(answerNav);
                this.answer.Add(answer);
            }
        }

        /// <summary>
        /// Writes the question/answer data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the question/answer data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// The <see cref="When"/> or <see cref="Question"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.when, Resources.WhenNullValue);
            Validator.ThrowSerializationIfNull(this.question, Resources.QuestionAnswerQuestionNotSet);

            writer.WriteStartElement("question-answer");

            this.when.WriteXml("when", writer);

            this.question.WriteXml("question", writer);

            foreach (CodableValue choice in this.answerChoice)
            {
                choice.WriteXml("answer-choice", writer);
            }

            foreach (CodableValue answer in this.answer)
            {
                answer.WriteXml("answer", writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time of the question was asked.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date
        /// and time.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private HealthServiceDateTime when;

        /// <summary>
        /// Gets or sets the question that was asked.
        /// </summary>
        ///
        /// <remarks>
        /// A list of vocabularies may be found in the "question-sets".
        /// Contact the HealthVault team to help define the preferred vocabulary.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Question
        {
            get { return this.question; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Question), Resources.QuestionAnswerQuestionMandatory);
                this.question = value;
            }
        }

        private CodableValue question;

        /// <summary>
        /// Gets a collection of the possible answers to the question.
        /// </summary>
        ///
        /// <remarks>
        /// Questions that are answered using free-form text will not specify answer choices.
        /// The answer vocabulary used is typically one related to the vocabulary used for the question.
        /// For example, the choice for "High blood pressure" would code for that condition.
        /// Standard answers (such as yes/no) can be coded using the preferred vocabulary
        /// "answer-choice-sets".
        /// </remarks>
        ///
        public Collection<CodableValue> AnswerChoice => this.answerChoice;

        private readonly Collection<CodableValue> answerChoice = new Collection<CodableValue>();

        /// <summary>
        /// Gets a collection of the recorded answer(s) to the question.
        /// </summary>
        ///
        /// <remarks>
        /// In many cases, the coding of the answer is identical to the coding of the
        /// selected <see cref="AnswerChoice"/>.
        /// </remarks>
        ///
        public Collection<CodableValue> Answer => this.answer;

        private readonly Collection<CodableValue> answer = new Collection<CodableValue>();

        /// <summary>
        /// Gets a string representation of the question/answer.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the question/answer.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            if (this.question != null)
            {
                result.Append(this.question);
            }

            for (int index = 0; index < this.answer.Count; ++index)
            {
                if (index > 0)
                {
                    result.Append(
                        Resources.GroupSeparator);
                }
                else
                {
                    result.Append(" ");
                }

                CodableValue answer = this.answer[index];
                result.Append(answer);
            }

            return result.ToString();
        }
    }
}
