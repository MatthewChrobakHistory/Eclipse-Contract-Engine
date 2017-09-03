using Server.IO;
using System.IO;
using System.Xml.Serialization;

namespace Server.Data.Models.Maps
{
    public class Map
    {
        [XmlIgnore]
        public static readonly string MapPath = Program.DataPath + "maps\\";
        [XmlIgnore]
        public static readonly System.Type Type = new Map().GetType();

        [XmlIgnore]
        public const int MapWidth = 30;
        [XmlIgnore]
        public const int MapHeight = 20;

        public string Name;
        public byte[] Attributes;
        public Layer[] Layers;

        public static void LoadMaps() {
            foreach (string file in Directory.GetFiles(MapPath, "*.xml")) {
                DataManager.Maps.Add(Serialization.Deserialize<Map>(file, Map.Type));
            }
        }
    }
}
