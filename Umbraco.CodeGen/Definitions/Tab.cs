namespace Umbraco.CodeGen.Definitions
{
    public class Tab
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public int? Order { get; set; }

        protected bool Equals(Tab other)
        {
            return Id == other.Id && string.Equals(Caption, other.Caption) && Order == other.Order;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tab) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode*397) ^ (Caption != null ? Caption.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Order.GetHashCode();
                return hashCode;
            }
        }
    }
}