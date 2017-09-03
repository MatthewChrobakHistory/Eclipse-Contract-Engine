using API;
using API.Networking;
using System;

namespace SamplePlugin
{
    public class PacketSystem : ServerPacketManager, IPlugin
    {
        public string GetPluginName() {
            return "Packet Handler";
        }

        public string[] GetPluginRequirements() {
            return new string[0];
        }

        public void Run() {
            AddPacketHandler((index, data) => {
                Console.WriteLine("A packet for player " + index + " was received with a size of " + data.Length);
            });
        }
    }
}
