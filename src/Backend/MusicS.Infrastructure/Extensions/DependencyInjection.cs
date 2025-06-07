using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicS.Infrastructure.Helpers;
using Microsoft.Extensions.Options;
using MusicS.Application.Interfaces;
using MusicS.Infrastructure.Services;

namespace MusicS.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AwsS3Settings>(configuration.GetSection("AWS"));
        
        services.AddScoped<IS3Service, S3Service>();

        return services;
    }
}