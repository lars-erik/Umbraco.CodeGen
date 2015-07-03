namespace Umbraco.CodeGen.Definitions
{
    public class Info : EntityDescription
    {
        public string Icon { get; set; }
        public string Thumbnail { get; set; }
        public bool AllowAtRoot { get; set; }
        public string Master { get; set; }

        protected bool Equals(Info other)
        {
            return base.Equals(other) && string.Equals(Icon, other.Icon) && string.Equals(Thumbnail, other.Thumbnail) && AllowAtRoot.Equals(other.AllowAtRoot) && string.Equals(Master, other.Master);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Info) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (Icon != null ? Icon.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Thumbnail != null ? Thumbnail.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ AllowAtRoot.GetHashCode();
                hashCode = (hashCode*397) ^ (Master != null ? Master.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}