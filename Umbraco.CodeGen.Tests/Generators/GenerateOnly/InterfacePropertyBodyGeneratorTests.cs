using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Tests.Generators.GenerateOnly
{
    [TestFixture]
    public class InterfacePropertyBodyGeneratorTests : TypeCodeGeneratorTestBase
    {
        [Test]
        public void Generate_Adds_Getter()
        {
            var property = new PropertyModel { Alias = "aProperty", ClrName = "AProperty" };
            var propNode = new CodeMemberProperty { Type = new CodeTypeReference("String") };

            var generator = new InterfacePropertyBodyGenerator(new GeneratorConfig());
            generator.Generate(propNode, property);

            Assert.IsTrue(propNode.HasGet);
            Assert.AreEqual(0, propNode.GetStatements.Count);
        }
    }
}
