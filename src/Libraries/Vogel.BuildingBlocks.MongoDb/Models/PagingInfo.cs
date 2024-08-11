namespace Vogel.BuildingBlocks.MongoDb.Models
{
    public class PagingInfo
    {
        public string? NextCursor { get; set; }
        public string? PreviousCursor { get; set; }
        public bool HasNext => NextCursor != null;
        public bool HasPrevious => PreviousCursor != null;
        public bool Ascending { get; set; }
        public PagingInfo(string? nextCursor, string? previousCursor, bool ascending)
        {
            NextCursor = nextCursor;
            PreviousCursor = previousCursor;
            Ascending = ascending;
        }
    }
}
