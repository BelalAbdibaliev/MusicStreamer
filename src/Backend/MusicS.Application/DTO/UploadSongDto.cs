namespace MusicS.Application.DTO;

public class UploadSongDto
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Album { get; set; }
    public Stream File { get; set; }
    public string FileName { get; set; }
}