using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using umbraco.presentation.install.utills;

namespace Umbraco.CodeGen.Tests.Generators
{
    [TestFixture]
    public class PropertiesGeneratorTests
    {
        private MediaType contentType;
        private CodeTypeDeclaration type;

        [SetUp]
        public void SetUp()
        {
            contentType = new MediaType
            {
                GenericProperties = new List<GenericProperty>
                {
                    new GenericProperty{Alias="prop1"},
                    new GenericProperty{Alias="prop2"}
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
        public void Generate_CallsPropertyGeneratorsForAllProperties()
        {
            var spies = new[] {new SpyGenerator(), new SpyGenerator()};
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
