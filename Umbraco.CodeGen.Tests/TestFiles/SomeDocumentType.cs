namespace Umbraco.CodeGen.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Umbraco.Core.Models;
    using Umbraco.Web;
    
    [Description("A description of some document type")]
    public partial class SomeDocumentType : Umbraco.Core.Models.TypedModelBase
    {
        private string icon = "privateMemberIcon.gif";
        private string thumbnail = "privateMemberThumb.png";
        private bool allowAtRoot = true;
        private string defaultTemplate = "ATemplate";
        private string[] allowedTemplates = new string[] {
                "ATemplate",
                "AnotherTemplate"};
        private System.Type[] structure = new System.Type[] {
                typeof(SomeOtherDocType)};
        public SomeDocumentType(IPublishedContent content) : 
                base(content)
        {
        }
        [Description("A description")]
        [DataType("Richtext editor")]
        [Category("A tab")]
        [Required()]
        [RegularExpression("[a-z]")]
        public virtual String SomeProperty
        {
            get
            {
                return Content.GetPropertyValue<String>("someProperty");
            }
        }
        [Description("Another description")]
        [DataType("Richtext editor")]
        [Category("A tab")]
        public virtual String AnotherProperty
        {
            get
            {
                return Content.GetPropertyValue<String>("anotherProperty");
            }
        }
        [DataType("Numeric")]
        public virtual Int32 TablessProperty
        {
            get
            {
                return Content.GetPropertyValue<Int32>("tablessProperty");
            }
        }
    }
}
