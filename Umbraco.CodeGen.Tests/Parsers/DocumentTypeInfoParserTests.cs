using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen.Tests.Parsers
{
    [TestFixture]
    public class DocumentTypeInfoParserTests : InfoParserTestBase
    {
        private DocumentTypeInfo typedInfo;

        [SetUp]
        public void SetUp()
        {
            Configuration = new ContentTypeConfiguration(null);
            Parser = new DocumentTypeInfoParser(Configuration);
            ContentType = new DocumentType();
            Info = ContentType.Info;
            typedInfo = (DocumentTypeInfo) Info;
        }

        [Test]
        public void DefaultTemplate_WhenMember_HasMemberValue()
        {
            const string code = @"
                public class AClass {
                    string defaultTemplate = ""defaultTemplate"";
                }";
            Parse(code);
            Assert.AreEqual("defaultTemplate", typedInfo.DefaultTemplate);
        }

        [Test]
        public void DefaultTemplate_WhenMissingMember_IsNull()
        {
            Parse(EmptyClass);
            Assert.IsNull(typedInfo.DefaultTemplate);
        }

        [Test]
        public void AllowedTemplates_WhenMember_HasMemberValue()
        {
            const string code = @"
                public class AClass {
                    string[] allowedTemplates = new[]{
                        ""template"",
                        ""another""
                    };
                }";
            Parse(code);
            Assert.That(
                new[]{"template", "another"}
                .SequenceEqual(typedInfo.AllowedTemplates)
                );
        }

        [Test]
        public void AllowedTemplates_WhenMissingMember_IsEmpty()
        {
            Parse(EmptyClass);
            Assert.AreEqual(0, typedInfo.AllowedTemplates.Count);
        }
    }
}
