using Amazon.S3.Model;

namespace MusicS.Application.Interfaces;

public interface IS3Service
{
    Task UploadFileAsync(string key, Stream inputStream);
    Task<Stream> GetFileAsync(string key);
    Task DeleteFileAsync(string key);
    Task<Stream> GetFileRangeAsync(string key, long start, long end);
    Task<GetObjectMetadataResponse> GetObjectMetadataAsync(string key);
}