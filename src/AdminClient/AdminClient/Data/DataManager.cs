using System.Collections.Generic;
using AdminClient.Data.Models.Players;

namespace AdminClient.Data
{
    public static class DataManager
    {
        public static List<Player> Players { private set; get; }

        public static void Load() {
            // Inlcude all data-loading logic here
        }

        public static void Save() {
            // Include all data-saving logic here.
        }
    }
}
