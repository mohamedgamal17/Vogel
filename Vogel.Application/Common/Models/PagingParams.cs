namespace Vogel.Application.Common.Models
{
    public class PagingParams
    {
        public string? Cursor { get; set; }
        public bool Asending { get; set; }
        public int Limit { get; set; }
    }

    public class PagingAndSortingParams : PagingParams
    {
        public string SortBy { get; set; }
 
    }
}
