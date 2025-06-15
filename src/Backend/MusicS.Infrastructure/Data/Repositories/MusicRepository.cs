using Microsoft.EntityFrameworkCore;
using MusicS.Application.Interfaces;
using MusicS.Domain.Entities;

namespace MusicS.Infrastructure.Data.Repositories;

public class MusicRepository: IMusicRepository
{
    private readonly ApplicationDbContext _context;

    public MusicRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<MusicInfo> GetInfo(string key)
    {
        return await _context.Musics.FirstOrDefaultAsync(x => x.Key == key);
    }

    public async Task<IEnumerable<MusicInfo>> GetAll(int page, int pageSize)
    {
        var songs = await _context.Musics.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return songs;
    }

    public async Task<IEnumerable<MusicInfo>> Search(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return Enumerable.Empty<MusicInfo>();

        pattern = $"%{pattern.Trim()}%";

        return await _context.Musics
            .Where(m =>
                EF.Functions.Like(m.Title, pattern) ||
                EF.Functions.Like(m.Artist, pattern) ||
                EF.Functions.Like(m.Album, pattern)
            )
            .ToListAsync();
    }

    public async Task Add(MusicInfo music)
    {
        await _context.Musics.AddAsync(music);
        await _context.SaveChangesAsync();
    }

    public async Task Update(MusicInfo music)
    {
        _context.Update(music);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(MusicInfo music)
    {
        _context.Musics.Remove(music);
        await _context.SaveChangesAsync();
    }
}