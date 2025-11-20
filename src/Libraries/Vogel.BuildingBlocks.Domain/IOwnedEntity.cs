namespace Vogel.BuildingBlocks.Domain
{
    public interface IOwnedEntity <T>
    {
        public T UserId { get; set; }

        bool IsOwnedBy(T userId);
    }

    public interface IOwnedEntity  : IOwnedEntity<string>
    {
       
    }
}
