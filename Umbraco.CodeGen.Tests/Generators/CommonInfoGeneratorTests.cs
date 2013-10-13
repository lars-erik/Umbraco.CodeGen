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
    public class CommonInfoGeneratorTests
    {
        private CommonInfoGenerator generator;
        private CodeTypeDeclaration type;

        [SetUp]
        public void SetUp()
        {
            generator = new CommonInfoGenerator(null);
            type = new CodeTypeDeclaration();
        }

        [Test]
        public void Generate_Alias_PascalCasesClassName()
        {
            var contentType = new MediaType {Info = {Alias = "aClass"}};
            Generate(contentType);
            Assert.AreEqual("AClass", type.Name);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Cannot generate class with alias null or empty")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_Alias_NullOrEmpty_Throws(string alias)
        {
            var contentType = new MediaType {Info={Alias=alias}};
            Generate(contentType);
        }

        [Test]
        public void Generate_Name_NotEqualToAlias_AddsDisplayNameAttribute()
        {
            var contentType = new MediaType {Info = {Alias = "aClass", Name = "A fancy class"}};
            Generate(contentType);
            Assert.AreEqual("A fancy class", FindAttributeValue("DisplayName"));
        }

        [Test]
        [TestCase("A Class")]
        [TestCase("AClass")]
        [TestCase("aClass")]
        public void Generate_Name_LowerCaseOrSplitEqualsAlias_DoesNotAddDisplayNameAttribute(string name)
        {
            var contentType = new MediaType {Info = {Alias = "aClass", Name = name}};
            Generate(contentType);
            Assert.AreEqual(0, type.CustomAttributes.Count);
        }

        [Test]
        public void Generate_Description_WhenNonEmpty_AddsDescriptionAttribute()
        {
            var contentType = new MediaType {Info = {Alias = "aClass", Description = "A fancy class"}};
            Generate(contentType);
            Assert.AreEqual("A fancy class", FindAttributeValue("Description"));
        }

        private void Generate(ContentType contentType)
        {
            generator.Generate(type, contentType);
        }

        private object FindAttributeValue(string attributeName)
        {
            var attribute = type.CustomAttributes.Cast<CodeAttributeDeclaration>().Single(att => att.Name == attributeName);
            var value = ((CodePrimitiveExpression) attribute.Arguments[0].Value).Value;
            return value;
        }
    }
}
