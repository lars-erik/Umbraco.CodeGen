using System;
using System.CodeDom;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators.Annotated;

namespace Umbraco.CodeGen.Tests.Generators.Annotated
{
    public class EntityDescriptionGeneratorTests : AnnotationCodeGeneratorTestBase
    {
        protected EntityDescription EntityDescription;
        private DocumentType documentType;
        private CodeAttributeDeclaration attribute;

        [SetUp]
        public void SetUp()
        {
            Configuration = CodeGeneratorConfiguration.Create().MediaTypes;
            attribute = new CodeAttributeDeclaration();
            Generator = new EntityDescriptionGenerator(Configuration);
            documentType = new DocumentType { Info = { Alias = "anEntity" } };
            EntityDescription = documentType.Info;
        }

        [Test]
        public void Generate_Name_NotEqualToAlias_AddsDisplayNameAttribute()
        {
            EntityDescription.Name = "A fancy name";
            Generate();
            Assert.AreEqual("A fancy name", FindAttributeArgumentValue(attribute, "DisplayName"));
        }

        [Test]
        [TestCase("An Entity")]
        [TestCase("AnEntity")]
        [TestCase("anEntity")]
        public void Generate_Name_LowerCaseOrSplitEqualsAlias_OmitsDisplayName(string name)
        {
            EntityDescription.Name = name;
            Generate();
            Assert.IsNull(FindAttributeArgument(attribute, "DisplayName"));
        }

        [Test]
        public void Generate_Description_WhenNonEmpty_AddsDescriptionAttribute()
        {
            const string expectedDescription = "A fancy description";
            EntityDescription.Description = expectedDescription;
            Generate();
            Assert.AreEqual(expectedDescription, FindAttributeArgumentValue(attribute, "Description"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_Description_WhenMissingOrWhiteSpace_OmitsDescription(string description)
        {
            EntityDescription.Description = description;
            Generate();
            Assert.IsNull(FindAttributeArgument(attribute, "Description"));
        }

        protected virtual void Generate()
        {
            Generator.Generate(attribute, EntityDescription);
        }
    }
}