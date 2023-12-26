using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Module;

public interface IModuleDefinition
{
    void AddModuleServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment);

    void MapEndpoints(IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment environment);
}
