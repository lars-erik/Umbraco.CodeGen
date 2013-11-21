using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers.Annotated;

namespace Umbraco.CodeGen.Tests.Parsers.Annotated
{
    public class DocumentTypeInfoParserTests : ContentTypeCodeParserTestBase
    {
        private DocumentTypeInfo typedInfo;

        [SetUp]
        public void SetUp()
        {
            Configuration = new CodeGeneratorConfiguration().MediaTypes;
            Parser = new DocumentTypeInfoParser(Configuration);
            ContentType = new DocumentType();
            Info = ContentType.Info;
            typedInfo = (DocumentTypeInfo) Info;
        }

        [Test]
        public void DefaultTemplate_WhenArgument_HasArgumentValue()
        {
            const string code = @"
                [DocumentType(DefaultTemplate=""defaultTemplate"")]
                public class AClass {
                }";
            Parse(code);
            Assert.AreEqual("defaultTemplate", typedInfo.DefaultTemplate);
        }

        [Test]
        public void DefaultTemplate_WhenMissingArgument_IsNull()
        {
            Parse(EmptyClass);
            Assert.IsNull(typedInfo.DefaultTemplate);
        }

        [Test]
        public void AllowedTemplates_WhenArgument_HasArgumentValue()
        {
            const string code = @"
                [DocumentType(AllowedTemplates=new[]{
                        ""template"",
                        ""another""
                    })]
                public class AClass {
                }";
            Parse(code);
            Assert.That(
                new[] {"template", "another"}
                    .SequenceEqual(typedInfo.AllowedTemplates)
                );
        }

        [Test]
        public void AllowedTemplates_WhenMissingArgument_IsEmpty()
        {
            Parse(EmptyClass);
            Assert.AreEqual(0, typedInfo.AllowedTemplates.Count);
        }
    }
}
