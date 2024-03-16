using Vogel.Application.Common.Models;

namespace Vogel.Host.Models
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }

        public PagingInfo? PagingInfo { get; set; }
    }
}
