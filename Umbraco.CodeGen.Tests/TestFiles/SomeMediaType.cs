namespace Umbraco.CodeGen.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Umbraco.Core.Models;
    using Umbraco.Web;
    
    public partial class InheritedMediaFolder : Folder
    {
        private string icon = "folder.gif";
        private string thumbnail = "folder.png";
        private bool allowAtRoot = true;
        private System.Type[] structure = new System.Type[] {
                typeof(Folder),
                typeof(Image),
                typeof(File),
                typeof(InheritedMediaFolder)};
        public InheritedMediaFolder(IPublishedContent content) : 
                base(content)
        {
        }
        [DataType("Textstring")]
        [Category("A tab")]
        public virtual String LetsHaveAProperty
        {
            get
            {
                return Content.GetPropertyValue<String>("letsHaveAProperty");
            }
        }
        [DataType("Textstring")]
        public virtual String AndATablessProperty
        {
            get
            {
                return Content.GetPropertyValue<String>("andATablessProperty");
            }
        }
    }
}
