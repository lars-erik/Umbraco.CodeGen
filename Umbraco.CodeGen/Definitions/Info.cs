namespace Umbraco.CodeGen.Definitions
{
    public class Info : EntityDescription
    {
        public string Icon { get; set; }
        public string Thumbnail { get; set; }
        public bool AllowAtRoot { get; set; }
        public string Master { get; set; }
    }
}