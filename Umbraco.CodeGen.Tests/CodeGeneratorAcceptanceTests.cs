using NUnit.Framework;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests
{
    [TestFixture]
    public class CodeGeneratorAcceptanceTests : CodeGeneratorAcceptanceTestBase
    {
        [Test]
        public void BuildCode_GeneratesCodeForDocumentType()
        {
            TestBuildCode("SomeDocumentType", "DocumentType");
        }

        [Test]
        public void BuildCode_GeneratesCodeForMediaType()
        {
            TestBuildCode("SomeMediaType", "MediaType");
        }

        protected override CodeGeneratorFactory CreateGeneratorFactory()
        {
            return new BclCodeGeneratorFactory();
        }
    }
}
