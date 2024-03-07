using MongoDB.Bson.Serialization.Attributes;

namespace Vogel.Application.Common.Dtos
{
    public abstract class EntityDto
    {
        public string Id { get; set; }
    }
}
