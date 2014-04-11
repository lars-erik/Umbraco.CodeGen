using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Parsers;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests
{
    [TestFixture]
    public class CodeGeneratorAcceptanceTests
    {
        [Test]
        public void BuildCode_GeneratesCodeForDocumentType()
        {
            TestBuildCode("SomeDocumentType", "DocumentType");
        }

        [Test]
        public void BuildCode_GeneratesCodeForMediaType()
        {
            TestBuildCode("SomeMediaType", "MediaType");
        }

        private static void TestBuildCode(string fileName, string contentTypeName)
        {
            ContentType contentType;
            var expectedOutput = "";
            using (var inputReader = File.OpenText(@"..\..\TestFiles\" + fileName + ".xml"))
            {
                contentType = new ContentTypeSerializer().Deserialize(inputReader);
            }
            using (var goldReader = File.OpenText(@"..\..\TestFiles\" + fileName + ".cs"))
            {
                expectedOutput = goldReader.ReadToEnd();
            }

            var configuration = new CodeGeneratorConfiguration();
            configuration.TypeMappings.Add(new TypeMapping("Umbraco.Integer", "Int32"));
            var typeConfig = configuration.Get(contentTypeName);
            typeConfig.BaseClass = "Umbraco.Core.Models.TypedModelBase";
            typeConfig.Namespace = "Umbraco.CodeGen.Models";

            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            var factory = new BclCodeGeneratorFactory();
            var dataTypeProvider = new TestDataTypeProvider();
            var generator = new CodeGenerator(typeConfig, dataTypeProvider, factory);
            generator.Generate(contentType, writer);

            writer.Flush();
            Console.WriteLine(sb.ToString());

            Assert.AreEqual(expectedOutput, sb.ToString());
        }
    }
}
