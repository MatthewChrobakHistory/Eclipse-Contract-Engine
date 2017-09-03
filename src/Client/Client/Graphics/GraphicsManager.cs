namespace Client.Graphics
{
    public static class GraphicsManager
    {
        // Directories
        public static readonly string SurfacePath = Program.StartupPath + "data\\surfaces\\";
        public static readonly string GuiPath = GraphicsManager.SurfacePath + "gui\\";
        public static readonly string FontPath = Program.DataPath + "fonts\\";

        // The class object containing the graphics system.
        public static IGraphics Graphics { private set; get; }
        public static int WindowWidth = 960;
        public static int WindowHeight = 640;
        public const uint DrawWidth = 960;
        public const uint DrawHeight = 640;
        public static float WidthRatio = 1f;
        public static float HeightRatio = 1f;

        public static void Initialize() {
            // Set and initialize the graphics system.
            GraphicsManager.Graphics = new Sfml.Sfml();
        }
    }
}
