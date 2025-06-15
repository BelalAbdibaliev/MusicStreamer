using Amazon;
using MusicS.Application.DTO;
using MusicS.Application.Interfaces;
using MusicS.Domain.Entities;

namespace MusicS.Application.Services;

public class MusicService: IMusicService
{
    private readonly IFileService _fileService;
    private readonly IMusicRepository _musicRepository;

    public MusicService(IFileService fileService, IMusicRepository musicRepository)
    {
        _fileService = fileService;
        _musicRepository = musicRepository;
    }


    public async Task SaveSong(UploadSongDto dto)
    {
        if(dto is null)
            throw new ArgumentNullException(nameof(dto));

        string key = $"{Guid.NewGuid() + dto.FileName}";
        
        var responseUrl = await _fileService.UploadFileAsync(key, dto.File);
        
        if(string.IsNullOrEmpty(responseUrl))
            throw new ArgumentNullException();

        var song = new MusicInfo
        {
            Title = dto.Title,
            Artist = dto.Artist,
            Album = dto.Album,
            CloudUrl = responseUrl,
            FileName = dto.FileName,
            Key = key,
        };

        await _musicRepository.Add(song);
    }

    public async Task DeleteSong(string key)
    {
        await _fileService.DeleteFileAsync(key);

        var song = await _musicRepository.GetInfo(key);
        
        if(song is null)
            throw new ArgumentNullException("Song not found");
        
        await _musicRepository.Delete(song);
    }

    public async Task<MusicInfo> GetSong(string key)
    {
        return await _musicRepository.GetInfo(key);
    }
}