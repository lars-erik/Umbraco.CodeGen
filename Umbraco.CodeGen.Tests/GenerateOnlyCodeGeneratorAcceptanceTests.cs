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
using File = System.IO.File;

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

        [Test]
        public void BuildCode_Generates_Code_For_DocumentType_With_Composition()
        {
            TestBuildCode("GenerateOnlyDocumentTypeWithComposition", CreateCodeGenDocumentType(), "DocumentType");
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

        private static DocumentType CreateCodeGenDocumentType()
        {
            var expected = new DocumentType
            {
                Info = new DocumentTypeInfo
                {
                    Alias = "SomeDocumentType",
                    AllowAtRoot = true,
                    AllowedTemplates = new List<string> { "ATemplate", "AnotherTemplate" },
                    DefaultTemplate = "ATemplate",
                    Description = "A description of some document type",
                    Icon = "privateMemberIcon.gif",
                    Master = "",
                    Name = "Some document type",
                    Thumbnail = "privateMemberThumb.png"
                },
                Tabs = new List<Tab>
                {
                    new Tab {Caption = "A tab"},
                    new Tab()
                },
                Structure = new List<string>
                {
                    "SomeOtherDocType"
                },
                Composition = new List<ContentType>
                {
                    new ContentType
                    {
                        Info = new Info
                        {
                            Alias = "Mixin"
                        },
                        Tabs = new List<Tab>{new Tab{Caption="Mixin tab"}},
                        GenericProperties = new List<GenericProperty>
                        {
                            new GenericProperty
                            {
                                Alias = "mixinProp",
                                Name = "Mixin prop",
                                PropertyEditorAlias = "Umbraco.Integer",
                                Tab = "Mixin tab"
                            }
                        }
                    }
                },
                GenericProperties = new List<GenericProperty>
                {
                    new GenericProperty
                    {
                        Alias = "someProperty",
                        Definition = null,
                        Description = "A description",
                        Name = "Some property",
                        Tab = "A tab",
                        PropertyEditorAlias = "Umbraco.TinyMCEv3"
                    },
                    new GenericProperty
                    {
                        Alias = "anotherProperty",
                        Definition = null,
                        Description = "Another description",
                        Name = "Another property",
                        Tab = "A tab",
                        PropertyEditorAlias = "Umbraco.TinyMCEv3"
                    },
                    new GenericProperty
                    {
                        Alias = "tablessProperty",
                        Definition = null,
                        Name = "Tabless property",
                        PropertyEditorAlias = "Umbraco.Integer"
                    },
                }
            };
            return expected;
        }
    }
}
