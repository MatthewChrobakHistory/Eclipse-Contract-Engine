using AdminClient.Plugins;
using AdminClient.Scripting;
using System.IO;

namespace AdminClient.IO
{
    public static class FolderSystem
    {
        public static void Check() {
            // Create an array of directories to check.
            string[] folders = {
                // General program directories.
                Program.DataPath,

                // Directories needed for data.

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
