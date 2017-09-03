using API;
using API.DataModels.Players;

namespace SamplePlugin
{
    public class PlayerExtension : IPlugin
    {
        public string GetPluginName() {
            return "Player Editor";
        }

        public string[] GetPluginRequirements() {
            return new string[0];
        }

        public void Run() {
            PlayerPlugin.AddField("UserID", false);
        }
    }
}
