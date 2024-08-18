using System.Numerics;

namespace Vogel.BuildingBlocks.Shared.Dtos
{
    public abstract class EntityDto<T> : IEntityDto<T>
    {
        public T Id { get; set; }
    }
}
