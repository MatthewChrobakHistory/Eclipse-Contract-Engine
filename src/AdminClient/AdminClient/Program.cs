using AdminClient.Data;
using AdminClient.Editors;
using AdminClient.IO;
using AdminClient.Networking;
using AdminClient.Plugins;
using AdminClient.Scripting;
using System;
using System.Windows.Forms;

namespace AdminClient
{
    public static class Program
    {
        // Global variables related to the client.
        public static readonly string StartupPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string DataPath = Program.StartupPath + "data\\";
        private static AdminWindow Window = new AdminWindow();

        // The main point of entry for the application.
        [STAThread]
        private static void Main(string[] args) {

            Application.EnableVisualStyles();

            // Check the folders and files in the system.
            FolderSystem.Check();

            // Load the client data.
            DataManager.Load();

            // Start the network.
            NetworkManager.Initialize();

            // Initialize the plugin manager.
            PluginManager.Initialize();

            // Initialize the scripting system.
            ScriptManager.Initialize();

            Application.Run(Window);
        }

        public static void ShowMessage(string message) {
            MessageBox.Show(message);
        }

        public static void Write(string message, bool newLine = true) {
            if (newLine) {
                Console.WriteLine(message);
            } else {
                Console.Write(message);
            }
        }
    }
}