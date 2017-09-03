using System;
using API;
using API.Scripting;
using API.Networking;

namespace SamplePlugin
{
    public class SamplePlugin : ServerPacketManager, IPlugin, IScripting
    {
        public Action<string> RunFile { get; set; }

        public string GetPluginName() {
            return "Matt's Plugin";
        }

        public string[] GetPluginRequirements() {
            return new string[0];
        }

        public void Run() {
            AddPacketHandler((x, y) => {

            });
        }
    }
}
