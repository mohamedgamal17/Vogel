namespace Vogel.Application.Common.Models
{
    public class S3ObjectStorageSaveModel
    {
       
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public Stream Content { get; set; }
        public S3ObjectStorageSaveModel(string fileName, string contentType, Stream content)
        {
            FileName = fileName;
            ContentType = contentType;
            Content = content;
        }

        public S3ObjectStorageSaveModel()
        {
            
        }
    }
}
