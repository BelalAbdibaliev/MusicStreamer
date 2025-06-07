namespace MusicS.Infrastructure.Helpers;

public class AwsS3Settings
{
    public string BucketName { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string Region { get; set; }
}