using System.CodeDom;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Generators.GenerateOnly;

namespace Umbraco.CodeGen.Tests.Generators.GenerateOnly
{
    [TestFixture]
    public class InterfaceGeneratorTests : TypeCodeGeneratorTestBase
    {
        private CodeNamespace ns;

        [SetUp]
        public void SetUp()
        {
            Configuration = CodeGeneratorConfiguration.Create().DocumentTypes;
            Generator = new InterfaceGenerator(
                Configuration
                );
            ContentType = new DocumentType { Info = { Alias = "Mixin" } };
            ns = new CodeNamespace("ANamespace");
        }

        [Test]
        public void Generate_AddsTypeToNamespace()
        {
            Generate();
            Assert.IsNotNull(Type);
        }

        [Test]
        public void Derives_From_IPublishedContent()
        {
            Generate();
            Assert.AreEqual("IPublishedContent", Type.BaseTypes[0].BaseType);
        }

        [Test]
        public void Is_Interface()
        {
            Generate();
            Assert.IsTrue(Type.IsInterface);
        }

        [Test]
        public void Is_Partial()
        {
            Generate();
            Assert.IsTrue(Type.IsPartial);
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
