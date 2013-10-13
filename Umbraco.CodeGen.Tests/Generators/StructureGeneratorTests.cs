using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests.Generators
{
    [TestFixture]
    public class StructureGeneratorTests : TypeCodeGeneratorTestBase
    {
        private Info info;

        [SetUp]
        public void SetUp()
        {
            Generator = new StructureGenerator(Configuration);
            Configuration = new ContentTypeConfiguration(null);
            Candidate = Type = new CodeTypeDeclaration();
            ContentType = new MediaType();
        }

        [Test]
        public void Generate_Structure_NonEmpty_IsTypeArrayField()
        {
            ContentType.Structure = new List<string> { "aClass", "anotherClass" };
            Generate();
            var field = FindField("structure");
            var initializer = (CodeArrayCreateExpression)field.InitExpression;
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
            ContentType.Structure = new List<string> { null, "", "  ", "aClass" };
            Generate();
            var field = FindField("structure");
            var initializer = (CodeArrayCreateExpression)field.InitExpression;
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
            Assert.IsNull(FindField("structure"));
        }

        [Test]
        public void Generate_Structure_AllNullOrEmptyItems_OmitsField()
        {
            ContentType.Structure = new List<string> { null, "", "  " };
            Generate();
            Assert.IsNull(FindField("structure"));
        }

        private void Generate()
        {
            Generator.Generate(Type, ContentType);
        }

    }
}
