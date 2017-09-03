namespace API.Data
{
    public class Entity : Location
    {
        public Direction Direction;

        public Entity() : base() {
            this.Direction = Direction.Down;
        }

        public Entity(int map, int x, int y) : base(map, x, y) {
            this.Direction = Direction.Down;
        }

        public virtual void Teleport(int x, int y) {
            base.Move(x, y);
        }

        public virtual void Teleport(Location position) {
            base.Move(position.Map, position.X, position.Y);
        }
    }
}
