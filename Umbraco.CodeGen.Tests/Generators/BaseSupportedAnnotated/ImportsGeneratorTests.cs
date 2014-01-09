using System.CodeDom;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Generators.BaseSupportedAnnotated;

namespace Umbraco.CodeGen.Tests.Generators.BaseSupportedAnnotated
{
    [TestFixture]
    class ImportsGeneratorTests
    {
        [Test]
        public void Generate_AddsImports()
        {
            var ns = new CodeNamespace("ANamespace");
            var generator = new ImportsGenerator(null);
            generator.Generate(ns, null);
            Assert.That(
                new[]
                {
                    "System",
                    "Umbraco.CodeGen.Annotations"
                }.SequenceEqual(
                    ns.Imports.Cast<CodeNamespaceImport>()
                        .Select(import => import.Namespace)
                ));
        }
    }
}
