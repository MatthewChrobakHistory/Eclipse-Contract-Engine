using Lidgren.Network;

namespace Server.Networking.Lidgren
{
    public class Client
    {
        public bool Connected { private set; get; }
        public NetConnection Connection;

        public Client(NetConnection connection) {
            this.Connection = connection;
            this.Connected = true;
        }

        public void Disconnect() {
            if (Connection != null) {
                Connection.Disconnect("Disconnect");
            }
            Connected = false;
        }

        public override string ToString() {
            return Connection.RemoteEndPoint.Address.ToString();
        }
    }
}
