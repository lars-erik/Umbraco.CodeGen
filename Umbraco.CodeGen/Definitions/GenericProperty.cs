namespace Umbraco.CodeGen.Definitions
{
    public class GenericProperty : EntityDescription
    {
        public string Type { get; set; }
        public string Definition { get; set; }
        public string Tab { get; set; }
        public bool Mandatory { get; set; }
        public string Validation { get; set; }
    }
}