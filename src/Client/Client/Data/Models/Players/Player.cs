using API.Data.Players;

namespace Client.Data.Models.Players
{
    public class Player : PlayerPlugin
    {
        public string Username { private set; get; }
        public string DisplayName { private set; get; }
    }
}
