using System.CodeDom;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests.Generators
{
    [TestFixture]
    public class ClassGeneratorTests : EntityDescriptionGeneratorTests
    {
        private Info info;
        private CodeNamespace ns;

        [SetUp]
        public void SetUp()
        {
            Configuration = new ContentTypeConfiguration(null);
            Generator = new ClassGenerator(
                Configuration,
                new EntityDescriptionGenerator(Configuration)
                );
            ContentType = new MediaType { Info = { Alias = "anEntity" } };
            EntityDescription = info = ContentType.Info;
            ns = new CodeNamespace("ANamespace");
        }

        [Test]
        public void Generate_AddsTypeToNamespace()
        {
            Generate();
            Assert.IsNotNull(Type);
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
            const string expectedMaster = "aBaseClass";
            info.Master = expectedMaster;
            Generate();
            Assert.AreEqual(expectedMaster.PascalCase(), Type.BaseTypes[0].BaseType);
        }

        [Test]
        public void CallsMemberGenerators()
        {
            var spies = new[] { new SpyGenerator(), new SpyGenerator() };
            var memberGenerators = spies.Cast<CodeGeneratorBase>().ToArray();
            Generator = new ClassGenerator(Configuration, new EntityDescriptionGenerator(Configuration), memberGenerators);
            Generate();
            Assert.That(spies.All(s => s.Called));
        }

        protected override void Generate()
        {
            Generator.Generate(ns, ContentType);
            Candidate = Type = ns.Types.Cast<CodeTypeDeclaration>().Single();
        }
    }
}
