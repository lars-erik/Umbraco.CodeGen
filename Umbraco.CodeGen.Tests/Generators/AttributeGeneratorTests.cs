using System;
using System.CodeDom;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests.Generators
{
    [TestFixture]
    public class AttributeGeneratorTests : TypeCodeGeneratorTestBase
    {
        private SpyGenerator spy1;
        private SpyGenerator spy2;

        [SetUp]
        public void SetUp()
        {
            Configuration = CodeGeneratorConfiguration.Create().DocumentTypes;
            spy1 = new SpyGenerator();
            spy2 = new SpyGenerator();
        }

        [Test]
        public void Generate_DocumentType_OnType_AddsAndPassesAttributeToChildGenerators()
        {
            SetupDocumentType();
            Generate_AddsAndPassesAttributeToChildren("DocumentType", GenerateContentType);
        }

        [Test]
        public void Generate_MediaType_OnType_AddsAndPassesAttributeToChildGenerators()
        {
            SetupDocumentType();
            Generate_AddsAndPassesAttributeToChildren("MediaType", GenerateContentType);
        }

        [Test]
        public void Generate_GenericProperty_OnProperty_AddsAndPassesAttributeToChildGenerators()
        {
            SetupDocumentType();
            Generate_AddsAndPassesAttributeToChildren("GenericProperty", GenerateProperty);
        }

        private void Generate_AddsAndPassesAttributeToChildren(string attributeName, Action generateDelegate)
        {
            Generator = new AttributeCodeGenerator(attributeName, Configuration, spy1, spy2);
            generateDelegate();
            var attribute = FindAttribute(attributeName);
            Assert.AreSame(attribute, spy1.CodeObjects[0]);
            Assert.AreSame(attribute, spy2.CodeObjects[0]);
        }

        private void SetupDocumentType()
        {
            ContentType = new DocumentType();
            Generator = new AttributeCodeGenerator("DocumentType", Configuration);
            Candidate = new CodeTypeDeclaration();
        }

        private void SetupMediaType()
        {
            ContentType = new MediaType();
            Generator = new AttributeCodeGenerator("MediaType", Configuration);
            Candidate = new CodeTypeDeclaration();
        }

        private void SetupProperty()
        {
            ContentType = new MediaType();
            Generator = new AttributeCodeGenerator("GenericProperty", Configuration);
            Candidate = new CodeMemberProperty();
        }

        private void GenerateContentType()
        {
            Generator.Generate(Candidate, ContentType);
        }

        private void GenerateProperty()
        {
            Generator.Generate(Candidate, new GenericProperty());
        }
    }
}
