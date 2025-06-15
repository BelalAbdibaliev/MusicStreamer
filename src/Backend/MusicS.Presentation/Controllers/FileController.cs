using Microsoft.AspNetCore.Mvc;
using MusicS.Application.DTO;
using MusicS.Application.Interfaces;
using MusicS.Presentation.DTO;

namespace MusicS.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController: ControllerBase
{
    private readonly IFileService _fileService;
    private readonly IMusicService _musicService;

    public FileController(IFileService fileService, IMusicService musicService)
    {
        _fileService = fileService;
        _musicService = musicService;
    }
    
    [HttpGet("stream/{key}")]
    public async Task<IActionResult> Stream(string key)
    {
        var rangeHeader = Request.Headers["Range"].ToString();

        var metadata = await _fileService.GetObjectMetadataAsync(key);
        var totalSize = metadata.Size;

        if (string.IsNullOrEmpty(rangeHeader))
        {
            var fullStream = await _fileService.GetFileAsync(key);
            return File(fullStream, metadata.ContentType ?? "audio/mpeg", enableRangeProcessing: true);
        }

        var range = rangeHeader.Replace("bytes=", "").Split('-');
        long start = long.Parse(range[0]);
        long end = range.Length > 1 && !string.IsNullOrWhiteSpace(range[1]) 
            ? long.Parse(range[1]) 
            : totalSize - 1;
        var length = end - start + 1;

        var partialStream = await _fileService.GetFileRangeAsync(key, start, end);

        Response.StatusCode = 206;
        Response.Headers["Content-Range"] = $"bytes {start}-{end}/{totalSize}";
        Response.Headers["Accept-Ranges"] = "bytes";

        return File(partialStream, metadata.ContentType ?? "audio/mpeg", enableRangeProcessing: false);
    }

    
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(UploadSongRequestDto dto)
    {
        if (dto == null || dto.File.Length == 0)
            return BadRequest("Empty file");

        using var stream = dto.File.OpenReadStream();

        UploadSongDto songDto = new UploadSongDto
        {
            File = stream,
            Title = dto.Title,
            Artist = dto.Artist,
            Album = dto.Album,
            FileName = dto.File.FileName,
        };

        await _musicService.SaveSong(songDto);

        return Ok("Uploaded");
    }

    [HttpGet("download/{key}")]
    public async Task<IActionResult> Download(string key)
    {
        var stream = await _fileService.GetFileAsync(key);
        var song = await _musicService.GetSong(key);
        
        return File(stream, "application/octet-stream", song.FileName);
    }

    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete(string key)
    {
        await _musicService.DeleteSong(key);
        return Ok("Deleted");
    }
}