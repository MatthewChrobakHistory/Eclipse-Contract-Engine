namespace Server.Networking
{
    public static class NetworkManager
    {
        public static INetwork Network { private set; get; }
        public static PacketManager PacketManager { private set; get; }

        public static void Initialize() {
            NetworkManager.Network = new Net.Network();
            NetworkManager.PacketManager = new PacketManager();
        }

        public static void Destroy() {
            NetworkManager.Network.Destroy();
        }
    }
}
