using System;
using System.Collections.Generic;
using System.Threading;
using Lidgren.Network;


namespace Server.Networking.Lidgren
{
    public class Network : INetwork
    {
        private List<Client> _client = new List<Client>();
        private NetPeerConfiguration _config;
        private NetServer _server;
        private Thread _incomingData;

        public Network() {
            // Set up the config.
            _config = new NetPeerConfiguration("EclipseGaming");
            _config.LocalAddress = System.Net.IPAddress.Parse("127.0.0.1");
            _config.Port = 7001;
            _config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            // Set up the server.
            _server = new NetServer(_config);
            _server.Start();

            // Handle incoming messages.
            _incomingData = new Thread(OnReceive);
            _incomingData.Start();
        }

        private void OnReceive() {
            NetIncomingMessage msg;
            int tick = 0;
            
            while (true) {

                // Check for disconnections.
                if (tick < Environment.TickCount) {
                    foreach (var client in _client) {
                        if (!client.Connected) {
                            if (client.Connection?.Status == NetConnectionStatus.Connected) {
                                Program.Write("Client at " + client + " disconnected.");
                                client.Disconnect();
                            }
                        }
                    }

                    tick = Environment.TickCount + 1000;
                }

                while ((msg = _server.ReadMessage()) != null) {
                    // Approving a connection.
                    switch (msg.MessageType) {
                        case NetIncomingMessageType.ConnectionApproval:
                            msg.SenderConnection.Approve(_server.CreateMessage("Approve"));
                            AddConnection(msg.SenderConnection);
                            Program.Write("Connection accepted at " + msg.SenderEndPoint.Address.ToString());
                            break;
                        case NetIncomingMessageType.Data:
                            NetworkManager.PacketManager.HandlePacket(GetClientID(msg.SenderConnection), msg.Data);
                            break;
                        case NetIncomingMessageType.WarningMessage:
                            Program.Write(msg.MessageType.ToString() + ": " + msg.ToString());
                            break;
                        default:
                            if (msg.SenderEndPoint != null) {
                                Console.WriteLine("[" + msg.SenderEndPoint.Address.ToString() + "] " + msg.MessageType.ToString() + ": " + msg.ReadString());
                            } else {
                                Console.WriteLine("[ LOCAL ] " + msg.MessageType.ToString() + ": " + msg.ReadString());
                            }
                            break;
                    }
                }
            }
        }

        private void AddConnection(NetConnection connection) {
            // Look through our collection for a free slot.
            for (int i = 0; i < this._client.Count; i++) {
                if (!this._client[i].Connected) {
                    // If there is a free slot, create a new client at that position.
                    this._client[i] = new Client(connection);
                    return;
                }
            }

            // If an unused spot could not be found, add a new entry in the collection, and a
            // corresponding player.
            this._client.Add(new Client(connection));
            Data.DataManager.Players.Add(new Data.Models.Players.Player());
        }

        private int GetClientID(NetConnection connection) {
            for (int i = 0; i < _client.Count; i++) {
                if (_client[i]?.Connection?.RemoteEndPoint?.Address == connection.RemoteEndPoint.Address) {
                    return i;
                }
            }

            return -1;
        }

        public void Destroy() {
            _server.Shutdown("Shutdown");
        }

        public void SendDataTo(int index, byte[] array) {
            var msg = _server.CreateMessage();
            msg.Write(array);
            _server.SendMessage(msg, _client[index].Connection, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendDataToAll(byte[] array) {
            throw new NotImplementedException();
        }

        public void SendFile(int index, string filePath, string clientPath) {
            throw new NotImplementedException();
        }

        public void HandleRequestFileChunk(int index, int chunkID) {
            throw new NotImplementedException();
        }

        public void HandleFileMeta(int index, byte[] array) {
            throw new NotImplementedException();
        }

        public void HandleFileChunk(int index, byte[] array) {
            throw new NotImplementedException();
        }

        public void HandleEndSendingFile(int index) {
            throw new NotImplementedException();
        }

        public void SendFiles(int index, Dictionary<string, string> files) {
            throw new NotImplementedException();
        }

        public void HandleRequestFile(int index, string filePath) {
            throw new NotImplementedException();
        }
    }
}
