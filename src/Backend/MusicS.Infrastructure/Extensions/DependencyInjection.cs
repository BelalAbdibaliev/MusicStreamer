using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicS.Infrastructure.Helpers;
using Microsoft.Extensions.Options;
using MusicS.Application.Interfaces;
using MusicS.Infrastructure.Data;
using MusicS.Infrastructure.Data.Repositories;
using MusicS.Infrastructure.Services;

namespace MusicS.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AwsS3Settings>(configuration.GetSection("AWS"));
        
        services.AddScoped<IFileService, S3Service>();
        services.AddScoped<IMusicRepository, MusicRepository>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        return services;
    }
}