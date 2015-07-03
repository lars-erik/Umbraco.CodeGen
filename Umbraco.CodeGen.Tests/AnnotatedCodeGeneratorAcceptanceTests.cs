using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Generators.Annotated;

namespace Umbraco.CodeGen.Tests
{
    [TestFixture]
    public class AnnotatedCodeGeneratorAcceptanceTests : CodeGeneratorAcceptanceTestBase
    {
        [Test]
        public void BuildCode_GeneratesCodeForDocumentType()
        {
            TestBuildCode("SomeAnnotatedDocumentType", "SomeDocumentType", "DocumentType");
        }

        [Test]
        public void BuildCode_GeneratesCodeForMediaType()
        {
            TestBuildCode("SomeAnnotatedMediaType", "SomeMediaType", "MediaType");
        }

        protected override CodeGeneratorFactory CreateGeneratorFactory()
        {
            return new AnnotatedCodeGeneratorFactory();
        }
    }
}
