using LongRunningTask.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;



namespace LongRunningTask.Infraestructure.Configuration
{
    public static class RegisterExtensions
    {
        public static void RegisterInfraestructure(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddScoped<ICharacterReceiver, SignalCharacterReceiver>();
        }
    }
}
