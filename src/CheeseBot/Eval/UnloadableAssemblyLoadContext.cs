using System.Reflection;
using System.Runtime.Loader;

namespace CheeseBot.Eval
{
    public class UnloadableAssemblyLoadContext : AssemblyLoadContext
    {
        public UnloadableAssemblyLoadContext() : base(isCollectible: true)
        {

        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}