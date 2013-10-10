namespace Umbraco.CodeGen.Definitions
{
    public class Info : Entity
    {
        public string Icon { get; set; }
        public string Thumbnail { get; set; }
        public bool AllowAtRoot { get; set; }
        public string Master { get; set; }
    }
}