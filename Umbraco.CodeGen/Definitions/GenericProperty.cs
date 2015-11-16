namespace Umbraco.CodeGen.Definitions
{
    public class GenericProperty : EntityDescription
    {
        /// <summary>
        /// Umbraco property editor type
        /// </summary>
        public string PropertyEditorAlias { get; set; }
        public string Definition { get; set; }
        public string Tab { get; set; }
        public bool Mandatory { get; set; }
        public string Validation { get; set; }

        protected bool Equals(GenericProperty other)
        {
            return base.Equals(other) && string.Equals(PropertyEditorAlias, other.PropertyEditorAlias) && string.Equals(Definition, other.Definition) && string.Equals(Tab, other.Tab) && Mandatory.Equals(other.Mandatory) && string.Equals(Validation, other.Validation);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GenericProperty) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (PropertyEditorAlias != null ? PropertyEditorAlias.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Definition != null ? Definition.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Tab != null ? Tab.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Mandatory.GetHashCode();
                hashCode = (hashCode*397) ^ (Validation != null ? Validation.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}