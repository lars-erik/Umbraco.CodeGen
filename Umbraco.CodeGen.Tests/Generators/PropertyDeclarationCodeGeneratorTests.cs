using System;
using System.CodeDom;
using System.Collections.Generic;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests.Generators
{
    public class PropertyDeclarationCodeGeneratorTests : EntityDescriptionGeneratorTests
    {
        private CodeGeneratorConfiguration codeGenConfig;
        private GenericProperty property;
        private CodeMemberProperty codeProperty;

        [SetUp]
        public void SetUp()
        {
            codeGenConfig = new CodeGeneratorConfiguration();
            Configuration = codeGenConfig.MediaTypes;
            Generator = new PropertyDeclarationGenerator(
                Configuration, 
                TestDataTypeProvider.All,
                new EntityDescriptionGenerator(Configuration)
            );
            Candidate = codeProperty = new CodeMemberProperty();
            EntityDescription = property = new GenericProperty{Alias="anEntity"};
        }

        [Test]
        public void Generate_PropertyIsPublic()
        {
            Generate();
            Assert.AreEqual(MemberAttributes.Public, codeProperty.Attributes);
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
            codeGenConfig.DefaultTypeMapping = null;
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

        [Test]
        public void Generate_Definition_WhenDefinitionExists_IsDefinitionName()
        {
            property.Definition = TestDataTypeProvider.Numeric.DefinitionId;
            Generate();
            Assert.AreEqual("Numeric", FindAttributeValue("DataType"));
        }

        [Test]
        public void Generate_Definition_WhenDefinitionDoesNotExist_IsDefaultDefinition()
        {
            Generate();
            Assert.AreEqual("Textstring", FindAttributeValue("DataType"));
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "TypeMappings/DefaultDefinitionId not set. Cannot guess default definition.")]
        public void Generate_Definition_WhenDefinitionDoesNotExist_DefaultNotConfigured_Throws()
        {
            codeGenConfig.DefaultDefinitionId = null;
            Generate();
        }

        [Test]
        public void Generate_Tab_WhenSet_CategoryAttributeHasValue()
        {
            property.Tab = "A tab";
            Generate();
            Assert.AreEqual("A tab", FindAttributeValue("Category"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_Tab_WhenEmpty_OmitsCategoryAttribute(string value)
        {
            property.Tab = value;
            Generate();
            Assert.IsNull(FindAttribute("Category"));
        }

        [Test]
        public void Generate_Mandatory_WhenTrue_RequiredAttributeIsSet()
        {
            property.Mandatory = true;
            Generate();
            Assert.IsNotNull(FindAttribute("Required"));
        }

        [Test]
        public void Generate_Mandatory_WhenFalse_OmitsRequiredAttribute()
        {
            Generate();
            Assert.IsNull(FindAttribute("Required"));
        }

        [Test]
        public void Generate_Validation_HasValue_RegularExpressionAttributeHasValue()
        {
            property.Validation = "[a-z]";
            Generate();
            Assert.AreEqual("[a-z]", FindAttributeValue("RegularExpression"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Generate_Validation_IsNullOrEmpty_OmitsRegularExpressionAttribute(string value)
        {
            property.Validation = value;
            Generate();
            Assert.IsNull(FindAttribute("RegularExpression"));
        }
    }
}
