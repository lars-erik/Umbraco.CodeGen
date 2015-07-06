using System.Collections.Generic;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests
{
    [TestFixture]
    public class GenerateOnlyInterfaceAcceptanceTests : CodeGeneratorAcceptanceTestBase
    {
        [Test]
        public void BuildCode_Generates_Interface()
        {
            TestBuildCode("GenerateOnlyInterface", CreateInterfaceType(), "DocumentType");
        }

        protected override CodeGeneratorFactory CreateGeneratorFactory()
        {
            return new InterfaceGeneratorFactory();
        }

        protected override void OnConfiguring(CodeGeneratorConfiguration configuration, string contentTypeName)
        {
            var config = configuration.Get(contentTypeName);
            config.BaseClass = "IPublishedContent";
        }

        private static DocumentType CreateInterfaceType()
        {
            var expected = new DocumentType
            {
                Info = new DocumentTypeInfo()
                {
                    Alias = "Mixin"
                },
                Tabs = new List<Tab> { new Tab { Caption = "Mixin tab" } },
                GenericProperties = new List<GenericProperty>
                {
                    new GenericProperty
                    {
                        Alias = "mixinProp",
                        Name = "Mixin prop",
                        Type = "Umbraco.Integer",
                        Tab = "Mixin tab"
                    }
                }
            };
            return expected;
        }
    }
}
