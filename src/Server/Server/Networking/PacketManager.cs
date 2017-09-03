using Server.Data;
using Server.Data.Models.Players;
using Server.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Server.Networking
{
    public class PacketManager
    {
        public Dictionary<int, Action<int, byte[]>> PacketHandlers { get; private set; }

        public PacketManager() {
            PacketHandlers = new Dictionary<int, Action<int, byte[]>>();

            AddPacket(HandleRegisterPlayer);
            AddPacket(HandleLoginPlayer);
            AddPacket(HandleRequestLeaveGame);
            AddPacket(HandleRequestFileChunk);
            AddPacket(HandleFileMeta);
            AddPacket(HandleFileChunk);
            AddPacket(HandleEndSendingFile);
            AddPacket(HandleRequestFile);
        }


        private void AddPacket(Action<int, byte[]> packet) {
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

        public byte[] HandlePacket(int index, byte[] array) {

            // Push the bytes into a new databuffer object.
            var packet = new DataBuffer(array);

            bool process = true;

            // Continue to loop while there's still data to process.
            while (process) {
                // Get the size of the next packet.
                int size = packet.ReadInt();

                // Do we have more than all the data need for the packet?
                if (array.Length > size) {

                    // Resize the array containing the bytes needed for this packet, and
                    // create a new array containing the excess.
                    byte[] excessbuffer = new byte[array.Length - size];
                    Array.ConstrainedCopy(array, size, excessbuffer, 0, array.Length - size);
                    Array.Resize(ref array, size);

                    // Read the packet head, validate its contents, and invoke its data handler.
                    int head = packet.ReadInt();
                    if (PacketHandlers.ContainsKey(head)) {
                        PacketHandlers[head].Invoke(index, RemovePacketHead(array));
                    }

                    // Recreate the databuffer object with just the excess bytes, and 
                    // continue to loop.
                    packet = new DataBuffer(excessbuffer);

                    // Do we have all the data needed for the packet?
                } else if (array.Length == size) {

                    // Read the packet head, validate its contents, and invoke its data handler.
                    int head = packet.ReadInt();
                    if (head >= 0 && head < PacketHandlers.Count) {
                        PacketHandlers[head].Invoke(index, RemovePacketHead(array));
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

        #region Handle incoming packets
        private void HandleRegisterPlayer(int index, byte[] array) {
            var packet = new DataBuffer(array);
            var player = DataManager.Players[index];
            string username = packet.ReadString();
            string password = packet.ReadString();

            if (player.TryCreate(username, password)) {
                SendEnterGame(index);
            } else {
                SendMessage(index, "Player already exists!");
            }
        }
        private void HandleLoginPlayer(int index, byte[] array) {
            var packet = new DataBuffer(array);
            var player = DataManager.Players[index];
            string username = packet.ReadString();
            string password = packet.ReadString();
            string action = packet.ReadString();

            if (Player.TryLoad(ref player, username, password)) {
                // How do we handle their logging in?
                switch (action) {
                    case "admin client":
                        if (player.AccessLevel >= AccessLevel.Developer) {
                            SendEnterGame(index);
                        } else {
                            SendMessage(index, "Invalid access level: " + player.AccessLevel);
                        }
                        break;
                    case "game":
                        SendEnterGame(index);
                        break;
                    default:
                        SendMessage(index, "Invalid action: " + action);
                        break;
                }
                
            } else {
                SendMessage(index, "Player does not exist, or password is incorrect!");
            }
        }

        private void HandleRequestLeaveGame(int index, byte[] array) {
            SendLeaveGame(index);
        }
        private void HandleRequestFileChunk(int index, byte[] array) {
            var packet = new DataBuffer(array);
            int chunkID = packet.ReadInt();

            NetworkManager.Network.HandleRequestFileChunk(index, chunkID);
        }
        private void HandleFileMeta(int index, byte[] array) {
            NetworkManager.Network.HandleFileMeta(index, array);
        }
        private void HandleFileChunk(int index, byte[] array) {
            NetworkManager.Network.HandleFileChunk(index, array);
        }
        private void HandleEndSendingFile(int index, byte[] array) {
            NetworkManager.Network.HandleEndSendingFile(index);
        }
        
        private void HandleRequestFile(int index, byte[] array) { 
            NetworkManager.Network.HandleRequestFile(index, new DataBuffer(array).ReadString());
        }
        #endregion

        #region Handle outgoing packets
        public void SendMessage(int index, string message) {
            var packet = new DataBuffer();
            packet.Write(OutgoingPacket.SendMessage);
            packet.Write(message);
            NetworkManager.Network.SendDataTo(index, packet.ToArray());
        }

        public void SendEnterGame(int index) {

            var files = new Dictionary<string, string>();
            files.Add(Program.StartupPath + "sound.m4a", "sound.m4a");
            files.Add(Program.StartupPath + "sound2.m4a", "sound2.m4a");
            SendFiles(index, files);

            var packet = new DataBuffer();
            packet.Write(OutgoingPacket.SendEnterGame);
            NetworkManager.Network.SendDataTo(index, packet.ToArray());
        }

        public void SendLeaveGame(int index) {
            var packet = new DataBuffer();
            packet.Write(OutgoingPacket.SendLeaveGame);
            NetworkManager.Network.SendDataTo(index, packet.ToArray());
        }
        public void SendFiles(int index, Dictionary<string, string> files) {
            NetworkManager.Network.SendFiles(index, files);
        }
        #endregion
    }
}
