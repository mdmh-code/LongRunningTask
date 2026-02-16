using LongRunningTask.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LongRunningTask.Domain.Configuration
{
    public static class RegisterExtensions
    {
        public static void RegisterDomain(this IServiceCollection services)
        {
            services.AddSingleton<IDelayProvider, ThreadDelayProvider>();
            services.AddScoped<StringProcessor>();
            services.AddScoped<CharacterEmmiter>();
        }
    }
}
