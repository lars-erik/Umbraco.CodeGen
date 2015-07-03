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
        [Test]
        public void Maps_Metadata()
        {
            var expected = CreateCodeGenDocumentType();
            var umbracoContentType = CreateUmbracoContentType();

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

        private static ContentType CreateUmbracoContentType()
        {
            var umbracoContentType = new Core.Models.ContentType(-1)
            {
                Alias = "SomeDocumentType",
                Name = "Some document type",
                AllowedAsRoot = true,
                AllowedContentTypes = new[] {new ContentTypeSort(new Lazy<int>(() => 1), 1, "SomeOtherDocType"),},
                AllowedTemplates = new[] {new Template("", "", "ATemplate"), new Template("", "", "AnotherTemplate"),},
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
            return umbracoContentType;
        }

        private static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        [SetUp]
        public void Setup()
        {
            var settings = new Mock<IUmbracoSettingsSection>();
            var requestHandler = new Mock<IRequestHandlerSection>();
            settings.Setup(m => m.RequestHandler).Returns(requestHandler.Object);
            requestHandler.Setup(m => m.CharCollection).Returns(new IChar[0]);
            SetUmbracoSettings(settings.Object);
        }

        private static void SetUmbracoSettings(IUmbracoSettingsSection umbracoSettingsSection)
        {
            var type = typeof (UmbracoConfig);
            var method = type.GetMethod("SetUmbracoSettings", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(UmbracoConfig.For, new object[] {umbracoSettingsSection});
        }
    }
}
