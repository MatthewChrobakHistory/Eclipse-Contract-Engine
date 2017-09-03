using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace API.Data.Players
{
    public abstract class PlayerPlugin : Entity
    {
        [XmlIgnore]
        private static Dictionary<string, dynamic> _pluginFields = new Dictionary<string, dynamic>();

        [XmlArray("PluginFields")]
        public List<CustomField> PluginFields = new List<CustomField>();

        public PlayerPlugin() {
            foreach (var entry in _pluginFields) {
                PluginFields.Add(new CustomField(entry.Key, entry.Value));
            }
        }

        public dynamic GetField(string key) {
            if (_pluginFields.ContainsKey(key)) {
                return PluginFields.Where((x) => x.Key == key).ToArray()[0].Value;
            } else {
                System.Console.WriteLine("Could not get " + key);
                return "";
            }
        }

        public static void AddField(string key, dynamic value) {
            if (!_pluginFields.ContainsKey(key)) {
                _pluginFields.Add(key, value);
            }
        }
    }
}
