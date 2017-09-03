using System.Collections.Generic;

namespace API.Networking
{
    public abstract class ClientPacketManager
    {
        public Dictionary<int, System.Action<byte[]>> PacketHandlers;

        protected void AddPacketHandler(System.Action<byte[]> packet) {
            this.PacketHandlers.Add(PacketHandlers.Count, packet);
        }
    }
}
