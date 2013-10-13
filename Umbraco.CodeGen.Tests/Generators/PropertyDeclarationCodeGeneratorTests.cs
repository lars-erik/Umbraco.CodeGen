using System;
using System.CodeDom;
using System.Collections.Generic;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;

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
            codeGenConfig = new CodeGeneratorConfiguration
            {
                DefaultTypeMapping = "String",
                DefaultDefinitionId = "Textstring"
            };
            Configuration = new ContentTypeConfiguration(codeGenConfig);
            var dataTypes = new List<DataTypeDefinition>
            {
                new DataTypeDefinition("Textstring", "e4431ff4-89d6-4656-8aea-02daed62074f", "4bb6058a-1199-4a8d-90ec-1d58b1c9bcbf"),
                new DataTypeDefinition("Numeric", "edc83fe4-c7d5-4c4d-a067-2992a820edbd", "109f1923-3e38-46b0-8fd0-bca30bfb9e51")
            };
            Generator = new PropertyDeclarationGenerator(
                Configuration, 
                dataTypes,
                new EntityDescriptionGenerator(Configuration)
            );
            Candidate = codeProperty = new CodeMemberProperty();
            EntityDescription = property = new GenericProperty{Alias="anEntity"};
        }

        [Test]
        public void Generate_Type_WhenNotConfigured_IsDefaultType()
        {
            codeGenConfig.DefaultTypeMapping = "String";
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
            codeGenConfig.TypeMappings= new Dictionary<string, string>{{"edc83fe4-c7d5-4c4d-a067-2992a820edbd", "Int32"}};
            property.Type = "EDC83FE4-C7D5-4C4D-A067-2992A820EDBD";
            Generate();
            Assert.AreEqual("Int32", codeProperty.Type.BaseType);
        }

        [Test]
        public void Generate_Definition_WhenDefinitionExists_IsDefinitionName()
        {
            property.Definition = "109f1923-3e38-46b0-8fd0-bca30bfb9e51";
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
