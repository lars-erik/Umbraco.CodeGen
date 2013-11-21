using System;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers.Annotated;

namespace Umbraco.CodeGen.Tests.Parsers.Annotated
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
                [MediaType(Structure = new[]{
                        typeof(AnotherClass),
                        typeof(DifferentClass)
                    })]
                public class AClass {
                }";

            Parse(code);
            Console.WriteLine(String.Join(", ", ContentType.Structure));
            Assert.That(
                new[] { "anotherClass", "differentClass" }
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
