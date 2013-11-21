using System;

namespace Umbraco.CodeGen.Annotations
{
    public abstract class EntityDescriptionAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}