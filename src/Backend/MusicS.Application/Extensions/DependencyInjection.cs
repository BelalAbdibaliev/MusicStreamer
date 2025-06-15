using Microsoft.Extensions.DependencyInjection;
using MusicS.Application.Interfaces;
using MusicS.Application.Services;

namespace MusicS.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMusicService, MusicService>();
        
        return services;
    }
}