using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Tests.Generators
{
    [TestFixture]
    public class PropertiesGeneratorTests
    {
        private TypeModel contentType;
        private CodeTypeDeclaration type;

        [SetUp]
        public void SetUp()
        {
            contentType = new TypeModel
            {
                Properties =
                {
                    new PropertyModel{Alias = "prop1", ClrName = "Prop1"},
                    new PropertyModel{Alias = "prop2", ClrName = "Prop2"}
                }
            };
            type = new CodeTypeDeclaration();
        }

        [Test]
        public void Generate_AddsPropertiesToType()
        {
            var generator = new PropertiesGenerator(null);
            generator.Generate(type, contentType);
            var properties = type.Members.OfType<CodeMemberProperty>().ToList();
            Assert.AreEqual(2, properties.Count());
        }

        [Test]
        public void Generate_Adds_Properties_From_Mixins()
        {
            contentType.MixinTypes.Add(
                new TypeModel
                {
                    ClrName = "Mixin",
                    Properties = 
                    {
                        new PropertyModel {Alias = "prop3", ClrName="Prop3"}
                    }
                }
            );

            var generator = new PropertiesGenerator(null);
            generator.Generate(type, contentType);
            var properties = type.Members.OfType<CodeMemberProperty>().ToList();
            Assert.AreEqual(3, properties.Count());
        }

        [Test]
        public void Generate_CallsPropertyGeneratorsForAllProperties()
        {
            var spies = new[] { new SpyGenerator(), new SpyGenerator() };
            var memberGenerators = spies.Cast<CodeGeneratorBase>().ToArray();
            var generator = new PropertiesGenerator(null, memberGenerators);
            generator.Generate(type, contentType);
            Assert.That(spies.All(spy => spy.Called));
            Assert.That(
                spies.All(spy =>
                    spy.CodeObjects.SequenceEqual(
                        type.Members.OfType<CodeMemberProperty>()
                    )
                )
            );
        }
    }
}
