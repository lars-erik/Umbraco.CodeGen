using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers.Annotated;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests.Parsers.Annotated
{
    [TestFixture]
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
        public void Parse_Description_WhenDescriptionArgument_IsValue()
        {
            const string code = @"
                public class AClass {
                    [GenericProperty(Description=""A description"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("A description", property.Description);
        }

        [Test]
        public void Parse_Description_WhenEmptyDescriptionArgument_IsEmpty()
        {
            const string code = @"
                public class AClass {
                    [GenericProperty(Description="""")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("", property.Description);
        }

        [Test]
        public void Parse_Description_WhenArgumentMissing_IsNull()
        {
            ParseProperty(PureProperty);
            Assert.IsNull(property.Description);
        }

        [Test]
        public void Parse_Definition_WhenKnown_IsValue()
        {
            const string code = @"
                public class AClass {
                    [GenericProperty(Definition=""2e6d3631-066e-44b8-aec4-96f09099b2b5"")]
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
                    [GenericProperty(Definition=""2E6D3631-066E-44B8-AEC4-96F09099B2B5"")]
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
                    [GenericProperty(Definition=""RTE"")]
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
                    [GenericProperty(Definition=""" + Guid.Empty + @""")]
                    public string AProperty {get;set;}
                }
            "};
            foreach (var snippet in code)
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
                    [GenericProperty(DataType=""Textstring"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("ec15c1e5-9d90-422a-aa52-4f7622c63bea", property.Type);
        }

        [Test]
        public void Parse_Tab_WhenTabArgument_IsArgumentValue()
        {
            const string code = @"
                public class AClass {
                    [GenericProperty(Tab=""A tab"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("A tab", property.Tab);
        }

        [Test]
        public void Parse_Tab_WhenArgumentMissing_IsNull()
        {
            ParseProperty(PureProperty);
            Assert.IsNull(property.Tab);
        }

        [Test]
        public void Parse_Mandatory_WhenMandatoryArgumentTrue_IsTrue()
        {
            const string code = @"
                public class AClass {
                    [GenericProperty(Mandatory=true)]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.IsTrue(property.Mandatory);
        }

        [Test]
        public void Parse_Mandatory_WhenMandatoryArgumentFalse_IsTrue()
        {
            const string code = @"
                public class AClass {
                    [GenericProperty(Mandatory=false)]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.IsFalse(property.Mandatory);
        }

        [Test]
        public void Parse_Mandatory_WhenArgumentMissing_IsFalse()
        {
            ParseProperty(PureProperty);
            Assert.IsFalse(property.Mandatory);
        }

        [Test]
        public void Parse_Validation_WhenArgument_IsValue()
        {
            const string code = @"
                public class AClass {
                    [GenericProperty(Validation=""[a-z]"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("[a-z]", property.Validation);
        }

        [Test]
        public void Parse_Validation_WhenMissingArgument_IsNull()
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
