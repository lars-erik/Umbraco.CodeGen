using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Tests.TestHelpers;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Tests.Generators
{
    [TestFixture]
    public class ClassGeneratorTests : TypeCodeGeneratorTestBase
    {
        private CodeNamespace ns;

        [SetUp]
        public void SetUp()
        {
            Configuration = TestFactory.TestConfig();
            Generator = new ClassGenerator(
                Configuration
                );
            ContentType = new TypeModel { Alias = "anEntity", ClrName = "AnEntity" };
            ns = new CodeNamespace("ANamespace");
        }

        [Test]
        public void Generate_AddsTypeToNamespace()
        {
            Generate();
            Assert.IsNotNull(Type);
        }

        [Test]
        public void Generate_ClassIsPartial()
        {
            Generate();
            Assert.IsTrue(Type.IsPartial);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_Master_WhenNullOrEmpty_IsConfiguredBaseClass(string baseClassName)
        {
            ContentType.BaseType = new TypeModel {ClrName = baseClassName};
            Configuration.BaseClass = typeof(object);
            Generate();
            Assert.AreEqual(Configuration.BaseClass.FullName, Type.BaseTypes[0].BaseType);
        }

        [Test]
        public void Generate_Master_WhenNotEmpty_IsBaseClassPascalCased()
        {
            const string expectedBaseClrName = "ABaseClass";
            ContentType.HasBase = true;
            ContentType.BaseType = new TypeModel {ClrName = expectedBaseClrName};
            Generate();
            Assert.AreEqual(expectedBaseClrName, Type.BaseTypes[0].BaseType);
        }

        [Test]
        public void Generate_Composition_When_More_Than_Master_Adds_Interfaces()
        {
            const string expectedInterface = "IMixin";
            ContentType.MixinTypes.Add(
                new TypeModel
                {
                    ClrName = "Mixin"
                }
            );

            Generate();

            Assert.AreEqual(expectedInterface, Type.BaseTypes[1].BaseType);
        }

        [Test]
        public void Generate_When_Is_Mixin_Adds_Own_Interface()
        {
            const string expectedInterface = "IAnEntity";
            ContentType.IsMixin = true;

            Generate();

            Assert.AreEqual(expectedInterface, Type.BaseTypes[1].BaseType);
        }

        [Test]
        public void CallsMemberGenerators()
        {
            var spies = new[] { new SpyGenerator(), new SpyGenerator() };
            var memberGenerators = spies.Cast<CodeGeneratorBase>().ToArray();
            Generator = new ClassGenerator(Configuration, memberGenerators);
            Generate();
            Assert.That(spies.All(s => s.Called));
        }

        protected void Generate()
        {
            Generator.Generate(ns, ContentType);
            Candidate = Type = ns.Types.Cast<CodeTypeDeclaration>().Single();
        }
    }
}
