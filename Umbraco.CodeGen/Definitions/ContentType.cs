using System.Collections.Generic;

namespace Umbraco.CodeGen.Definitions
{
    public class ContentType : Entity, IEntityDescription
    {
        public string Name { get { return Info.Name; } }
        public string Alias { get { return Info.Alias; } }
        public string Description { get { return Info.Description; } }
        
        public Info Info { get; set; }
        public List<GenericProperty> GenericProperties { get; set; }
        public List<Tab> Tabs { get; set; }
        public List<string> Structure { get; set; }
        public List<ContentType> Composition { get; set; }
        public bool IsMixin { get; set; }

        public ContentType()
        {
            GenericProperties = new List<GenericProperty>();
            Tabs = new List<Tab>();
            Structure = new List<string>();
            Info = new Info();
            Composition = new List<ContentType>();
        }

        protected bool Equals(ContentType other)
        {
            return Equals(Info, other.Info) && 
                   GenericProperties.NullableSequenceEqual(other.GenericProperties) && 
                   Tabs.NullableSequenceEqual(other.Tabs) && 
                   Structure.NullableSequenceEqual(other.Structure);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ContentType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Info != null ? Info.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (GenericProperties != null ? GenericProperties.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Tabs != null ? Tabs.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Structure != null ? Structure.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}