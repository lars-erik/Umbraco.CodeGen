using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen.Tests.Parsers
{
    [TestFixture]
    public class StructureParserTests : ContentTypeCodeParserTestBase
    {
        [SetUp]
        public void SetUp()
        {
            Configuration = new CodeGeneratorConfiguration().MediaTypes;
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
                new[]{"anotherClass", "differentClass"}
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
