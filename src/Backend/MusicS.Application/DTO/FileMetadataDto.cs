namespace MusicS.Application.DTO;

public class FileMetadataDto
{
    public string Key { get; set; }
    public long Size { get; set; }
    public string ContentType { get; set; }
    public DateTime? LastModified { get; set; }
    public IDictionary<string, string> Metadata { get; set; }

}