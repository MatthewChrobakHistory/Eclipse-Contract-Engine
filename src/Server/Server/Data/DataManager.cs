using Server.Data.Models.Maps;
using Server.Data.Models.Players;
using Server.IO;
using System.Collections.Generic;

namespace Server.Data
{
    public static class DataManager
    {
        public static List<Player> Players = new List<Player>();
        public static List<Map> Maps = new List<Map>();

        public static void Load() {

            //var map = new Map();
            //map.Name = "TestMap";
            //map.Layers = new Layer[1];
            //map.Attributes = new byte[Map.MapWidth * Map.MapHeight];
            
            //for (int i = 0; i < map.Attributes.Length; i++) {
            //    map.Attributes[i] = 0;
            //}

            //map.Layers[0] = new Layer();

            //map.Layers[0].Tile = new Tile[Map.MapWidth * Map.MapHeight];

            //for (int x = 0; x < Map.MapWidth; x++) {
            //    for (int y = 0; y < Map.MapHeight; y++) {
            //        map.Layers[0].Tile[API.Data.Location.ToInt(x, y, Map.MapWidth)] = new Tile();
            //        map.Layers[0].Tile[API.Data.Location.ToInt(x, y, Map.MapWidth)].Surface = "red";
            //        map.Layers[0].Tile[API.Data.Location.ToInt(x, y, Map.MapWidth)].X = 0;
            //        map.Layers[0].Tile[API.Data.Location.ToInt(x, y, Map.MapWidth)].Y = 0;
            //    }
            //}

            //Maps.Add(map);

            Map.LoadMaps();
        }

        public static void Save() {

        }
    }
}
