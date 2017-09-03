using System.Xml.Serialization;

namespace Client.Data.Models.Maps
{
    public class Map
    {
        [XmlIgnore]
        public const int MapWidth = 30;
        [XmlIgnore]
        public const int MapHeight = 20;

        public string Name;
        public byte[] Attributes;
        public Layer[] Layers;
    }
}
