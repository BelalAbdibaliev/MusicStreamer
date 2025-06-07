using Microsoft.AspNetCore.Mvc;
using MusicS.Application.Interfaces;

namespace MusicS.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController: ControllerBase
{
    private readonly IS3Service _s3Service;

    public FileController(IS3Service s3Service)
    {
        _s3Service = s3Service;
    }
    
    [HttpGet("stream/{fileName}")]
    public async Task<IActionResult> Stream(string fileName)
    {
        var rangeHeader = Request.Headers["Range"].ToString();

        var metadata = await _s3Service.GetObjectMetadataAsync(fileName);
        var totalSize = metadata.ContentLength;

        if (string.IsNullOrEmpty(rangeHeader))
        {
            var fullStream = await _s3Service.GetFileAsync(fileName);
            return File(fullStream, metadata.Headers.ContentType ?? "audio/mpeg", enableRangeProcessing: true);
        }

        var range = rangeHeader.Replace("bytes=", "").Split('-');
        long start = long.Parse(range[0]);
        long end = range.Length > 1 && !string.IsNullOrWhiteSpace(range[1]) 
            ? long.Parse(range[1]) 
            : totalSize - 1;
        var length = end - start + 1;

        var partialStream = await _s3Service.GetFileRangeAsync(fileName, start, end);

        Response.StatusCode = 206;
        Response.Headers["Content-Range"] = $"bytes {start}-{end}/{totalSize}";
        Response.Headers["Accept-Ranges"] = "bytes";

        return File(partialStream, metadata.Headers.ContentType ?? "audio/mpeg", enableRangeProcessing: false);
    }

    
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Empty file");

        using var stream = file.OpenReadStream();
        await _s3Service.UploadFileAsync(file.FileName, stream);

        return Ok("Uploaded");
    }

    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> Download(string fileName)
    {
        var stream = await _s3Service.GetFileAsync(fileName);
        return File(stream, "application/octet-stream", fileName);
    }

    [HttpDelete("{fileName}")]
    public async Task<IActionResult> Delete(string fileName)
    {
        await _s3Service.DeleteFileAsync(fileName);
        return Ok("Deleted");
    }
}