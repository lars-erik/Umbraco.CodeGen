using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators.Annotated;

namespace Umbraco.CodeGen.Tests.Generators.Annotated
{
    [TestFixture]
    public class DocumentTypeInfoGeneratorTests : AnnotationCodeGeneratorTestBase
    {
        private DocumentTypeInfoGenerator generator;
        private DocumentTypeInfo info;
        private CodeAttributeDeclaration attribute;
        private DocumentType documentType;

        [SetUp]
        public void SetUp()
        {
            Configuration = CodeGeneratorConfiguration.Create().MediaTypes;
            attribute = new CodeAttributeDeclaration("DocumentType");
            generator = new DocumentTypeInfoGenerator(Configuration);
            documentType = new DocumentType
            {
                Info = info = new DocumentTypeInfo {Alias = "aClass"}
            };
        }

        [Test]
        public void Generate_DefaultTemplate_NotEmpty_IsAttributeArgument()
        {
            info.DefaultTemplate = "ATemplate";
            Generate();
            Assert.AreEqual("ATemplate", FindAttributeArgumentValue(attribute, "DefaultTemplate"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_DefaultTemplate_NullOrWhitespace_OmitsField(string value)
        {
            info.DefaultTemplate = value;
            Generate();
            Assert.IsNull(FindAttributeArgument(attribute, "DefaultTemplate"));
        }

        [Test]
        public void Generate_AllowedTemplates_NotEmpty_IsStringArrayAttributeArgument()
        {
            info.AllowedTemplates = new List<string>{"ATemplate", "AnotherTemplate"};
            Generate();
            var argument = FindAttributeArgument(attribute, "AllowedTemplates");
            Assert.IsNotNull(argument);
            var initializer = (CodeArrayCreateExpression) argument.Value;
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
            var argument = FindAttributeArgument(attribute, "AllowedTemplates");
            Assert.IsNotNull(argument);
            var initializer = (CodeArrayCreateExpression)argument.Value;
            Assert.That(
                new[] { "AnotherTemplate" }.SequenceEqual(
                initializer.Initializers.Cast<CodePrimitiveExpression>().Select(ex => ex.Value)
                )
            );
        }

        [Test]
        public void Generate_AllowedTemplates_Empty_OmitsArgument()
        {
            Generate();
            Assert.IsNull(FindAttributeArgument(attribute, "AllowedTemplates"));
        }

        [Test]
        public void Generate_AllowedTemplates_AllNullOrEmptyItems_OmitsArgument()
        {
            info.AllowedTemplates = new List<string> { null, "", "  " };
            Generate();
            Assert.IsNull(FindAttributeArgument(attribute, "AllowedTemplates"));
        }

        private void Generate()
        {
            generator.Generate(attribute, documentType);
        }
    }
}
