﻿using System;
using System.CodeDom;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators.Annotated;

namespace Umbraco.CodeGen.Tests.Generators.Annotated
{
    [TestFixture]
    public class CommonInfoGeneratorTests : AnnotationCodeGeneratorTestBase
    {
        private Info info;
        private ContentType contentType;
        private CodeTypeDeclaration type;
        private CodeAttributeDeclaration attribute;

        [SetUp]
        public void SetUp()
        {
            Configuration = new CodeGeneratorConfiguration().MediaTypes;
            contentType = new MediaType { Info = { Alias = "anEntity" } };
            Candidate = type = new CodeTypeDeclaration();
            info = contentType.Info;

            attribute = new CodeAttributeDeclaration("MediaType");
            type.CustomAttributes.Add(attribute);

            Generator = new CommonInfoGenerator(Configuration);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Common info generator must be used on an attribute declaration")]
        public void Generate_NotCodeTypeDeclaration_Throws()
        {
            Generator.Generate(new CodeMemberProperty(), contentType);;
        }

        [Test]
        public void Generate_Icon_WhenHasValue_IsAttributeArgumentWithValue()
        {
            info.Icon = "icon.gif";
            Generate();
            var argValue = FindAttributeArgumentValue(attribute, "Icon");
            Assert.AreEqual(info.Icon, argValue);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_Icon_NoValue_OmitsIconArgument(string value)
        {
            info.Icon = value;
            Generate();
            Assert.IsNull(FindAttributeArgument(attribute, "Icon"));
        }

        [Test]
        public void Generate_Thumbnail_WhenHasValue_IsAttributeArgumentWithValue()
        {
            info.Thumbnail = "thumb.png";
            Generate();
            var argValue = FindAttributeArgumentValue(attribute, "Thumbnail");
            Assert.AreEqual(info.Thumbnail, argValue);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_Thumbnail_NoValue_OmitsIconArgument(string value)
        {
            info.Thumbnail = value;
            Generate();
            Assert.IsNull(FindAttributeArgument(attribute, "Thumbnail"));
        }

        [Test]
        public void Generate_AllowAtRoot_WhenTrue_IsAttributeArgumentWithValue()
        {
            info.AllowAtRoot = true;
            Generate();
            var argValue = FindAttributeArgumentValue(attribute, "AllowAtRoot");
            Assert.That(argValue, Is.True);
        }

        [Test]
        public void Generate_AllowAtRoot_False_OmitsAllowAtRootArgument()
        {
            Generate();
            Assert.IsNull(FindAttributeArgument(attribute, "AllowAtRoot"));
        }

        private void Generate()
        {
            Generator.Generate(attribute, contentType);
        }
    }
}