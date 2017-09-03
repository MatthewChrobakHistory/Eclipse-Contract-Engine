namespace Client.Graphics
{
    public interface IGraphics : ISystem
    {
        void DrawObject(object surface);
        void ShowMessage(string message);
        object GetFont();
    }
}
