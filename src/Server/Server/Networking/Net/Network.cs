using Server.IO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server.Networking.Net
{
    public class Network : INetwork
    {
        // The socket to listen for new incoming connections.
        private Socket _server;

        // The list of clients connected to the server.
        private List<Client> _client;


        public Network() {
            // Initialize the list of clients.
            this._client = new List<Client>();

            // Create a TCP socket and bind it to a port.
            this._server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._server.Bind(new IPEndPoint(IPAddress.Any, 7001));

            // Allow up to five pending incoming connections.
            this._server.Listen(5);

            // Begin accepting connections.
            this._server.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void AcceptCallback(IAsyncResult ar) {
            // Add a new client with the accepted connection.
            this.AddConnection(this._server.EndAccept(ar));

            // Continue to accept connections.
            this._server.Listen(5);
            this._server.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        public void Destroy() {
            // Disconnect every client in the collection.
            foreach (var client in this._client) {
                client.Disconnect();
            }
            // Clear the list.
            this._client.Clear();
        }

        public void SendDataTo(int index, byte[] array) {

            // Make the data TCP ready.
            array = new DataBuffer(array).ToPaddedArray();

            // Make sure that the index provided is a valid client index.
            if (index >= 0 && index < this._client.Count) {
                this._client[index].SendData(array);
            } else {
                // Otherwise, display an error message.
                Program.Write("NetworkError: Tried to send data to nonexistant client: " + index);
            }
        }

        public void SendDataToAll(byte[] array) {

            // Make the data TCP ready.
            array = new DataBuffer(array).ToPaddedArray();

            // Send the given data to every client in the collection.
            foreach (var client in this._client) {
                client.SendData(array);
            }
        }

        public void AddConnection(Socket connection) {
            // Look through our collection for a free slot.
            for (int i = 0; i < this._client.Count; i++) {
                if (!this._client[i].Connected) {
                    // If there is a free slot, create a new client at that position.
                    this._client[i] = new Client(connection, i);
                    return;
                }
            }

            // If an unused spot could not be found, add a new entry in the collection, and a
            // corresponding player.
            this._client.Add(new Client(connection, this._client.Count));
            Data.DataManager.Players.Add(new Data.Models.Players.Player());
        }

        public void SendFiles(int index, Dictionary<string, string> files) {
            _client[index].SendIncomingFilesMeta(files);
        }

        public void HandleRequestFile(int index, string filePath) {
            _client[index].HandleRequestFile(filePath);
        }

        public void HandleRequestFileChunk(int index, int chunkID) {
            _client[index].SendFileChunk(chunkID);
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
    }
}
