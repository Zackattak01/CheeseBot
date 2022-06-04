using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using CheeseBot.Plugins;

namespace CheeseBot.Services
{
    public class PluginMasterService : IHostedService
    {
        private readonly IReadOnlyList<Plugin> _allPlugins;
        private readonly PluginLoadInfo _loadInfo;
        private readonly CommandService _commandService;
        
        private ILogger<PluginMasterService> Logger { get; }
        public IReadOnlyList<Plugin> Plugins { get; }

        public PluginMasterService(ILogger<PluginMasterService> logger, PluginLoadInfo loadInfo, CommandService commandService)
        {
            Logger = logger;
            _loadInfo = loadInfo;
            _commandService = commandService;

            Plugins = loadInfo.ValidPlugins;
            _allPlugins = loadInfo.Plugins;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var commandsInfo = AddPluginCommands();
            LogPluginLoadInformation(commandsInfo);
            
            return Task.CompletedTask;
        }

        private IReadOnlyList<(Plugin Plugin, int ModuleCount, int CommandCount)> AddPluginCommands()
        { 
            void MutateModule(ModuleBuilder moduleBuilder)
            {
                var methods = moduleBuilder.Type.GetMethods(BindingFlags.Static | BindingFlags.Public);
                var method = methods.FirstOrDefault(x => x.GetCustomAttribute<MutateModuleAttribute>() != null);
                if (method == null)
                    return;

                method.Invoke(null, new object[] { moduleBuilder });
            }

            var addedCommands = new List<(Plugin, int, int)>();

            foreach (var plugin in Plugins)
            {
                var addedModules = _commandService.AddModules(plugin.Assembly, action: MutateModule);
                addedCommands.Add((plugin, addedModules.Count, addedModules.SelectMany(CommandUtilities.EnumerateAllCommands).Count()));
            }

            return addedCommands;
        }
        
        private void LogPluginLoadInformation(IReadOnlyList<(Plugin Plugin, int ModuleCount, int CommandCount)> commandInfo)
        {
            Logger.LogInformation("Attempted to load plugins from {0}", Path.GetFullPath(_loadInfo.SearchLocation));
            var msg = Plugins.Count switch
            {
                0 when _allPlugins.Count == 0 => "No plugins were loaded.",
                0 when _allPlugins.Count > 0 => "No valid plugins were loaded.",
                _ => "{0} plugin(s) were loaded in {1}ms: {2}"
            };

            IEnumerable<string> FormatPluginList(IEnumerable<(Plugin Plugin, int ModuleCount, int CommandCount)> commandInfo)
            {
                foreach (var info in commandInfo)
                {
                    if (info.ModuleCount > 0)
                        yield return $"{info.Plugin.Manifest.Name} ({info.ModuleCount} modules(s) with {info.CommandCount} commands)";
                    else
                        yield return info.Plugin.Manifest.Name;
                }
            }
            
            var pluginString = string.Join(", ", FormatPluginList(commandInfo));
            Logger.LogInformation(msg, Plugins.Count, (int)_loadInfo.LoadingTime.TotalMilliseconds, pluginString);
            
            if (_allPlugins.Count != Plugins.Count)
            {
                var logMessage = new StringBuilder("The following plugins could not be loaded:\n");
                var invalidPlugins = _allPlugins.Except(Plugins);

                var foundInvalidPlugin = false;
                foreach (var invalidPlugin in invalidPlugins)
                {
                    // Detects if the "invalid plugin" is actually just a dependency of another plugin
                    if (Plugins.SelectMany(x => x.Assembly.GetReferencedAssemblies().Select(x => x.Name)).Contains(invalidPlugin.Assembly.GetName().Name))
                        continue;

                    foundInvalidPlugin = true;
                    logMessage.AppendLine($"Assembly: {invalidPlugin.Assembly.FullName} Reason: {invalidPlugin.ValidationInformation.ErrorString}");
                }
                
                if (foundInvalidPlugin)
                    Logger.LogError(logMessage.ToString().Trim());
            }
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}