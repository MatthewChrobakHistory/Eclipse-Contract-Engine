using System.IO;

namespace Server.Networking.Net
{
    public class OutgoingFile
    {
        public string ClientPath { private set; get; }
        public long Size { private set; get; }

        public OutgoingFile(string serverPath, string clientPath) {
            if (File.Exists(serverPath)) {
                this.ClientPath = clientPath;
                this.Size = new FileInfo(serverPath).Length;
            }
        }
    }
}
