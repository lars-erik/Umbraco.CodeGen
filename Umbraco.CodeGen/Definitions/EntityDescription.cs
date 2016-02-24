namespace Umbraco.CodeGen.Definitions
{
    public abstract class EntityDescription : IEntityDescription
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }

        protected bool Equals(EntityDescription other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Alias, other.Alias) && string.Equals(Description, other.Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EntityDescription) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Alias != null ? Alias.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Description != null ? Description.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}