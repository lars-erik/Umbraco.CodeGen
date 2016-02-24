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
        public virtual System.Web.IHtmlString SomeProperty
        {
            get
            {
                return Content.GetPropertyValue<System.Web.IHtmlString>("someProperty");
            }
        }
        public virtual System.Web.IHtmlString AnotherProperty
        {
            get
            {
                return Content.GetPropertyValue<System.Web.IHtmlString>("anotherProperty");
            }
        }
        public virtual int TablessProperty
        {
            get
            {
                return Content.GetPropertyValue<int>("tablessProperty");
            }
        }
        public virtual int MixinProp
        {
            get
            {
                return Content.GetPropertyValue<int>("mixinProp");
            }
        }
    }
}
