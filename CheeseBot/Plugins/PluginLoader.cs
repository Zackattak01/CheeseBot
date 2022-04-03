using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace CheeseBot.Plugins
{
    public static class PluginLoader
    {
        public const string DefaultSearchLocation = "Plugins";
        public static IReadOnlyList<Plugin> LoadPlugins(string location = null)
        {
            var files = DiscoverPluginsFiles(location ?? DefaultSearchLocation);
            
            if (files is null)
                return Array.Empty<Plugin>();
            
            var assemblies = LoadPluginAssemblies(files);
            return assemblies.Select(x => new Plugin(x)).ToList();
        }

        private static IEnumerable<string> DiscoverPluginsFiles(string location)
        {
            if (Directory.Exists(location))
                return Directory.EnumerateFiles(location, "*.dll", SearchOption.AllDirectories).Select(Path.GetFullPath);
            else
                return null;
        }

        private static IEnumerable<Assembly> LoadPluginAssemblies(IEnumerable<string> files)
        {
            var alc = new AssemblyLoadContext(null);
            var assemblies = new List<Assembly>();

            foreach (var pluginFile in files)
            {
                var asm = alc.LoadFromAssemblyPath(pluginFile);
                assemblies.Add(asm);
            }

            return assemblies;
        }
    }
}