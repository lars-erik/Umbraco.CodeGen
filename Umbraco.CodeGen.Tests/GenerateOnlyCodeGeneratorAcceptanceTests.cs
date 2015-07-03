using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests
{
    [TestFixture]
    public class GenerateOnlyCodeGeneratorAcceptanceTests : CodeGeneratorAcceptanceTestBase
    {
        [Test]
        public void BuildCode_Generates_Code_For_DocumentType()
        {
            TestBuildCode("GenerateOnlyDocumentType", "SomeDocumentType", "DocumentType");
        }

        protected override CodeGeneratorFactory CreateGeneratorFactory()
        {
            return new GenerateOnlyGeneratorFactory();
        }

        protected override void OnConfiguring(CodeGeneratorConfiguration configuration, string contentTypeName)
        {
            var config = configuration.Get(contentTypeName);
            config.BaseClass = "global::Umbraco.Core.Models.PublishedContent.PublishedContentModel";
        }
    }
}
