using System.Collections.Generic;

namespace Umbraco.CodeGen.Definitions
{
    public class DocumentType : ContentType
    {
        public DocumentType()
        {
            Info = new DocumentTypeInfo();
        }
    }
}