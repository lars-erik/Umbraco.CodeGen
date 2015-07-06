using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Generators.Bcl;

namespace Umbraco.CodeGen.Tests.Generators
{
    [TestFixture]
    public class ClassGeneratorTests : TypeCodeGeneratorTestBase
    {
        private Info info;
        private CodeNamespace ns;

        [SetUp]
        public void SetUp()
        {
            Configuration = CodeGeneratorConfiguration.Create().MediaTypes;
            Generator = new ClassGenerator(
                Configuration,
                new EntityDescriptionGenerator(Configuration)
                );
            ContentType = new MediaType { Info = { Alias = "anEntity" } };
            info = ContentType.Info;
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
        public void Generate_Master_WhenNullOrEmpty_IsConfiguredBaseClass(string master)
        {
            info.Master = master;
            Configuration.BaseClass = "ConfiguredBase";
            Generate();
            Assert.AreEqual(Configuration.BaseClass, Type.BaseTypes[0].BaseType);
        }

        [Test]
        public void Generate_Master_WhenNotEmpty_IsBaseClassPascalCased()
        {
            const string expectedMaster = "ABaseClass";
            info.Master = expectedMaster;
            Generate();
            Assert.AreEqual(expectedMaster.PascalCase(), Type.BaseTypes[0].BaseType);
        }

        [Test]
        public void Generate_Composition_When_More_Than_Master_Adds_Interfaces()
        {
            const string expectedInterface = "IMixin";
            ContentType.Composition = new List<ContentType>
            {
                new ContentType
                {
                    Info = new Info
                    {
                        Alias = "Mixin"
                    }
                }
            };

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
