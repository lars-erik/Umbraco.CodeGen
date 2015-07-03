using System.Collections.Generic;

namespace Umbraco.CodeGen.Definitions
{
    public class DocumentTypeInfo : Info
    {
        public List<string> AllowedTemplates { get; set; }
        public string DefaultTemplate { get; set; }

        public DocumentTypeInfo()
        {
            AllowedTemplates = new List<string>();
        }

        protected bool Equals(DocumentTypeInfo other)
        {
            return AllowedTemplates.NullableSequenceEqual(AllowedTemplates) && string.Equals(DefaultTemplate, other.DefaultTemplate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DocumentTypeInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((AllowedTemplates != null ? AllowedTemplates.GetHashCode() : 0)*397) ^ (DefaultTemplate != null ? DefaultTemplate.GetHashCode() : 0);
            }
        }
    }
}