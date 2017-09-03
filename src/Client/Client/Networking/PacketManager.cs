using Client.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Client.Networking
{
    public class PacketManager
    {
        public Dictionary<int, Action<byte[]>> PacketHandlers { get; private set; }

        // Outgoing file related fields.
        private string _outgoingFilePath;
        private byte[] _outgoingFileData;
        private int _outgoingChunkSize = 8192 - 12;

        // Incoming file related fields.
        private List<string> _incomingFiles;
        private Thread _fileMetaWatcher;

        private string _incomingFilePath;
        private byte[] _incomingFileData;
        private int _incomingChunkSize;
        private bool[] _incomingChunkReceived;
        private int _lastChunkReceived;
        private Thread _fileDataWatcher;
        

        public PacketManager() {
            // Create the array of data handlers.
            this.PacketHandlers = new Dictionary<int, Action< byte[]>>();

            // Add all packet handlers to the array here.
            AddPacket(HandleMessage);
            AddPacket(HandleEnterGame);
            AddPacket(HandleLeaveGame);
            AddPacket(HandleIncomingFilesMeta);
            AddPacket(HandleFileMeta);
            AddPacket(HandleFileChunk);
            AddPacket(HandleEndSendingFile);
            AddPacket(HandleRequestFileChunk);
        }

        private void AddPacket(Action<byte[]> packet) {
            this.PacketHandlers.Add(PacketHandlers.Keys.Count, packet);
        }

        private byte[] RemovePacketHead(byte[] array) {
            // If the size of the entire buffer is 8, all the packet contains is the head and size.
            // Packets like that are just initiation packets, and don't actually contin
            // other data. So, what we return won't be manipulated anyways. Return null.
            if (array.Length == 8) {
                return null;
            }

            // Create a new array of the size desired.
            byte[] clippedArray = new byte[array.Length - 8];

            // Copy the offset bytes into the clipped array.
            Array.Copy(array, 8, clippedArray, 0, array.Length - 8);

            // Return the clipped array.
            return clippedArray;
        }

        public byte[] HandlePacket(byte[] array) {

            // Push the bytes into a new databuffer object.
            var packet = new DataBuffer(array);

            bool process = true;

            // Continue to loop while there's still data to process.
            while (process) {

                // Get the size of the next packet.
                int size = packet.ReadInt();

                // Do we have more than all the data needed for the packet?
                if (array.Length > size) {

                    // Resize the array containing the bytes needed for this packet, and
                    // create a new array containing the excess.
                    byte[] excessbuffer = new byte[array.Length - size];
                    Array.ConstrainedCopy(array, size, excessbuffer, 0, array.Length - size);
                    Array.Resize(ref array, size);

                    // Read the packet head, validate its contents, and invoke its data handler.
                    int head = packet.ReadInt();
                    if (PacketHandlers.ContainsKey(head)) {
                        PacketHandlers[head].Invoke(RemovePacketHead(array));
                    }

                    // Recreate the databuffer object with just the excess bytes, and 
                    // continue to loop.
                    packet = new DataBuffer(excessbuffer);

                    // Do we have all the data needed for the packet?
                } else if (array.Length == size) {

                    // Read the packet head, validate its contents, and invoke its data handler.
                    int head = packet.ReadInt();
                    if (head >= 0 && head < PacketHandlers.Count) {
                        PacketHandlers[head].Invoke(RemovePacketHead(array));
                    }

                    // Return an empty array.
                    return new byte[0];
                } else {
                    if (size > 8192) {
                        Console.WriteLine("Absurd size expected: " + size);
                        return new byte[0];
                    } else {
                        Console.WriteLine("Not enough bytes: " + array.Length + "/" + size);
                    }

                    // We have less data than we need. There's nothing to process yet.
                    process = false;
                }
            }

            // Return the unprocessed bytes.
            return packet.ToArray();
        }


        #region Handling incoming packets
        private void HandleMessage(byte[] array) {
            var packet = new DataBuffer(array);
            Graphics.GraphicsManager.Graphics.ShowMessage(packet.ReadString());
        }
        
        private void HandleEnterGame(byte[] array) {
            if (Program.State == ClientState.MainMenu) {
                Program.SetGameState(ClientState.Game);
            }
        }
        private void HandleLeaveGame(byte[] array) {
            if (Program.State == ClientState.Game) {
                Program.SetGameState(ClientState.MainMenu);
            }
        }


        private void HandleIncomingFilesMeta(byte[] array) {
            var packet = new DataBuffer(array);
            _incomingFiles = new List<string>();

            int count = packet.ReadInt();
            for (int i = 0; i < count; i++) {
                _incomingFiles.Add(packet.ReadString());
            }
            this._incomingChunkSize = packet.ReadInt();

            SendRequestFile(_incomingFiles[0]);
            _fileMetaWatcher = new Thread(FileMetaWatcher);
            _fileMetaWatcher.Start();
        }

        private void HandleFileMeta(byte[] array) {
            var packet = new DataBuffer(array);
            
            this._incomingFilePath = Program.StartupPath + packet.ReadString();
            this._incomingFileData = new byte[packet.ReadLong()];
            this._incomingChunkReceived = new bool[this._incomingFileData.Length / this._incomingChunkSize];

            Console.WriteLine("Recieved incoming file: " + _incomingFilePath);

            this._lastChunkReceived = Environment.TickCount;
            if (_fileDataWatcher?.ThreadState != ThreadState.Running) {
                _fileDataWatcher = new Thread(FileDataWatcher);
                _fileDataWatcher.Start();
            }
        }
        private void HandleFileChunk(byte[] array) {
            var packet = new DataBuffer(array);
            int pos = packet.ReadInt();

            this._incomingChunkReceived[pos / _incomingChunkSize] = true;
            this._lastChunkReceived = Environment.TickCount;

            Array.ConstrainedCopy(array, 4, _incomingFileData, pos, array.Length - 4);
        }
        private void FileDataWatcher() {

            bool needFiles = true;

            while (needFiles) {
                if (this._lastChunkReceived + 1000 < Environment.TickCount) {

                    needFiles = false;

                    for (int i = 0; i < _incomingChunkReceived.Length; i++) {
                        if (!_incomingChunkReceived[i]) {
                            Console.WriteLine("Watcher: Requesting chunk " + i);
                            SendRequestFileChunk(i);
                            needFiles = true;
                            this._lastChunkReceived = Environment.TickCount;
                            break;
                        }
                    }
                }
            }
        }
        private void HandleEndSendingFile(byte[] array) {

            for (int i = 0; i < _incomingChunkReceived.Length; i++) {
                if (!_incomingChunkReceived[i]) {
                    Console.WriteLine("Requesting chunk " + i);
                    SendRequestFileChunk(i);
                    return;
                }
            }

            File.WriteAllBytes(_incomingFilePath, _incomingFileData);

            
            if (_incomingFiles.Count > 0) {
                _incomingFiles.RemoveAt(0);
                if (_incomingFiles.Count > 0) {
                    SendRequestFile(_incomingFiles[0]);

                    if (_fileDataWatcher?.ThreadState != ThreadState.Running) {
                        _fileDataWatcher = new Thread(FileDataWatcher);
                        _fileDataWatcher.Start();
                    }
                }
            }
        }
        private void HandleRequestFileChunk(byte[] array) {
            var packet = new DataBuffer(array);
            int chunkID = packet.ReadInt();

            SendFileChunk(chunkID);
        }
        #endregion

        #region Sending outgoing packets
        public void SendRegisterUser(string username, string password) {
            var packet = new DataBuffer();
            packet.Write(OutgoingPacket.SendRegisterUser);
            packet.Write(username);
            packet.Write(password);
            NetworkManager.Network.SendData(packet.ToArray());
        }
        public void SendLoginUser(string username, string password) {
            var packet = new DataBuffer();
            packet.Write(OutgoingPacket.SendLoginUser);
            packet.Write(username);
            packet.Write(password);
            packet.Write("game");
            NetworkManager.Network.SendData(packet.ToArray());
        }
        public void SendRequestLeaveGame() {
            if (Program.State == ClientState.Game) {
                var packet = new DataBuffer();
                packet.Write(OutgoingPacket.SendRequestLeaveGame);
                NetworkManager.Network.SendData(packet.ToArray());
            }
        }


        public void SendRequestFileChunk(int chunkID) {
            var packet = new DataBuffer();
            packet.Write(OutgoingPacket.SendRequestFileChunk);
            packet.Write(chunkID);
            NetworkManager.Network.SendData(packet.ToArray());
        }

        public void SendFileMeta(string localPath, string serverPath) {

            // Make sure that the file actually exists.
            if (!File.Exists(localPath)) {
                Program.Write("Tried to send a file that does not exist: " + localPath);
                return;
            }

            // Store the value on our end.
            this._outgoingFilePath = localPath;
            this._outgoingFileData = File.ReadAllBytes(localPath);

            // Send the client details about the file.
            var packet = new DataBuffer();
            packet.Write(OutgoingPacket.SendFileMeta);
            packet.Write(serverPath);
            packet.Write(new FileInfo(localPath).Length);
            packet.Write(_outgoingChunkSize);

            // Send the packet.
            NetworkManager.Network.SendData(packet.ToArray());

            for (int i = 0; i < _outgoingFileData.Length / _outgoingChunkSize; i++) {
                SendFileChunk(i, false);
            }
        }
        public void SendFileChunk(int chunkID, bool checkup = true) {
            var packet = new DataBuffer();
            packet.Write(OutgoingPacket.SendFileChunk);
            packet.Write(chunkID * _outgoingChunkSize);

            for (int i = chunkID * _outgoingChunkSize; i < (chunkID + 1) * _outgoingChunkSize; i++) {
                // Make sure we're within the bounds of the file data.
                if (i < _outgoingFileData.Length) {
                    packet.Write(_outgoingFileData[i]);
                } else {
                    break;
                }
            }

            NetworkManager.Network.SendData(packet.ToArray());

            // Check up on them.
            if (checkup) {
                SendEndFile();
            }
        }
        public void SendEndFile() {
            var packet = new DataBuffer();
            packet.Write(OutgoingPacket.SendEndSendingFile);
            NetworkManager.Network.SendData(packet.ToArray());
        }

        public void SendRequestFile(string filePath) {
            var packet = new DataBuffer();
            packet.Write(OutgoingPacket.SendRequestFile);
            packet.Write(filePath);
            NetworkManager.Network.SendData(packet.ToArray());
        }
        private void FileMetaWatcher() {

            int tmr = Environment.TickCount + 5000;

            while (false) {
                if (tmr < Environment.TickCount) {
                    Program.Write("Needed to remind the server to send the file meta.");
                    SendRequestFile(_incomingFiles[0]);
                    tmr = Environment.TickCount + 5000;
                }
            }
        }
        #endregion
    }
}
