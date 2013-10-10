using System.Collections.Generic;

namespace Umbraco.CodeGen.Definitions
{
    public abstract class ContentType
    {
        public List<GenericProperty> GenericProperties { get; set; }
        public List<Tab> Tabs { get; set; }
        public List<string> Structure { get; set; }

        protected ContentType()
        {
            GenericProperties = new List<GenericProperty>();
            Tabs = new List<Tab>();
            Structure = new List<string>();
        }
    }
}