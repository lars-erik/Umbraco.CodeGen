using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Generators.Annotated;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests
{
    [TestFixture]
    public class BaseSupportedAnnotatedCodeGeneratorAcceptanceTests : CodeGeneratorAcceptanceTestBase
    {
        [Test]
        public void BuildCode_GeneratesCodeForDocumentType()
        {
            TestBuildCode("SomeBaseSupportedAnnotatedDocumentType", "SomeDocumentType", "DocumentType");
        }

        [Test]
        public void BuildCode_GeneratesCodeForMediaType()
        {
            TestBuildCode("SomeBaseSupportedAnnotatedMediaType", "SomeMediaType", "MediaType");
        }

        protected override void OnConfiguring(CodeGeneratorConfiguration configuration, string contentTypeName)
        {
            var typeConfig = configuration.Get(contentTypeName);
            typeConfig.BaseClass = "BaseClassWithSupport";
        }

        protected override CodeGeneratorFactory CreateGeneratorFactory()
        {
            return new BaseSupportedAnnotatedCodeGeneratorFactory();
        }
    }
}
