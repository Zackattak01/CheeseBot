using CheeseBot.Plugins;
using Disqord.Hosting;

namespace CheeseBot.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder InstallPlugins(this IHostBuilder builder)
        {
            return builder
                .ConfigureServices((context, services) =>
                {
                    var plugins = PluginLoader.LoadPlugins(context.Configuration["plugins_location"]);
                    var validPlugins = plugins.Where(x => x.GetValidationInformation().IsValidPluginDefinition);
                    var validPluginAssemblies = validPlugins.Select(x => x.Assembly).ToList();
                    
                    // Stolen from 
                    // https://github.com/Quahu/Disqord/blob/3a3ef050aaf1e7c80f13abf41f4190d0291f1d7a/src/Disqord/Hosting/DiscordClientHostBuilderExtensions.cs#L45-L76
                    // and
                    // https://github.com/Quahu/Disqord/blob/3a3ef050aaf1e7c80f13abf41f4190d0291f1d7a/src/Disqord.Core/DependencyInjection/ServiceCollectionExtensions.cs#L10-L11
                    static Type GetImplementationType(ServiceDescriptor descriptor)
                        => descriptor.ImplementationType ?? (descriptor.ImplementationInstance?.GetType() ?? descriptor.ImplementationFactory?.GetType().GenericTypeArguments[1]);
                    
                    for (var i = 0; i < validPluginAssemblies.Count; i++)
                    {
                        var types = validPluginAssemblies[i].GetExportedTypes();
                        foreach (var type in types)
                        {
                            if (type.IsAbstract)
                                continue;

                            if (!typeof(DiscordClientService).IsAssignableFrom(type))
                                continue;

                            var hasService = false;
                            for (var j = 0; j < services.Count; j++)
                            {
                                var service = services[j];
                                if (service.ServiceType != type && (service.ServiceType != typeof(IHostedService) || GetImplementationType(service) != type))
                                    continue;

                                hasService = true;
                                break;
                            }

                            if (hasService)
                                continue;

                            services.AddDiscordClientService(type);
                        }
                    }
                    
                    services.AddHostedService(s => new PluginMasterService(s.GetRequiredService<ILogger<PluginMasterService>>(), plugins, s.GetRequiredService<IConfiguration>()));
                });
        }
    }
}