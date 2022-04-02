using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CheeseBot.Plugins;
using Disqord.Bot.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CheeseBot.Services
{
    public class PluginMasterService : IHostedService
    {
        private readonly IReadOnlyList<Plugin> _allPlugins;
        private ILogger<PluginMasterService> Logger { get; }
        public IReadOnlyList<Plugin> Plugins { get; }

        public PluginMasterService(ILogger<PluginMasterService> logger, IReadOnlyList<Plugin> plugins)
        {
            Logger = logger;
            Plugins = plugins.Where(x => x.GetValidationInformation().IsValidPluginDefinition).ToList();
            _allPlugins = plugins;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Attempting to load plugins from {Path.GetFullPath(PluginLoader.DefaultSearchLocation)}");
            var msg = Plugins.Count switch
            {
                0 when _allPlugins.Count == 0 => "No plugins were loaded.",
                0 when _allPlugins.Count > 0 => "No valid plugins were loaded.",
                _ => $"{Plugins.Count} plugin(s) were loaded: {string.Join(", ", Plugins.Select(x => x.Manifest.Name))}"
            };

            Logger.LogInformation(msg);
            
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
                    logMessage.AppendLine($"Assembly: {invalidPlugin.Assembly.FullName} Reason: {invalidPlugin.GetValidationInformation().ErrorString}");
                }
                
                if (foundInvalidPlugin)
                    Logger.LogError(logMessage.ToString().Trim());
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}