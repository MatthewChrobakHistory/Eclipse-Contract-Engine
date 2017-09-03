using API.Data.Players;
using Server.IO;
using System.IO;

namespace Server.Data.Models.Players
{
    public class Player : PlayerPlugin
    {
        public static readonly string PlayerPath = Program.DataPath + "players\\";

        public string Username;
        public string DisplayName;
        public string Password;

        public AccessLevel AccessLevel;

        public Player() {
            
        }

        public bool TryCreate(string username, string password) {
            string file = PlayerPath + username + ".xml";

            // Make sure the file doesn't already exist.
            if (File.Exists(file)) {
                return false;
            }

            // Go ahead and make the player.
            this.Username = username;
            this.DisplayName = username;
            this.Password = password;

            // Save the player before returning true.
            Serialization.Serialize<Player>(file, this.GetType(), this);
            return true;
        }

        public static bool TryLoad(ref Player player, string username, string password) {
            string file = PlayerPath + username + ".xml";

            if (File.Exists(file)) {
                var tempPlayer = Serialization.Deserialize<Player>(file, player.GetType());
                if (tempPlayer.Username == username && tempPlayer.Password == password) {
                    player = tempPlayer;
                    return true;
                }
            }

            return false;
        }
    }
}
