using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicS.Application.DTO;
using MusicS.Application.Interfaces;
using MusicS.Infrastructure.Helpers;

namespace MusicS.Infrastructure.Services;

public class S3Service: IFileService
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<S3Service> _logger;
    private readonly AwsS3Settings _settings;
    
    public S3Service(IOptions<AwsS3Settings> settings, ILogger<S3Service> logger)
    {
        _settings = settings.Value;
        _s3Client = new AmazonS3Client(
            _settings.AccessKey,
            _settings.SecretKey,
            RegionEndpoint.GetBySystemName(_settings.Region));
    }
    
    public async Task<string> UploadFileAsync(string key, Stream inputStream)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key,
            InputStream = inputStream
        };

        await _s3Client.PutObjectAsync(putRequest);
        return $"https://{_settings.BucketName}.s3.amazonaws.com/{key}";
    }

    public async Task<Stream> GetFileAsync(string key)
    {
        var response = await _s3Client.GetObjectAsync(_settings.BucketName, key);
        return response.ResponseStream;
    }

    public async Task DeleteFileAsync(string key)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key
        };

        await _s3Client.DeleteObjectAsync(deleteRequest);
    }

    public async Task<FileMetadataDto> GetObjectMetadataAsync(string key)
    {
        var response = await _s3Client.GetObjectMetadataAsync(_settings.BucketName, key);
        
        return new FileMetadataDto
        {
            Key = key,
            Size = response.ContentLength,
            ContentType = response.Headers.ContentType,
            LastModified = response.LastModified,
            Metadata = response.Metadata.Keys.ToDictionary(k => k, k => response.Metadata[k])
        };

    }

    public async Task<Stream> GetFileRangeAsync(string key, long start, long end)
    {
        var request = new GetObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key,
            ByteRange = new ByteRange(start, end)
        };

        var response = await _s3Client.GetObjectAsync(request);
        return response.ResponseStream;
    }

}