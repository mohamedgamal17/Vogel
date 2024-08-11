using System.Numerics;

namespace Vogel.BuildingBlocks.Application.Dtos
{
    public abstract class EntityDto<T> : IEntityDto<T>
    {
        public T Id { get; set; }
    }
}
