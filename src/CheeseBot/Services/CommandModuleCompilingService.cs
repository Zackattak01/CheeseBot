using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CheeseBot.Services
{
    public class CommandModuleCompilingService : CheeseBotService
    {
        private readonly CommandService _commandService;
        private readonly Dictionary<Snowflake, CommandModuleLoadContext> _loadedModules;
        
        public CommandModuleCompilingService(CommandService commandService, ILogger<CommandModuleCompilingService> logger) : base(logger)
        {
            _commandService = commandService;
            _loadedModules = new Dictionary<Snowflake, CommandModuleLoadContext>();
        }

        public bool IsModuleLoaded(Snowflake id)
            => _loadedModules.ContainsKey(id);

        public ICompileResult CreateModules(Snowflake id, string code)
        {
            Logger.LogInformation("Compiling module code...");
            var sw = Stopwatch.StartNew();
            var result = CompileUtils.CompileCommandModule(id.ToString(), code);
            sw.Stop();
            Logger.LogInformation("Finished compiling module code in {0}ms.", sw.ElapsedMilliseconds);

            if (result is SuccessfulCompileResult successfulResult)
            {
                var context = successfulResult.AssemblyLoadContext;
                if (context.Assemblies.Count() != 1)
                    throw new Exception("Assemblies loaded in load context is not equal to 1.  This should not happen.");

                var assembly = context.Assemblies.First();

                
                var modules = _commandService.AddModules(assembly);
                Logger.LogInformation("Added {0} module(s).", modules.Count);
                
                _loadedModules.Add(id, new CommandModuleLoadContext(context, modules));
            }

            return result;
        }

         
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool RemoveModules(Snowflake id)
        {
            if (!_loadedModules.TryGetValue(id, out var context))
                return false;
            
            foreach (var module in context.Modules)
                _commandService.RemoveModule(module);
            Logger.LogInformation("Removed {0} module(s).", context.Modules.Count());
            
            _loadedModules.Remove(id);
            
            context.AssemblyLoadContext.Unload();
            Logger.LogInformation($"Unloaded assembly load context.");
            return true;
        }

    }
}