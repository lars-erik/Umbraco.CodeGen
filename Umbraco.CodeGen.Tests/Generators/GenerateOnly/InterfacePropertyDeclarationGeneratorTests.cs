using System;
using System.CodeDom;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators.Bcl;
using Umbraco.CodeGen.Generators.GenerateOnly;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests.Generators.GenerateOnly
{
    [TestFixture]
    public class InterfacePropertyDeclarationGeneratorTests : TypeCodeGeneratorTestBase
    {
        private CodeGeneratorConfiguration codeGenConfig;
        private GenericProperty property;
        private CodeMemberProperty codeProperty;

        [SetUp]
        public void SetUp()
        {
            codeGenConfig = CodeGeneratorConfiguration.Create();
            Configuration = codeGenConfig.DocumentTypes;
            Generator = new InterfacePropertyDeclarationGenerator(
                Configuration,
                TestDataTypeProvider.All,
                new EntityDescriptionGenerator(Configuration)
            );
            Candidate = codeProperty = new CodeMemberProperty();
            property = new GenericProperty { Alias = "aProperty" };
        }

        [Test]
        public void Generate_PropertyIsAbstract()
        {
            Generate();
            Assert.AreEqual(MemberAttributes.Abstract, codeProperty.Attributes);
        }

        [Test]
        public void Generate_Type_WhenNotConfigured_IsDefaultType()
        {
            Generate();
            Assert.AreEqual("String", codeProperty.Type.BaseType);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "TypeMappings/Default not set. Cannot guess default property type.")]
        public void Generate_Type_WhenNotConfigured_DefaultNotConfigured_Throws()
        {
            codeGenConfig.TypeMappings.DefaultType = null;
            Generate();
        }

        [Test]
        public void Generate_Type_WhenConfigured_IsConfiguredType()
        {
            codeGenConfig.TypeMappings = new TypeMappings(new[]{
                new TypeMapping("1413afcb-d19a-4173-8e9a-68288d2a73b8", "Int32")
            });
            property.Type = "1413AFCB-D19A-4173-8E9A-68288D2A73B8";
            Generate();
            Assert.AreEqual("Int32", codeProperty.Type.BaseType);
        }


        protected virtual void Generate()
        {
            Generator.Generate(Candidate, property);
        }
    }
}
