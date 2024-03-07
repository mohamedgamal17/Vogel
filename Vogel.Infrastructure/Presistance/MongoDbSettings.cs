namespace Vogel.Infrastructure.Presistance
{
    public class MongoDbSettings
    {
        public const string CONFIG_KEY = "MongoDb";
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
