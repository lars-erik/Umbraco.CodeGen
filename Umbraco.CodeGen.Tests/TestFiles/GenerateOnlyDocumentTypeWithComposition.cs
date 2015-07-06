namespace Umbraco.CodeGen.Models
{
    using global::System;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;
    
    public partial class SomeDocumentType : global::Umbraco.Core.Models.PublishedContent.PublishedContentModel, IMixin
    {
        public SomeDocumentType(IPublishedContent content) : 
                base(content)
        {
        }
        public virtual String SomeProperty
        {
            get
            {
                return Content.GetPropertyValue<String>("someProperty");
            }
        }
        public virtual String AnotherProperty
        {
            get
            {
                return Content.GetPropertyValue<String>("anotherProperty");
            }
        }
        public virtual Int32 TablessProperty
        {
            get
            {
                return Content.GetPropertyValue<Int32>("tablessProperty");
            }
        }
        public virtual Int32 MixinProp
        {
            get
            {
                return Content.GetPropertyValue<Int32>("mixinProp");
            }
        }
    }
}
