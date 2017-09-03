namespace API
{
    public interface IPlugin
    {
        string GetPluginName();
        string[] GetPluginRequirements();
        void Run();
    }
}
