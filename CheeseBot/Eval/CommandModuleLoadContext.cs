using System.Collections.Generic;
using System.Runtime.Loader;
using Qmmands;

namespace CheeseBot.Eval
{
    public class CommandModuleLoadContext
    {
        public AssemblyLoadContext AssemblyLoadContext { get; }
        public IEnumerable<Module> Modules { get; }

        public CommandModuleLoadContext(AssemblyLoadContext assemblyLoadContext, IEnumerable<Module> modules)
        {
            AssemblyLoadContext = assemblyLoadContext;
            Modules = modules;
        }
    }
}