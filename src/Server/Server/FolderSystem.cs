using System.IO;
using Server.Data.Models.Players;
using Server.Plugins;
using Server.Scripting;
using Server.Data.Models.Maps;

namespace Server
{
    public static class FolderSystem
    {
        public static void Check() {
            // Create an array of directories to check.
            string[] folders = {
                // General program directories.
                Program.DataPath,
                
                // Directories needed for data.
                Player.PlayerPath,
                Map.MapPath,

                // Directories needed for plugins.
                PluginManager.PluginPath,

                // Directories needed for scripts.
                ScriptManager.ScriptPath
            };

            // Loop through all the directories in the array, and see
            // if they exist. If not, create the directory.
            foreach (string folder in folders) {
                if (!Directory.Exists(folder)) {
                    Directory.CreateDirectory(folder);
                }
            }
        }
    }
}
