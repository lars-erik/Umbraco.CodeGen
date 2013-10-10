namespace Umbraco.CodeGen.Definitions
{
    public class MediaType : ContentType
    {
        public Info Info { get; set; }

        public MediaType()
        {
            Info = new Info();
        }
    }
}