namespace Vogel.Infrastructure
{
    public class S3ObjectStorageConfiguration
    {
        public const string CONFIG_KEY = "S3Storage";
        public string EndPoint { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }

        public bool WithSSL { get; set; }
    }
}
