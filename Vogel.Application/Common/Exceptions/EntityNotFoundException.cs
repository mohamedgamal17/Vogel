namespace Vogel.Application.Common.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message) : base(message)
        {
            
        }

        public EntityNotFoundException (Type entity , string id) : base(
                $"there is no such {entity.Name} entity with id : {id}"
            )
        {
            
        }
    }
}
