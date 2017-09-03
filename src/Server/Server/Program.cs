using Server.Data;
using Server.Networking;
using Server.Plugins;
using Server.Scripting;
using System;

namespace Server
{
    public static class Program
    {
        // Global variables related to the server.
        public static readonly string StartupPath = System.AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string DataPath = Program.DataPath + "data\\";

        // The main point of entry for the application.
        private static void Main(string[] args) {
            // Check the folders and files in the system.
            FolderSystem.Check();

            // Load the game data.
            DataManager.Load();

            // Initialize the networking system.
            NetworkManager.Initialize();

            // Initialize the scripting system.
            ScriptManager.Initialize();

            // Initialize the plugin system.
            PluginManager.Initialize();

            // Set up the server destroy event handler.
            Program.Write("[IMPORTANT INFORMATION]");
            Program.Write("------------------------------------------------------------------------------");
            Program.Write("Remember to turn off the server by pressing [CTRL + C] or [CTRL + BREAK].");
            Program.Write("If you do not, all online player's data will NOT be saved.");
            Program.Write("------------------------------------------------------------------------------");
            System.Console.CancelKeyPress += (s, e) => {
                Program.Destroy();
            };

            // Start the gameloop.
            Program.GameLoop();
        }

        private static void GameLoop() {
            while (true) {
                System.Threading.Thread.Sleep(5000);
            }
        }

        private static void Destroy() {
            NetworkManager.Destroy();
            DataManager.Save();
            Environment.Exit(0);
        }

        public static void Write(string message, bool newline = true) {
            if (newline) {
                System.Console.WriteLine(message);
            } else {
                System.Console.Write(message);
            }
        }
    }
}
