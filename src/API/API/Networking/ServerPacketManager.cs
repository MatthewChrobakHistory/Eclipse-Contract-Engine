using System.Collections.Generic;

namespace API.Networking
{
    public abstract class ServerPacketManager
    {
        public Dictionary<int, System.Action<int, byte[]>> PacketHandlers;

        protected void AddPacketHandler(System.Action<int, byte[]> packet) {
            this.PacketHandlers.Add(PacketHandlers.Count, packet);
        }
    }
}