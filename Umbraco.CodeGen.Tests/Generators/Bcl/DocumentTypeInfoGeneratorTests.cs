using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators.Bcl;

namespace Umbraco.CodeGen.Tests.Generators.Bcl
{
    [TestFixture]
    public class DocumentTypeInfoGeneratorTests : TypeCodeGeneratorTestBase
    {
        private DocumentTypeInfoGenerator generator;
        private DocumentTypeInfo info;
        private DocumentType documentType;

        [SetUp]
        public void SetUp()
        {
            Configuration = new CodeGeneratorConfiguration().MediaTypes;
            Candidate = Type = new CodeTypeDeclaration();
            generator = new DocumentTypeInfoGenerator(Configuration);
            documentType = new DocumentType { Info = { Alias = "aClass" } };
            info = (DocumentTypeInfo)documentType.Info;
        }

        [Test]
        public void Generate_DefaultTemplate_NotEmpty_IsFieldValue()
        {
            info.DefaultTemplate = "ATemplate";
            Generate();
            Assert.AreEqual("ATemplate", PrimitiveFieldValue("defaultTemplate"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_DefaultTemplate_NullOrWhitespace_OmitsField(string value)
        {
            info.DefaultTemplate = value;
            Generate();
            Assert.IsNull(FindField("defaultTemplate"));
        }

        [Test]
        public void Generate_AllowedTemplates_NotEmpty_IsStringArrayField()
        {
            info.AllowedTemplates = new List<string>{"ATemplate", "AnotherTemplate"};
            Generate();
            var field = FindField("allowedTemplates");
            Assert.IsNotNull(field);
            var initializer = (CodeArrayCreateExpression) field.InitExpression;
            Assert.That(
                new[]{"ATemplate", "AnotherTemplate"}.SequenceEqual(
                initializer.Initializers.Cast<CodePrimitiveExpression>().Select(ex => ex.Value)
                )
            );
        }

        [Test]
        public void Generate_AllowedTemplates_NullOrEmptyItems_OmitsEmpties()
        {
            info.AllowedTemplates = new List<string> { null, "", "  ", "AnotherTemplate" };
            Generate();
            var field = FindField("allowedTemplates");
            Assert.IsNotNull(field);
            var initializer = (CodeArrayCreateExpression)field.InitExpression;
            Assert.That(
                new[] { "AnotherTemplate" }.SequenceEqual(
                initializer.Initializers.Cast<CodePrimitiveExpression>().Select(ex => ex.Value)
                )
            );
        }

        [Test]
        public void Generate_AllowedTemplates_Empty_OmitsField()
        {
            Generate();
            Assert.IsNull(FindField("allowedTemplates"));
        }

        [Test]
        public void Generate_AllowedTemplates_AllNullOrEmptyItems_OmitsField()
        {
            info.AllowedTemplates = new List<string> { null, "", "  " };
            Generate();
            Assert.IsNull(FindField("allowedTemplates"));
        }

        private void Generate()
        {
            generator.Generate(Type, documentType);
        }
    }
}
