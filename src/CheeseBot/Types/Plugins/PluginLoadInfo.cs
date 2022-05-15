namespace CheeseBot.Plugins
{
    public class PluginLoadInfo
    {
        public IReadOnlyList<Plugin> Plugins { get; }
        public IReadOnlyList<Plugin> ValidPlugins { get; }
        public TimeSpan LoadingTime { get; }
        public string SearchLocation { get; }

        public PluginLoadInfo(IReadOnlyList<Plugin> plugins, IReadOnlyList<Plugin> validPlugins, TimeSpan loadingTime, string searchLocation)
        {
            Plugins = plugins;
            ValidPlugins = validPlugins;
            LoadingTime = loadingTime;
            SearchLocation = searchLocation ?? PluginLoader.DefaultSearchLocation;
        }
    }
}