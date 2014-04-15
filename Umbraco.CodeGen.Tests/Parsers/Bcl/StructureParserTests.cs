using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers.Bcl;

namespace Umbraco.CodeGen.Tests.Parsers.Bcl
{
    [TestFixture]
    public class StructureParserTests : ContentTypeCodeParserTestBase
    {
        [SetUp]
        public void SetUp()
        {
            Configuration = CodeGeneratorConfiguration.Create().MediaTypes;
            Parser = new StructureParser(Configuration);
            ContentType = new MediaType();
        }

        [Test]
        public void Parse_Structure_WhenStructureMember_HasMemberValue()
        {
            const string code = @"
                public class AClass {
                    Type[] structure = new[]{
                        typeof(AnotherClass),
                        typeof(DifferentClass)
                    };
                }";

            Parse(code);
            Assert.That(
                new[]{"AnotherClass", "DifferentClass"}
                .SequenceEqual(ContentType.Structure)
                );
        }

        [Test]
        public void Parse_Structure_WhenStructureMemberMissing_IsEmpty()
        {
            Parse(EmptyClass);
            Assert.AreEqual(0, ContentType.Structure.Count);
        }
    }
}
