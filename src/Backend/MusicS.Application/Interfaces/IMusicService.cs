using MusicS.Application.DTO;
using MusicS.Domain.Entities;

namespace MusicS.Application.Interfaces;

public interface IMusicService
{
    Task SaveSong(UploadSongDto dto);
    Task DeleteSong(string key);
    Task<MusicInfo> GetSong(string key);
}