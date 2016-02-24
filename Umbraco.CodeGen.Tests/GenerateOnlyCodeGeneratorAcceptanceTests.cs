using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Tests.TestHelpers;
using Umbraco.Core.Models.PublishedContent;
using File = System.IO.File;

namespace Umbraco.CodeGen.Tests
{
    [TestFixture]
    public class GenerateOnlyCodeGeneratorAcceptanceTests : CodeGeneratorAcceptanceTestBase
    {
        [Test]
        public void BuildCode_Generates_Code_For_DocumentType()
        {
            var docType = TestFactory.CreateExpectedDocumentType();
            docType.MixinTypes.Clear();
            TestBuildCode("GenerateOnlyDocumentType", docType, "DocumentType");
        }

        [Test]
        public void BuildCode_Generates_Code_For_DocumentType_With_Composition()
        {
            TestBuildCode("GenerateOnlyDocumentTypeWithComposition", 
                TestFactory.CreateExpectedDocumentType(),
                //CreateCodeGenDocumentType(), 
                "DocumentType");
        }

        protected override CodeGeneratorFactory CreateGeneratorFactory()
        {
            return new SimpleModelGeneratorFactory();
        }

        protected override void OnConfiguring(GeneratorConfig configuration, string contentTypeName)
        {
            configuration.BaseClass = typeof(PublishedContentModel);
        }
    }
}
