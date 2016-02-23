using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Definitions.ModelsBuilder;
using Umbraco.CodeGen.Generators.GenerateOnly;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Tests.Generators.GenerateOnly
{
    [TestFixture]
    public class InterfaceNameGeneratorTests : TypeCodeGeneratorTestBase
    {
        private TypeModel documentType;

        [SetUp]
        public void SetUp()
        {
            Configuration = CodeGeneratorConfiguration.Create().DocumentTypes;
            Candidate = Type = new CodeTypeDeclaration();
            Generator = new InterfaceNameGenerator(Configuration);
            documentType = new TypeModel { Alias = "aMixin", ClrName = "AMixin" };
        }

        [Test]
        public void Generate_Alias_Pascal_Cases_Name_And_Prefixes_With_I()
        {
            Generate();
            Assert.AreEqual("I" + documentType.ClrName, Candidate.Name);
        }

        protected virtual void Generate()
        {
            Generator.Generate(Candidate, documentType);
        }
    }
}
