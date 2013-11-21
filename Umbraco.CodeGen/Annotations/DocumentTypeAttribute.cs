using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbraco.CodeGen.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DocumentTypeAttribute : ContentTypeAttribute
    {
        public string DefaultTemplate { get; set; }
        public string[] AllowedTemplates { get; set; }
    }
}
