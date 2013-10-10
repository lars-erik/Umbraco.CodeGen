namespace Umbraco.CodeGen.Definitions
{
    public class DocumentType : ContentType
    {
        public DocumentTypeInfo Info { get; set; }

        public DocumentType()
        {
            Info = new DocumentTypeInfo();
        }
    }
}