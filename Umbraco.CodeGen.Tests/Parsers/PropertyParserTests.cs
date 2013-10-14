using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;
using Umbraco.CodeGen.Tests.Helpers;

namespace Umbraco.CodeGen.Tests.Parsers
{
    public class PropertyParserTests : ContentTypeCodeParserTestBase
    {
        private const string PureProperty = @"
            public class AClass {
                public string AProperty {get;set;}
            }";
        private GenericProperty property;
        private List<DataTypeDefinition> dataTypeConfiguration;
        private CodeGeneratorConfiguration codeGenConfig;

        [SetUp]
        public void SetUp()
        {
            Configuration = new CodeGeneratorConfiguration().MediaTypes;
            dataTypeConfiguration = TestDataTypeProvider.All;

            Parser = new PropertyParser(Configuration, dataTypeConfiguration);
        }

        [Test]
        public void Parse_PropertyNode_AddsGenericProperty()
        {
            ParseProperty(PureProperty);
            Assert.AreEqual(1, ContentType.GenericProperties.Count);
        }

        [Test]
        public void Parse_Alias_IsPropertyNameInCamelCase()
        {
            ParseProperty(PureProperty);
            Assert.AreEqual("aProperty", property.Alias);
        }

        [Test]
        public void Parse_Name_WhenPureProperty_IsSplitPascalCase()
        {
            ParseProperty(PureProperty);
            Assert.AreEqual("A Property", property.Name);
        }

        [Test]
        public void Parse_Description_WhenDescriptionAttribute_IsValue()
        {
            const string code = @"
                public class AClass {
                    [Description(""A description"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("A description", property.Description);
        }

        [Test]
        public void Parse_Description_WhenEmptyDescriptionAttribute_IsEmpty()
        {
            const string code = @"
                public class AClass {
                    [Description("""")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("", property.Description);
        }

        [Test]
        public void Parse_Description_WhenAttributeMissing_IsNull()
        {
            ParseProperty(PureProperty);
            Assert.IsNull(property.Description);
        }

        [Test]
        public void Parse_Definition_WhenKnown_IsValue()
        {
            const string code = @"
                public class AClass {
                    [DataType(""2e6d3631-066e-44b8-aec4-96f09099b2b5"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("2e6d3631-066e-44b8-aec4-96f09099b2b5", property.Definition);
        }

        [Test]
        public void Parse_Definition_WhenKnownUpperCase_IsValue()
        {
            const string code = @"
                public class AClass {
                    [DataType(""2E6D3631-066E-44B8-AEC4-96F09099B2B5"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("2e6d3631-066e-44b8-aec4-96f09099b2b5", property.Definition);
        }

        [Test]
        public void Parse_Definition_WhenKnownName_IsGuid()
        {
            const string code = @"
                public class AClass {
                    [DataType(""RTE"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("ca90c950-0aff-4e72-b976-a30b1ac57dad", property.Definition);
        }

        [Test]
        public void Parse_Definition_WhenMissingOrUnknown_IsDefault()
        {
            var code = new[]{
                PureProperty,
                @"public class AClass {
                    [DataType(""" + Guid.Empty + @""")]
                    public string AProperty {get;set;}
                }
            "};
            foreach(var snippet in code)
            {
                ParseProperty(snippet);
                Assert.AreEqual("0cc0eba1-9960-42c9-bf9b-60e150b429ae", property.Definition);
            }
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Default datatype could not be found. Set a known datatype in TypeMappings.DefaultDefinitionId.")]
        public void Parse_Definition_WhenMissingOrUnknown_AndDefaultIsMissing_Throws()
        {
            codeGenConfig = new CodeGeneratorConfiguration();
            codeGenConfig.DefaultDefinitionId = "";
            Configuration = codeGenConfig.MediaTypes;
            Parser = new PropertyParser(Configuration, dataTypeConfiguration);

            ParseProperty(PureProperty);
        }

        [Test]
        public void Parse_Type_IsTypeOfDefinition()
        {
            const string code = @"
                public class AClass {
                    [DataType(""Textstring"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("ec15c1e5-9d90-422a-aa52-4f7622c63bea", property.Type);
        }

        [Test]
        public void Parse_Tab_WhenCategoryAttribute_IsAttributeValue()
        {
            const string code = @"
                public class AClass {
                    [Category(""A tab"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("A tab", property.Tab);
        }

        [Test]
        public void Parse_Tab_WhenAttributeMissing_IsNull()
        {
            ParseProperty(PureProperty);
            Assert.IsNull(property.Tab);
        }

        [Test]
        public void Parse_Mandatory_WhenRequiredAttribute_IsTrue()
        {
            const string code = @"
                public class AClass {
                    [Required]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.IsTrue(property.Mandatory);
        }

        [Test]
        public void Parse_Mandatory_WhenAttributeMissing_IsFalse()
        {
            ParseProperty(PureProperty);
            Assert.IsFalse(property.Mandatory);
        }

        [Test]
        public void Parse_Validation_WhenRegexAttribute_IsValue()
        {
            const string code = @"
                public class AClass {
                    [RegularExpression(""[a-z]"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("[a-z]", property.Validation);
        }

        [Test]
        public void Parse_Validation_WhenMissingAttribute_IsNull()
        {
            ParseProperty(PureProperty);
            Assert.IsNull(property.Validation);
        }

        private void ParseProperty(string code)
        {
            const string propertyName = "AProperty";
            ContentType = new MediaType();
            var type = ParseType(code);
            var prop = type.Members.SingleOrDefault(m => m.Name == propertyName);
            Parser.Parse(prop, ContentType);
            property = ContentType.GenericProperties.SingleOrDefault(p => p.Alias == propertyName.CamelCase());
        }
    }
}
