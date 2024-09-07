namespace Vogel.Content.Application.Tests
{
    public static class Resource
    {

        public static async Task<Stream> LoadImageAsStream()
        {
            using var streamReader = new StreamReader("resources/fake.png");

            var memoryStream = new MemoryStream();

            await streamReader.BaseStream.CopyToAsync(memoryStream);

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }
    }
}
