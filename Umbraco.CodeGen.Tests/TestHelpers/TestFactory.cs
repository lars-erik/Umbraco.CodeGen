using System.Collections.Generic;
using System.Web;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Tests.TestHelpers
{
    static internal class TestFactory
    {
        public static GeneratorConfig TestConfig()
        {
            return new GeneratorConfig
            {
                BaseClass = typeof (PublishedContentModel),
                Namespace = "Umbraco.CodeGen.Models"
            };
        }

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
    }
}