﻿namespace Umbraco.CodeGen.Models
{
    using global::System;
    using global::Umbraco.CodeGen.Annotations;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;
    
    [DocumentType(Description="A description of some document type", Icon="privateMemberIcon.gif", Thumbnail="privateMemberThumb.png", AllowAtRoot=true, DefaultTemplate="ATemplate", AllowedTemplates=new String[] {
            "ATemplate",
            "AnotherTemplate"}, Structure=new System.Type[] {
            typeof(SomeOtherDocType)})]
    public partial class SomeDocumentType : Umbraco.Core.Models.TypedModelBase
    {
        public SomeDocumentType(IPublishedContent content) : 
                base(content)
        {
        }
        [GenericProperty(Description="A description", Definition="Richtext editor", Tab="A tab", Mandatory=true, Validation="[a-z]")]
        public virtual String SomeProperty
        {
            get
            {
                return Content.GetPropertyValue<String>("someProperty");
            }
        }
        [GenericProperty(Description="Another description", Definition="Richtext editor", Tab="A tab")]
        public virtual String AnotherProperty
        {
            get
            {
                return Content.GetPropertyValue<String>("anotherProperty");
            }
        }
        [GenericProperty(Definition="Numeric")]
        public virtual Int32 TablessProperty
        {
            get
            {
                return Content.GetPropertyValue<Int32>("tablessProperty");
            }
        }
    }
}
