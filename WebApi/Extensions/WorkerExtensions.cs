using Application.Service;
using Domain.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Extensions
{
    public static class WorkerExtensions
    {
        public static void AddWorker(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<WorkerSettings>(options => Configuration.GetSection(nameof(WorkerSettings)).Bind(options));
            services.AddHostedService<WorkerService>();
            services.AddHostedService<WorkerServiceWithInterval>();
        }
    }
}