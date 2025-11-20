namespace Vogel.BuildingBlocks.Domain
{
    public abstract class Entity<TKey> : IEntity<TKey> 
    {
        public TKey Id { get ; set ; }
    }

    public abstract class EntityOwned<TKey> : Entity<TKey>, IOwnedEntity<TKey> 
    {
        public TKey UserId { get ; set ; }

        public bool IsOwnedBy(TKey userId)
        {
            return EqualityComparer<TKey>.Default.Equals(Id, userId);
        }
    }
}
