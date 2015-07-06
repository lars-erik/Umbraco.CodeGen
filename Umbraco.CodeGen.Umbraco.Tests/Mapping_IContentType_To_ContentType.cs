using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Models;
using ContentType = Umbraco.Core.Models.ContentType;

namespace Umbraco.CodeGen.Umbraco.Tests
{
    [TestFixture]
    public class Mapping_IContentType_To_ContentType
    {
        private DocumentType expected;
        private ContentType umbracoContentType;

        [Test]
        public void Maps_Metadata()
        {
            var actual = ContentTypeMapping.Map(umbracoContentType);
            Assert.AreEqual(expected, actual, Serialize(actual));
        }

        [Test]
        public void Ignores_DefaultTemplate_If_None()
        {
            umbracoContentType.SetDefaultTemplate(null);
            ((DocumentTypeInfo) expected.Info).DefaultTemplate = null;

            var actual = ContentTypeMapping.Map(umbracoContentType);
            Assert.AreEqual(expected, actual, Serialize(actual));
        }

        private static DocumentType CreateCodeGenDocumentType()
        {
            var expected = new DocumentType
            {
                Info = new DocumentTypeInfo
                {
                    Alias = "SomeDocumentType",
                    AllowAtRoot = true,
                    AllowedTemplates = new List<string> {"ATemplate", "AnotherTemplate"},
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
                Composition = new List<Definitions.ContentType>
                {
                    new Definitions.ContentType
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
                                Type = "Umbraco.Number",
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
                        Type = "Umbraco.Richtext"
                    },
                    new GenericProperty
                    {
                        Alias = "anotherProperty",
                        Definition = null,
                        Description = "Another description",
                        Name = "Another property",
                        Tab = "A tab",
                        Type = "Umbraco.Richtext"
                    },
                    new GenericProperty
                    {
                        Alias = "tablessProperty",
                        Definition = null,
                        Name = "Tabless property",
                        Type = "Umbraco.Number"
                    },
                }
            };
            return expected;
        }

        [SetUp]
        public void Setup()
        {
            MockSettings();

            expected = CreateCodeGenDocumentType();
            umbracoContentType = CreateUmbracoContentType();
        }

        private static void MockSettings()
        {
            var settings = new Mock<IUmbracoSettingsSection>();
            var requestHandler = new Mock<IRequestHandlerSection>();
            settings.Setup(m => m.RequestHandler).Returns(requestHandler.Object);
            requestHandler.Setup(m => m.CharCollection).Returns(new IChar[0]);
            SetUmbracoSettings(settings.Object);
        }

        private static ContentType CreateUmbracoContentType()
        {
            var umbracoContentType = new Core.Models.ContentType(1)
            {
                Id = 2,
                Alias = "SomeDocumentType",
                Name = "Some document type",
                AllowedAsRoot = true,
                AllowedContentTypes = new[] {new ContentTypeSort(new Lazy<int>(() => 3), 1, "SomeOtherDocType"),},
                AllowedTemplates = new[] {new Template("", "", "ATemplate") { Id = 1 }, new Template("", "", "AnotherTemplate") { Id = 2 },},
                PropertyGroups = new PropertyGroupCollection(new[]
                {
                    new PropertyGroup
                    {
                        Name = "A tab",
                        PropertyTypes = new PropertyTypeCollection(new[]
                        {
                            new PropertyType(new DataTypeDefinition(-1, "Umbraco.Richtext"), "Richtext editor")
                            {
                                Alias = "someProperty",
                                Name = "Some property",
                                Description = "A description"
                            },
                            new PropertyType(new DataTypeDefinition(-1, "Umbraco.Richtext"), "Richtext editor")
                            {
                                Alias = "anotherProperty",
                                Name = "Another property",
                                Description = "Another description"
                            }
                        })
                    },
                    new PropertyGroup(new PropertyTypeCollection(new[]
                    {
                        new PropertyType(new DataTypeDefinition(-1, "Umbraco.Number"), "Numeric")
                        {
                            Alias = "tablessProperty",
                            Name = "Tabless property"
                        },
                    }))
                }),
                Description = "A description of some document type",
                Icon = "privateMemberIcon.gif",
                Thumbnail = "privateMemberThumb.png",
            };
            umbracoContentType.SetDefaultTemplate(umbracoContentType.AllowedTemplates.First());
            umbracoContentType.AddContentType(CreateMaster());
            umbracoContentType.AddContentType(CreateMixin());

            return umbracoContentType;
        }

        private static ContentType CreateMaster()
        {
            var master = new Core.Models.ContentType(-1)
            {
                Alias = "RootDocType",
                Name = "Root document type",
                AllowedAsRoot = true,
                Id = 1
            };
            return master;
        }

        private static ContentType CreateMixin()
        {
            var mixin = new Core.Models.ContentType(-1)
            {
                Id = 3,
                Alias = "Mixin",
                Name = "Some mixin",
                AllowedAsRoot = false,
                PropertyGroups = new PropertyGroupCollection(new[]
                {
                    new PropertyGroup
                    {
                        Name = "Mixin tab",
                        PropertyTypes = new PropertyTypeCollection(new[]
                        {
                            new PropertyType(new DataTypeDefinition(-1, "Umbraco.Number"), "Number")
                            {
                                Alias = "mixinProp",
                                Name = "Mixin prop",
                            }
                        })
                    }
                }),
                Description = "A description of some mixin type",
                Icon = "privateMemberIcon.gif",
                Thumbnail = "privateMemberThumb.png",
            };
            return mixin;
        }

        private static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        private static void SetUmbracoSettings(IUmbracoSettingsSection umbracoSettingsSection)
        {
            var type = typeof (UmbracoConfig);
            var method = type.GetMethod("SetUmbracoSettings", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(UmbracoConfig.For, new object[] {umbracoSettingsSection});
        }
    }
}
