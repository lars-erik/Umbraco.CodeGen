namespace Umbraco.CodeGen.Models
{
    using System;
    using Umbraco.CodeGen.Annotations;
    using Umbraco.Core.Models;
    using Umbraco.Web;
    
    [MediaType(Icon="folder.gif", Thumbnail="folder.png", AllowAtRoot=true, Structure=new System.Type[] {
            typeof(Folder),
            typeof(Image),
            typeof(File),
            typeof(InheritedMediaFolder)})]
    public partial class InheritedMediaFolder : Folder
    {
        public InheritedMediaFolder(IPublishedContent content) : 
                base(content)
        {
        }
        [GenericProperty(Definition="Textstring", Tab="A tab")]
        public virtual String LetsHaveAProperty
        {
            get
            {
                return Content.GetPropertyValue<String>("letsHaveAProperty");
            }
        }
        [GenericProperty(Definition="Textstring")]
        public virtual String AndATablessProperty
        {
            get
            {
                return Content.GetPropertyValue<String>("andATablessProperty");
            }
        }
    }
}
