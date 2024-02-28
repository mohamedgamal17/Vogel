namespace Vogel.Domain
{
    public abstract class Entity
    {
        public string Id { get;  set; }

        public Entity()
        {
            Id = GenerateId();
        }
        protected virtual string GenerateId()
        {
            string perfix = GetEntityPerfix();

            string id = Ulid.NewUlid().ToString();

            string combinedId = string.Format("{0}_{1}", perfix, id);

            return combinedId;
        }

        protected abstract string GetEntityPerfix();

    }

}
