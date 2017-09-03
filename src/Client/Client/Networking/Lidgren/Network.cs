using System;
using Lidgren.Network;
using System.Threading;

namespace Client.Networking.Lidgren
{
    public class Network : INetwork
    {
        private NetPeerConfiguration _config;
        private NetClient _client;
        private Thread _onReceive;

        public Network() {
            _config = new NetPeerConfiguration("EclipseGaming");
            _client = new NetClient(_config);

            // Start the client.
            _client.Start();
            _client.Connect("127.0.0.1", 7001);

            // Start the receiving data thread.
            _onReceive = new Thread(OnReceive);
            _onReceive.Start();


            // Wait for a response for seven seconds.
            int tick = Environment.TickCount + 7000;
            
            while (tick > Environment.TickCount) {
                if (IsConnected()) {
                    return;
                }
            }

            // No response. Assume the server is down, and exit.
            Environment.Exit(1);
        }

        public void Destroy() {
            _client.Disconnect("Disconnect");
        }

        public void SendData(byte[] array) {
            var msg = _client.CreateMessage();
            msg.Write(array);
            _client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        private void OnReceive() {
            NetIncomingMessage msg;

            while (true) {
                while ((msg = _client.ReadMessage()) != null) {
                    switch (msg.MessageType) {
                        case NetIncomingMessageType.ConnectionApproval:

                            break;
                        case NetIncomingMessageType.Data:
                            NetworkManager.PacketManager.HandlePacket(msg.Data);
                            break;
                        default:
                            Console.WriteLine("Unhandled " + msg.MessageType + " message: " + new IO.DataBuffer(msg.Data).ReadString());
                            break;
                    }
                }
            }
        }

        private bool IsConnected() {
            if (_client?.ServerConnection?.Status == NetConnectionStatus.Connected) {
                return true;
            }

            return false;
        }
    }
}
