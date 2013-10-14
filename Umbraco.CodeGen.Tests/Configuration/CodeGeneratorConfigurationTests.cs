using System;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Tests.Helpers;

namespace Umbraco.CodeGen.Tests.Configuration
{
    [TestFixture]
    public class CodeGeneratorConfigurationTests
    {
        [Test]
        [TestCase(@"
            <CodeGenerator>
                <DocumentTypes/>
                <MediaTypes/>
                <TypeMappings/>
            </CodeGenerator>")]
        [TestCase("<CodeGenerator />")]
        public void Deserialize_CodeGeneratorConfiguration_Empties_IsAllDefaultInstancesWithReferences(string xml)
        {
            var config = Deserialize<CodeGeneratorConfiguration>(xml);
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.DocumentTypes);
            Assert.AreSame(config, config.DocumentTypes.Config);
            Assert.IsNotNull(config.MediaTypes);
            Assert.AreSame(config, config.MediaTypes.Config);
            Assert.IsNotNull(config.TypeMappings);
            Assert.IsNotNull(config.TypeMappings.Items);
        }

        [Test]
        public void Deserialize_ContentTypeConfiguration_Empty_AllNullOrFalse()
        {
            var config = Deserialize<ContentTypeConfiguration>(@"<ContentTypeConfiguration />");
            Assert.IsNull(config.ModelPath);
            Assert.IsNull(config.BaseClass);
            Assert.IsNull(config.Namespace);
            Assert.IsFalse(config.GenerateClasses);
            Assert.IsFalse(config.GenerateXml);
        }

        [Test]
        public void Serialize_TypeMappings_Empty_IsSingleClosedElement()
        {
            var xml = SerializationHelper.CleanSerialize(new TypeMappings(), "TypeMappings");
            Assert.AreEqual("<TypeMappings />", xml);
            Console.WriteLine(xml);
        }

        [Test]
        public void Serialize_TypeMappings_HasDefaultType_IsSingleClosedElementWithDefaultTypeAttr()
        {
            var xml = SerializationHelper.CleanSerialize(new TypeMappings{DefaultType="x"}, "TypeMappings");
            Assert.AreEqual(@"<TypeMappings DefaultType=""x"" />", xml);
            Console.WriteLine(xml);
        }

        [Test]
        public void Serialize_TypeMappings_WritesItems()
        {
            var xml = SerializationHelper.CleanSerialize(new TypeMappings(new[]{new TypeMapping("a", "b"), new TypeMapping("c", "d")}), "TypeMappings");
            Assert.AreEqual(
@"<TypeMappings>
  <TypeMapping DataTypeId=""a"" Type=""b"" />
  <TypeMapping DataTypeId=""c"" Type=""d"" />
</TypeMappings>", xml);
        }

        [Test]
        public void Deserialize_TypeMappings_HasAttributeValues()
        {
            var xml = @"<TypeMappings DefaultType=""x"" DefaultDefinitionId=""y"" />";
            var typeMappings = Deserialize<TypeMappings>(xml);
            Assert.AreEqual("x", typeMappings.DefaultType);
            Assert.AreEqual("y", typeMappings.DefaultDefinitionId);
        }

        [Test]
        public void Deserialize_TypeMappings_Empty_HasDefaultValues()
        {
            const string xml = @"<TypeMappings />";
            var typeMappings = Deserialize<TypeMappings>(xml);
            Assert.AreEqual(TypeMappings.Defaults.DefaultDefinitionId, typeMappings.DefaultDefinitionId);
            Assert.AreEqual(TypeMappings.Defaults.DefaultType, typeMappings.DefaultType);
        }

        [Test]
        public void Deserialize_TypeMappings_HasItems()
        {
            const string xml = 
@"<TypeMappings>
  <TypeMapping DataTypeId=""a"" Type=""b"" />
  <TypeMapping DataTypeId=""c"" Type=""d"" />
</TypeMappings>";
            var mappings = Deserialize<TypeMappings>(xml);
            Assert.AreEqual(2, mappings.Count);
            Assert.AreEqual("d", mappings["c"]);
        }

        private static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(new StringReader(xml));
        }

    }
}
