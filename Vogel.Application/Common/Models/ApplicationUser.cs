namespace Vogel.Application.Common.Models
{
    public class ApplicationUser
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public List<string> Roles { get; set; }
    }
}
