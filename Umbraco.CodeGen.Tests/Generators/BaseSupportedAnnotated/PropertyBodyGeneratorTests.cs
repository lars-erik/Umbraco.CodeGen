using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators;
using PropertyBodyGenerator = Umbraco.CodeGen.Generators.BaseSupportedAnnotated.PropertyBodyGenerator;

namespace Umbraco.CodeGen.Tests.Generators.BaseSupportedAnnotated
{
    [TestFixture]
    public class PropertyBodyGeneratorTests : PropertyBodyGeneratorTestBase
    {
        [Test]
        public void Generate_Body_GetsValueFromBaseMethodGetValue()
        {
            var body = GeneratePropertyAndGetBodyText();

            Assert.AreEqual(
                "return GetValue<String>(\"aProperty\");",
                body
                );
        }

        protected override CodeGeneratorBase CreateGenerator()
        {
            return new PropertyBodyGenerator(CodeGeneratorConfiguration.Create().MediaTypes);
        }
    }
}
