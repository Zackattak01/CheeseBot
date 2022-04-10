using System.Reflection;

namespace CheeseBot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCheeseBotServices(this IServiceCollection services)
        {
            var baseType = typeof(CheeseBotService);
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsAssignableTo(baseType) && !x.IsAbstract);

            foreach (var type in types)
            {
                services.AddSingleton(type);
            }

            return services;
        }
    }
}