using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.Core;
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
        private IContentType umbracoContentType;

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
                                PropertyEditorAlias = "Umbraco.Number",
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
                        PropertyEditorAlias = "Umbraco.Richtext"
                    },
                    new GenericProperty
                    {
                        Alias = "anotherProperty",
                        Definition = null,
                        Description = "Another description",
                        Name = "Another property",
                        Tab = "A tab",
                        PropertyEditorAlias = "Umbraco.Richtext"
                    },
                    new GenericProperty
                    {
                        Alias = "tablessProperty",
                        Definition = null,
                        Name = "Tabless property",
                        PropertyEditorAlias = "Umbraco.Number"
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
            //SetUmbracoSettings(settings.Object);
        }

        private static IContentType CreateUmbracoContentType()
        {
            var umbracoContentType = new FakeContentType()
            {
                Id = 2,
                Alias = "SomeDocumentType",
                Name = "Some document type",
                AllowedAsRoot = true,
                AllowedContentTypes = new[] {new ContentTypeSort(new Lazy<int>(() => 3), 1, "SomeOtherDocType"),},
                AllowedTemplates = new[] {new FakeTemplate("ATemplate") { Id = 1 }, new FakeTemplate("AnotherTemplate") { Id = 2 },},
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

    internal class FakeTemplate : ITemplate
    {
        public FakeTemplate(string name)
        {
            ((ITemplate) this).Name = name;
        }

        public object DeepClone()
        {
            throw new NotImplementedException();
        }

        public int Id { get; set; }
        public Guid Key { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool HasIdentity { get; private set; }
        public void ResetOriginalPath()
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        public string Name { get; set; }
        public string Alias { get; set; }
        public bool IsMasterTemplate { get; private set; }
        public string MasterTemplateAlias { get; private set; }

        public RenderingEngine GetTypeOfRenderingEngine()
        {
            throw new NotImplementedException();
        }

        public void SetMasterTemplate(ITemplate masterTemplate)
        {
            throw new NotImplementedException();
        }

        public string Path { get; set; }
        public string OriginalPath { get; private set; }
        public string Content { get; set; }
        public string VirtualPath { get; set; }
        public bool IsDirty()
        {
            throw new NotImplementedException();
        }

        public bool IsPropertyDirty(string propName)
        {
            throw new NotImplementedException();
        }

        public void ResetDirtyProperties()
        {
            throw new NotImplementedException();
        }

        public bool WasDirty()
        {
            throw new NotImplementedException();
        }

        public bool WasPropertyDirty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public void ForgetPreviouslyDirtyProperties()
        {
            throw new NotImplementedException();
        }

        public void ResetDirtyProperties(bool rememberPreviouslyChangedProperties)
        {
            throw new NotImplementedException();
        }
    }

    internal class FakeContentType : IContentType
    {
        public object DeepClone()
        {
            throw new NotImplementedException();
        }

        public int Id { get; set; }
        public Guid Key { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool HasIdentity { get; private set; }
        public bool IsDirty()
        {
            throw new NotImplementedException();
        }

        public bool IsPropertyDirty(string propName)
        {
            throw new NotImplementedException();
        }

        public void ResetDirtyProperties()
        {
            throw new NotImplementedException();
        }

        public bool WasDirty()
        {
            throw new NotImplementedException();
        }

        public bool WasPropertyDirty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public void ForgetPreviouslyDirtyProperties()
        {
            throw new NotImplementedException();
        }

        public void ResetDirtyProperties(bool rememberPreviouslyChangedProperties)
        {
            throw new NotImplementedException();
        }

        public int CreatorId { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public string Path { get; set; }
        public int SortOrder { get; set; }
        public bool Trashed { get; private set; }
        public IDictionary<string, object> AdditionalData { get; private set; }
        public void RemovePropertyType(string propertyTypeAlias)
        {
            throw new NotImplementedException();
        }

        public void RemovePropertyGroup(string propertyGroupName)
        {
            throw new NotImplementedException();
        }

        public void SetLazyParentId(Lazy<int> id)
        {
            throw new NotImplementedException();
        }

        public bool PropertyTypeExists(string propertyTypeAlias)
        {
            throw new NotImplementedException();
        }

        public bool AddPropertyType(PropertyType propertyType, string propertyGroupName)
        {
            throw new NotImplementedException();
        }

        public bool AddPropertyType(PropertyType propertyType)
        {
            throw new NotImplementedException();
        }

        public bool AddPropertyGroup(string groupName)
        {
            throw new NotImplementedException();
        }

        public bool MovePropertyType(string propertyTypeAlias, string propertyGroupName)
        {
            throw new NotImplementedException();
        }

        public string Alias { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Thumbnail { get; set; }
        public bool AllowedAsRoot { get; set; }
        public bool IsContainer { get; set; }
        public IEnumerable<ContentTypeSort> AllowedContentTypes { get; set; }
        public PropertyGroupCollection PropertyGroups { get; set; }
        public IEnumerable<PropertyType> PropertyTypes { get; private set; }
        public bool AddContentType(IContentTypeComposition contentType)
        {
            throw new NotImplementedException();
        }

        public bool RemoveContentType(string alias)
        {
            throw new NotImplementedException();
        }

        public bool ContentTypeCompositionExists(string alias)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> CompositionAliases()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> CompositionIds()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IContentTypeComposition> ContentTypeComposition { get; private set; }
        public IEnumerable<PropertyGroup> CompositionPropertyGroups { get; private set; }
        public IEnumerable<PropertyType> CompositionPropertyTypes { get; private set; }
        public void SetDefaultTemplate(ITemplate template)
        {
            throw new NotImplementedException();
        }

        public bool RemoveTemplate(ITemplate template)
        {
            throw new NotImplementedException();
        }

        public IContentType DeepCloneWithResetIdentities(string newAlias)
        {
            throw new NotImplementedException();
        }

        public ITemplate DefaultTemplate { get; private set; }
        public IEnumerable<ITemplate> AllowedTemplates { get; set; }
    }
}
