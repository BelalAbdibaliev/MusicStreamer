using Microsoft.EntityFrameworkCore;
using MusicS.Domain.Entities;

namespace MusicS.Infrastructure.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
        
    }
    
    public DbSet<MusicInfo> Musics { get; set; }
}