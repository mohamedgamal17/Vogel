namespace Vogel.Domain
{
    public class User : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string MediaId { get; set; }
        protected override string GetEntityPerfix()
        {
            string entityPerfix = "usr";

            return entityPerfix;
        }
    }
}
