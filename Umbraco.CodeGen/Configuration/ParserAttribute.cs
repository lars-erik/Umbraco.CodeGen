using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.CodeGen.Configuration
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ParserAttribute : Attribute
    {
        public Type Type { get; set; }

        public ParserAttribute(Type type)
        {
            Type = type;
        }
    }
}
