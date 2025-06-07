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