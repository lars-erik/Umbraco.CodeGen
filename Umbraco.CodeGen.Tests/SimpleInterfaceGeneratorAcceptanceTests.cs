using System.Collections.Generic;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Factories;
using Umbraco.CodeGen.Generators;
using Umbraco.Core.Models;
using Umbraco.ModelsBuilder.Building;

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

        protected override void OnConfiguring(GeneratorConfig configuration, string contentTypeName)
        {
            configuration.BaseClass = typeof(IPublishedContent);
        }

        private static TypeModel CreateInterfaceType()
        {
            var expected = new TypeModel
            {
                Alias = "Mixin",
                Properties = 
                {
                    new PropertyModel
                    {
                        Alias = "mixinProp",
                        Name = "Mixin prop",
                        ClrName = "MixinProp",
                        ClrType = typeof(int)
                    }
                }
            };
            return expected;
        }
    }
}
