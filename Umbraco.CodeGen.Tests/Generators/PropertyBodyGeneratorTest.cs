using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests.Generators
{
    [TestFixture]
    public class PropertyBodyGeneratorTest : PropertyBodyGeneratorTestBase
    {
        [Test]
        public void Generate_Body_GetsContentPropertyValueOfType()
        {
            var body = GeneratePropertyAndGetBodyText();

            Assert.AreEqual(
                "return Content.GetPropertyValue<String>(\"aProperty\");",
                body
                );
        }

        protected override CodeGeneratorBase CreateGenerator()
        {
            return new PropertyBodyGenerator(new GeneratorConfig());
        }
    }
}
