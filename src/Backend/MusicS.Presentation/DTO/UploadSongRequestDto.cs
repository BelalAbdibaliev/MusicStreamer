namespace MusicS.Presentation.DTO;

public class UploadSongRequestDto
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Album { get; set; }
    public IFormFile File { get; set; }
}