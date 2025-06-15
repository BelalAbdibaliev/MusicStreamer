namespace MusicS.Domain.Entities;

public class MusicInfo
{
    public int Id { get; set; }
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Album { get; set; }
    public string CloudUrl { get; set; }
    public string FileName { get; set; }
    public string Key { get; set; }
}