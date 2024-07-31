namespace Vogel.BuildingBlocks.MongoDb
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MongoCollectionAttribute : Attribute
    {
        public string Name { get; set; }

        public MongoCollectionAttribute(string name)
        {
            Name = name;
        }
    }
}
