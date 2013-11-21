using System;

namespace Umbraco.CodeGen.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class ContentTypeAttribute : EntityDescriptionAttribute
    {
        public string Icon { get; set; }
        public string Thumbnail { get; set; }
        public bool AllowAtRoot { get; set; }
        public Type[] Structure { get; set; }
    }
}
