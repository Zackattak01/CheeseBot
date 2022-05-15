namespace CheeseBot.Plugins
{
    public abstract class PluginManifest
    {
        public abstract string Name { get; }

        public virtual void Awake()
        { }

        public virtual void ConfigureHost(IHostBuilder builder)
        { }

        public virtual void ConfigureServices(IServiceCollection services)
        { }
    }
}