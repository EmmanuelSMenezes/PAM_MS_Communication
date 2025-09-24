using Domain.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Extensions
{
    public static class MSCommunicationExtensions
    {
        public static void AddMSCommunication(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<MSCommunicationSettings>(options => Configuration.GetSection(nameof(MSCommunicationSettings)).Bind(options));
            services.Configure<LogSettings>(options => Configuration.GetSection(nameof(LogSettings)).Bind(options));
        }
    }
}