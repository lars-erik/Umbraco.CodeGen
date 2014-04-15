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
    public class StructureGeneratorTests : AnnotationCodeGeneratorTestBase
    {
        private MediaType contentType;
        private CodeAttributeDeclaration attribute;

        [SetUp]
        public void SetUp()
        {
            Generator = new StructureGenerator(Configuration);
            Configuration = CodeGeneratorConfiguration.Create().MediaTypes;
            attribute = new CodeAttributeDeclaration("MediaType");
            contentType = new MediaType();
        }

        [Test]
        public void Generate_Structure_NonEmpty_IsTypeArrayField()
        {
            contentType.Structure = new List<string> { "aClass", "anotherClass" };
            Generate();
            var initializer = FindArgument();
            Assert.That(
                new[] { "AClass", "AnotherClass" }.SequenceEqual(
                initializer.Initializers.Cast<CodeTypeOfExpression>()
                    .Select(ex => ex.Type.BaseType)
                )
            );
        }

        [Test]
        public void Generate_Structure_NullOrEmptyItems_OmitsEmpties()
        {
            contentType.Structure = new List<string> { null, "", "  ", "aClass" };
            Generate();
            var initializer = FindArgument();
            Assert.That(
                new[] { "AClass" }.SequenceEqual(
                initializer.Initializers.Cast<CodeTypeOfExpression>()
                    .Select(ex => ex.Type.BaseType)
                )
            );
        }

        [Test]
        public void Generate_Structure_Empty_OmitsField()
        {
            Generate();
            Assert.IsNull(FindAttributeArgument(attribute, "Structure"));
        }

        [Test]
        public void Generate_Structure_AllNullOrEmptyItems_OmitsField()
        {
            contentType.Structure = new List<string> { null, "", "  " };
            Generate();
            Assert.IsNull(FindAttributeArgument(attribute, "Structure"));
        }

        private void Generate()
        {
            Generator.Generate(attribute, contentType);
        }

        private CodeArrayCreateExpression FindArgument()
        {
            var argument = FindAttributeArgument(attribute, "Structure");
            var initializer = (CodeArrayCreateExpression) argument.Value;
            return initializer;
        }
    }
}
