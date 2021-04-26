using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CheeseBot.Eval;
using Disqord;
using Microsoft.Extensions.Logging;
using Qmmands;

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
            var result = CompileUtils.CompileCommandModule(id.ToString(), code);
            Logger.LogInformation("Done compiling module code.");

            if (result is SuccessfulCompileResult successfulResult)
            {
                var context = successfulResult.AssemblyLoadContext;
                if (!context.Assemblies.Any() || context.Assemblies.Count() > 1)
                    throw new Exception("Assemblies loaded in load context is not equal to 1.  This should not happen.");

                var assembly = context.Assemblies.First();

                
                var modules = _commandService.AddModules(assembly);
                Logger.LogInformation($"Added {modules.Count} module(s).");
                
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
            Logger.LogInformation($"Removed {context.Modules.Count()} module(s).");
            
            _loadedModules.Remove(id);

            // TODO: Im fairly certain this isn't being unloaded properly
            // Memory doesnt seem to be released even after the assemblyloadcontext obj is collected
            context.AssemblyLoadContext.Unload();
            Logger.LogInformation($"Unloaded assembly load context.");
            return true;
        }

    }
}