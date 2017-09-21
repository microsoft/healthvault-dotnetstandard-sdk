using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.HealthVault.ItemTypes;

namespace SandboxUwp
{
    class CustomDataType : ApplicationSpecific
    {
        // Add custom properties here
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public List<double> Values { get; set; }
        // Custom properties end

        private HealthServiceDateTime m_when;

        // HealthVault expects a specific data schema - custom properties are added at the end
        public override void WriteXml(XmlWriter writer)
        {
            m_when = new HealthServiceDateTime();

            writer.WriteStartElement("app-specific");
            {
                writer.WriteStartElement("format-appid");
                writer.WriteValue(Settings.HVAppID);
                writer.WriteEndElement();

                writer.WriteStartElement("format-tag");
                writer.WriteValue("custom-data-type");
                writer.WriteEndElement();

                m_when.WriteXml("when", writer);

                writer.WriteStartElement("summary");
                writer.WriteValue("");
                writer.WriteEndElement();

                // Custom properties start
                writer.WriteStartElement("CustomDataType");
                writer.WriteAttributeString("StartTime", StartTime.ToString());
                writer.WriteAttributeString("EndTime", EndTime.ToString());
                writer.WriteAttributeString("Values", string.Join(",", Values));
                writer.WriteEndElement();

                // Custom properties end
            }
            writer.WriteEndElement();
        }
    }
}
