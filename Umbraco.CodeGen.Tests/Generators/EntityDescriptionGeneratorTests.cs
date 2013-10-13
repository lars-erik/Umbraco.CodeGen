using System;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Tests.Generators
{
    public abstract class EntityDescriptionGeneratorTests : TypeCodeGeneratorTestBase
    {
        protected EntityDescription EntityDescription;

        [Test]
        public void Generate_Alias_PascalCasesName()
        {
            Generate();
            Assert.AreEqual(EntityDescription.Alias.PascalCase(), Candidate.Name);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Cannot generate entity with alias null or empty")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_Alias_NullOrEmpty_Throws(string alias)
        {
            EntityDescription.Alias=alias;
            Generate();
        }

        [Test]
        public void Generate_Name_NotEqualToAlias_AddsDisplayNameAttribute()
        {
            EntityDescription.Name = "A fancy name";
            Generate();
            Assert.AreEqual("A fancy name", FindAttributeValue("DisplayName"));
        }

        [Test]
        [TestCase("An Entity")]
        [TestCase("AnEntity")]
        [TestCase("anEntity")]
        public void Generate_Name_LowerCaseOrSplitEqualsAlias_OmitsDisplayName(string name)
        {
            EntityDescription.Name = name;
            Generate();
            Assert.IsNull(FindAttribute("DisplayName"));
        }

        [Test]
        public void Generate_Description_WhenNonEmpty_AddsDescriptionAttribute()
        {
            const string expectedDescription = "A fancy description";
            EntityDescription.Description = expectedDescription;
            Generate();
            Assert.AreEqual(expectedDescription, FindAttributeValue("Description"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_Description_WhenMissingOrWhiteSpace_OmitsDescription(string description)
        {
            EntityDescription.Description = description;
            Generate();
            Assert.IsNull(FindAttribute("Description"));
        }

        protected virtual void Generate()
        {
            Generator.Generate(Candidate, EntityDescription);
        }
    }
}