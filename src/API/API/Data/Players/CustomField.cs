using System.Xml.Serialization;

namespace API.Data.Players
{
    public class CustomField
    {
        public CustomField() {
            this.Key = string.Empty;
            this.Value = string.Empty;
        }

        public CustomField(string key, dynamic value) {
            this.Key = key;
            this.Value = value;
        }

        [XmlAttribute("key")]
        public string Key;

        [XmlElement("str", Type = typeof(string))]
        [XmlElement("int", Type = typeof(int))]
        [XmlElement("bool", Type = typeof(bool))]
        [XmlElement("double", Type = typeof(double))]
        public dynamic Value;
    }
}
