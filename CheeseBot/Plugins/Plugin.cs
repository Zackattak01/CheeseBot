using System.Reflection;

namespace CheeseBot.Plugins
{
    public class Plugin
    {
        public IPluginManifest Manifest { get; private set; }
        public Assembly Assembly { get; }

        private readonly bool _isValidPluginDefinition;
        private readonly string _validationError;

        public Plugin(Assembly assembly)
        {
            Assembly = assembly;
            (_isValidPluginDefinition, _validationError) = Validate();
        }

        private (bool IsValidPluginDefinition, string ErrorString) Validate()
        {
            var manifests = Assembly.DefinedTypes.Where(x => x.IsAssignableTo(typeof(IPluginManifest))).ToList();

            if (manifests.Count == 1)
            {
                var ctor = manifests[0].GetConstructors().FirstOrDefault(x => !x.GetParameters().Any());

                if (ctor == null)
                    return (false, "Plugin manifest did not contain a parameterless constructor!");
                
                Manifest = ctor.Invoke(null) as IPluginManifest;
            }
            
            return manifests.Count switch
            {
                1 => (true, string.Empty),
                0 => (false, "Plugin manifest is missing!"),
                _ => (false, $"Plugin has {manifests.Count} manifests! Plugin assembly should only contain 1!")
            };
        }

        public (bool IsValidPluginDefinition, string ErrorString) GetValidationInformation() 
            => (_isValidPluginDefinition, _validationError);
    }
}