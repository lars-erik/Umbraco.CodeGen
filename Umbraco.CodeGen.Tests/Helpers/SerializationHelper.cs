using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Tests.Helpers
{
    static internal class SerializationHelper
    {
        public static void BclSerialize<T>(StringBuilder builder, T contentType)
        {
            var writer = new StringWriter(builder);
            var xmlSerializer = new XmlSerializer(typeof (T), new[] {typeof(DocumentTypeInfo)});
            xmlSerializer.Serialize(writer, contentType);
        }

        public static string BclSerialize<T>(T contentType)
        {
            var builder = new StringBuilder();
            BclSerialize(builder, contentType);
            return builder.ToString();
        }

        public static string CleanSerialize<T>(T obj, string rootName)
        {
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };
            var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName) { Namespace = "" });
            var builder = new StringBuilder();
            var writer = XmlWriter.Create(builder, settings);

            serializer.Serialize(writer, obj, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("") }));
            return builder.ToString();
        }
    }
}