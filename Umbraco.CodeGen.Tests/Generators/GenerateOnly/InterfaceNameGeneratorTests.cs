using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators.GenerateOnly;

namespace Umbraco.CodeGen.Tests.Generators.GenerateOnly
{
    [TestFixture]
    public class InterfaceNameGeneratorTests : TypeCodeGeneratorTestBase
    {
        protected EntityDescription EntityDescription;
        private DocumentType documentType;

        [SetUp]
        public void SetUp()
        {
            Configuration = CodeGeneratorConfiguration.Create().DocumentTypes;
            Candidate = Type = new CodeTypeDeclaration();
            Generator = new InterfaceNameGenerator(Configuration);
            documentType = new DocumentType { Info = { Alias = "aMixin" } };
            EntityDescription = documentType.Info;
        }

        [Test]
        public void Generate_Alias_Pascal_Cases_Name_And_Prefixes_With_I()
        {
            Generate();
            Assert.AreEqual("I" + EntityDescription.Alias.PascalCase(), Candidate.Name);
        }

        protected virtual void Generate()
        {
            Generator.Generate(Candidate, EntityDescription);
        }
    }
}
