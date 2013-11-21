using System.Collections.Generic;

namespace Umbraco.CodeGen.Definitions
{
    public abstract class ContentType : Entity, IEntityDescription
    {
        public string Name { get { return Info.Name; } }
        public string Alias { get { return Info.Alias; } }
        public string Description { get { return Info.Description; } }
        
        public Info Info { get; set; }
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