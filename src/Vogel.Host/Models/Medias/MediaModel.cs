using MimeMapping;
using Vogel.Application.Medias.Commands;
using Vogel.Domain.Medias;

namespace Vogel.Host.Models.Medias
{
    public class MediaModel
    {
        public IFormFile File { get; set; }


        public async Task<CreateMediaCommand> ToCreateMediaCommand()
        {
            string extension = File.FileName.Split(".")[1];

            string fileName = string.Format("{0}_{1}.{2}", File.FileName.Split(".")[0], DateTime.Now.Ticks, extension);

            string mimeType = MimeUtility.GetMimeMapping(File.FileName);

            MediaType mediaType = GetMediaType(mimeType);

            MemoryStream stream = new MemoryStream();

            await File.CopyToAsync(stream);

            stream.Seek(0, SeekOrigin.Begin);

            return new CreateMediaCommand
            {
                Name = fileName,
                MimeType = mimeType,
                MediaType = mediaType,
                Content = stream
            };
        }

        public MediaType GetMediaType(string mimeType)
        {
            return mimeType.Split("/")[0].ToLower() switch
            {
                "video" => MediaType.Video,
                _ => MediaType.Image
            };
        }
    }
}
