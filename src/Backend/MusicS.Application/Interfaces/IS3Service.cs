namespace MusicS.Application.Interfaces;

public interface IS3Service
{
    Task UploadFileAsync(string key, Stream inputStream);
    Task<Stream> GetFileAsync(string key);
    Task DeleteFileAsync(string key);
}