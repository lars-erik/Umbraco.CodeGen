using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Tests
{
    static internal class SerializationHelper
    {
        public static void BclSerialize<T>(StringBuilder builder, T contentType)
        {
            var writer = new StringWriter(builder);
            var xmlSerializer = new XmlSerializer(typeof (T), new[] {typeof(DocumentTypeInfo)});
            xmlSerializer.Serialize(writer, contentType);
        }
    }
}