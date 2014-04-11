namespace Umbraco.CodeGen.Models
{
    using System;
    using Umbraco.CodeGen.Annotations;
    
    [DocumentType(Description="A description of some document type", Icon="privateMemberIcon.gif", Thumbnail="privateMemberThumb.png", AllowAtRoot=true, DefaultTemplate="ATemplate", AllowedTemplates=new String[] {
            "ATemplate",
            "AnotherTemplate"}, Structure=new System.Type[] {
            typeof(SomeOtherDocType)})]
    public partial class SomeDocumentType : BaseClassWithSupport
    {
        public SomeDocumentType(Umbraco.Core.Models.IPublishedContent content) : 
                base(content)
        {
        }
        [GenericProperty(Description="A description", Definition="Richtext editor", Tab="A tab", Mandatory=true, Validation="[a-z]")]
        public virtual String SomeProperty
        {
            get
            {
                return GetValue<String>("someProperty");
            }
        }
        [GenericProperty(Description="Another description", Definition="Richtext editor", Tab="A tab")]
        public virtual String AnotherProperty
        {
            get
            {
                return GetValue<String>("anotherProperty");
            }
        }
        [GenericProperty(Definition="Numeric")]
        public virtual Int32 TablessProperty
        {
            get
            {
                return GetValue<Int32>("tablessProperty");
            }
        }
    }
}
