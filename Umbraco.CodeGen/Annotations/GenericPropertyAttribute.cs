namespace Umbraco.CodeGen.Annotations
{
    public class GenericPropertyAttribute : EntityDescriptionAttribute
    {
        public string Definition { get; set; }
        public string Tab { get; set; }
        public bool Mandatory { get; set; }
        public string Validation { get; set; }
    }
}
