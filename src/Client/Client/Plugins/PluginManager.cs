using System.Collections.Generic;
using System.IO;
using System.Reflection;
using API;
using System.Linq;
using API.Networking;
using API.Scripting;

namespace Client.Plugins
{
    public static class PluginManager
    {
        public static readonly string PluginPath = Program.StartupPath + "plugins\\";
        private static Dictionary<string, IPlugin> _plugins;

        public static void Initialize() {
            PluginManager._plugins = new Dictionary<string, IPlugin>();

            LoadNewPlugins();
        }

        public static void LoadNewPlugins() {
            foreach (string file in Directory.GetFiles(PluginPath, "*.dll")) {
                var asm = Assembly.LoadFile(file);
                var types = asm.GetTypes();


                foreach (var type in types) {
                    if (type.GetInterfaces().Contains(typeof(IPlugin))) {
                        var instance = (IPlugin)asm.CreateInstance(type.ToString());

                        if (type.BaseType == typeof(ClientPacketManager)) {
                            ((ClientPacketManager)instance).PacketHandlers = Networking.NetworkManager.PacketManager.PacketHandlers;
                        }

                        if (!_plugins.ContainsKey(instance.GetPluginName())) {
                            foreach (var intf in type.GetInterfaces()) {
                                if (intf == typeof(IScripting)) {
                                    ((IScripting)instance).RunFile = Scripting.ScriptManager.RunFile;
                                }
                            }
                            _plugins.Add(instance.GetPluginName(), instance);
                        }
                    }
                }
            }

            foreach (var plugin in _plugins) {
                bool reqMet = true;
                foreach (var requirement in plugin.Value.GetPluginRequirements()) {
                    if (!_plugins.ContainsKey(requirement)) {
                        Program.Write("Requirements not met for " + plugin.Key + ": " + requirement);
                        reqMet = false;
                        break;
                    }
                }
                if (reqMet) {
                    Program.Write("Loaded " + plugin.Value.GetPluginName());
                    plugin.Value.Run();
                }
            }

            _plugins.Clear();
        }
    }
}
