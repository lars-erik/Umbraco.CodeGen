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
    }
}