namespace API.Data
{
    public class Position
    {
        public int X;
        public int Y;

        public Position(int x = 0, int y = 0) {
            this.X = x;
            this.Y = y;
        }

        public void Move(int x, int y) {
            this.X = x;
            this.Y = y;
        }
    }
}
