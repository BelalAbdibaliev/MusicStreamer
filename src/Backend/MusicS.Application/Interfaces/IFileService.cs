using Amazon.S3.Model;
using MusicS.Application.DTO;

namespace MusicS.Application.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(string key, Stream inputStream);
    Task<Stream> GetFileAsync(string key);
    Task DeleteFileAsync(string key);
    Task<Stream> GetFileRangeAsync(string key, long start, long end);
    Task<FileMetadataDto> GetObjectMetadataAsync(string key);
}