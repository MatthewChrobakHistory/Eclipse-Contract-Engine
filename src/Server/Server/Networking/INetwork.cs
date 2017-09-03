using System.Collections.Generic;

namespace Server.Networking
{
    public interface INetwork
    {
        void Destroy();
        void SendDataTo(int index, byte[] array);
        void SendDataToAll(byte[] array);

        void SendFiles(int index, Dictionary<string, string> files);
        void HandleRequestFile(int index, string filePath);
        void HandleRequestFileChunk(int index, int chunkID);

        void HandleFileMeta(int index, byte[] array);
        void HandleFileChunk(int index, byte[] array);
        void HandleEndSendingFile(int index);
    }
}