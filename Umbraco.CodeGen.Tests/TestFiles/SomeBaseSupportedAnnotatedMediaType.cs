namespace Umbraco.CodeGen.Models
{
    using System;
    using Umbraco.CodeGen.Annotations;
    
    [MediaType(Icon="folder.gif", Thumbnail="folder.png", AllowAtRoot=true, Structure=new System.Type[] {
            typeof(Folder),
            typeof(Image),
            typeof(File),
            typeof(InheritedMediaFolder)})]
    public partial class InheritedMediaFolder : Folder
    {
        public InheritedMediaFolder(Umbraco.Core.Models.IPublishedContent content) : 
                base(content)
        {
        }
        [GenericProperty(Definition="Textstring", Tab="A tab")]
        public virtual String LetsHaveAProperty
        {
            get
            {
                return GetValue<String>("letsHaveAProperty");
            }
        }
        [GenericProperty(Definition="Textstring")]
        public virtual String AndATablessProperty
        {
            get
            {
                return GetValue<String>("andATablessProperty");
            }
        }
    }
}
