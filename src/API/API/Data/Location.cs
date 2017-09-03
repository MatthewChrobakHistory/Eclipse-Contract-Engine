namespace API.Data
{
    public class Location : Position
    {
        public int Map;

        public Location(int map = 0, int x = 0, int y = 0) : base(x, y) {
            this.Map = map;
        }

        public void Move(int map, int x, int y) {
            base.Move(x, y);
            this.Map = map;
        }

        public static int ToInt(Position pos, int width) {
            return pos.Y * width + pos.X;
        }

        public static int ToInt(int x, int y, int width) {
            return y * width + x;
        }
    }
}
