using System;
using System.CodeDom;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators.Annotated;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests.Generators.Annotated
{
    public class PropertyInfoCodeGeneratorTests : AnnotationCodeGeneratorTestBase
    {
        private CodeGeneratorConfiguration codeGenConfig;
        private GenericProperty property;
        private CodeAttributeDeclaration attribute;

        [SetUp]
        public void SetUp()
        {
            codeGenConfig = CodeGeneratorConfiguration.Create();
            Configuration = codeGenConfig.MediaTypes;
            Generator = new PropertyInfoGenerator(
                Configuration, 
                TestDataTypeProvider.All
            );
            attribute = new CodeAttributeDeclaration();
            property = new GenericProperty{Alias="anEntity"};
        }

        [Test]
        public void Generate_Definition_WhenDefinitionExists_IsDefinitionName()
        {
            property.Definition = TestDataTypeProvider.Numeric.PropertyEditorAlias;
            Generate();
            Assert.AreEqual("Numeric", FindAttributeArgumentValue(attribute, "Definition"));
        }

        [Test]
        public void Generate_Definition_WhenDefinitionDoesNotExist_IsDefaultDefinition()
        {
            Generate();
            Assert.AreEqual("Textstring", FindAttributeArgumentValue(attribute, "Definition"));
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "TypeMappings/DefaultDefinitionId not set. Cannot guess default definition.")]
        public void Generate_Definition_WhenDefinitionDoesNotExist_DefaultNotConfigured_Throws()
        {
            codeGenConfig.TypeMappings.DefaultDefinitionId = null;
            Generate();
        }

        [Test]
        public void Generate_Tab_WhenSet_TabArgumentHasValue()
        {
            property.Tab = "A tab";
            Generate();
            Assert.AreEqual("A tab", FindAttributeArgumentValue(attribute, "Tab"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_Tab_WhenEmpty_OmitsTabArgument(string value)
        {
            property.Tab = value;
            Generate();
            Assert.IsNull(FindAttributeArgument(attribute, "Tab"));
        }

        [Test]
        public void Generate_Mandatory_WhenTrue_MandatoryArgumentIsSet()
        {
            property.Mandatory = true;
            Generate();
            Assert.IsTrue((bool)FindAttributeArgumentValue(attribute, "Mandatory"));
        }

        [Test]
        public void Generate_Mandatory_WhenFalse_OmitsMandatoryArgument()
        {
            Generate();
            Assert.IsNull(FindAttributeArgument(attribute, "Mandatory"));
        }

        [Test]
        public void Generate_Validation_HasValue_ValidationArgumentHasValue()
        {
            property.Validation = "[a-z]";
            Generate();
            Assert.AreEqual("[a-z]", FindAttributeArgumentValue(attribute, "Validation"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_Validation_IsNullOrEmpty_OmitsValidationArgument(string value)
        {
            property.Validation = value;
            Generate();
            Assert.IsNull(FindAttributeArgument(attribute, "Validation"));
        }

        protected virtual void Generate()
        {
            Generator.Generate(attribute, property);
        }
    }
}
