using System.Collections.Generic;
using System.Web;
using Umbraco.CodeGen.Definitions;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Tests.TestHelpers
{
    static internal class TestFactory
    {
        public static TypeModel CreateExpectedDocumentType()
        {
            return new TypeModel
            {
                Name = "Some Document Type",
                Alias = "SomeDocumentType",
                BaseType = null,
                ClrName = "SomeDocumentType",
                Description = "A description of some document type",
                DeclaringInterfaces = {},
                HasBase = false,
                HasCtor = true,
                HasImplement = false,
                Id = 1,
                ImplementingInterfaces = {},
                IsContentIgnored = false,
                IsMixin = false,
                IsParent = false,
                IsRenamed = false,
                ItemType = TypeModel.ItemTypes.Content,
                MixinTypes =
                {
                    new TypeModel
                    {
                        Name = "Mixin",
                        Alias = "mixin",
                        ClrName = "Mixin",
                        Properties = { new PropertyModel
                        {
                            Alias = "mixinProp",
                            ClrName = "MixinProp",
                            ClrType = typeof(int),
                            Name = "Mixin prop"
                        } }
                    }
                },
                ParentId = -1,
                Properties =
                {
                    new PropertyModel
                    {
                        Name = "Some Property",
                        Alias = "someProperty",
                        ClrName = "SomeProperty",
                        ClrType = typeof(IHtmlString),
                        Description = "A description",
                        IsIgnored = false
                    },
                    new PropertyModel
                    {
                        Name = "Another Property",
                        Alias = "anotherProperty",
                        ClrName = "AnotherProperty",
                        ClrType = typeof(IHtmlString),
                        Description = "Another description"
                    },
                    new PropertyModel
                    {
                        Name = "Tabless Property",
                        Alias = "tablessProperty",
                        ClrName = "TablessProperty",
                        ClrType = typeof(int),
                        Description = null
                    }

                },
                StaticMixinMethods = {}
            };
        }

        public static MediaType CreateExpectedMediaType()
        {
            return new MediaType
            {
                Info = new Info
                {
                    Name = "Inherited Media Folder",
                    Alias = "InheritedMediaFolder",
                    Icon = "folder.gif",
                    Thumbnail = "folder.png",
                    AllowAtRoot = true,
                    Master = "Folder"
                },
                Structure = new List<string>
                {
                    "Folder",
                    "Image",
                    "File",
                    "InheritedMediaFolder"
                },
                GenericProperties = new List<GenericProperty>
                {
                    new GenericProperty
                    {
                        Name = "Lets Have A Property",
                        Alias = "letsHaveAProperty",
                        PropertyEditorAlias = "76FD82CF-8AC3-4FF7-9E88-D0A8539AE109",
                        Definition = "0cc0eba1-9960-42c9-bf9b-60e150b429ae",
                        Tab = "A tab"
                    },
                    new GenericProperty
                    {
                        Name = "And A Tabless Property",
                        Alias = "andATablessProperty",
                        PropertyEditorAlias = "76FD82CF-8AC3-4FF7-9E88-D0A8539AE109",
                        Definition = "0cc0eba1-9960-42c9-bf9b-60e150b429ae",
                    }
                },
                Tabs = new List<Tab>
                {
                    new Tab
                    {
                        Id = 0,
                        Caption = "A tab"
                    }
                }
            };
        }
    }
}