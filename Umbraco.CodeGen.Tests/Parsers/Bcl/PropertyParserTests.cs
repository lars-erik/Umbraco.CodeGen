using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;
using Umbraco.CodeGen.Parsers.Bcl;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests.Parsers.Bcl
{
    public class PropertyParserTests : PropertyParserTestBase
    {
        protected override ContentTypeCodeParserBase CreateParser()
        {
            return new PropertyParser(Configuration, DataTypeConfiguration);
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
            Assert.AreEqual("aProperty", Property.Alias);
        }

        [Test]
        public void Parse_Name_WhenPureProperty_IsSplitPascalCase()
        {
            ParseProperty(PureProperty);
            Assert.AreEqual("A Property", Property.Name);
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
            Assert.AreEqual("A description", Property.Description);
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
            Assert.AreEqual("", Property.Description);
        }

        [Test]
        public void Parse_Description_WhenAttributeMissing_IsNull()
        {
            ParseProperty(PureProperty);
            Assert.IsNull(Property.Description);
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
            Assert.AreEqual("2e6d3631-066e-44b8-aec4-96f09099b2b5", Property.Definition);
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
            Assert.AreEqual("2e6d3631-066e-44b8-aec4-96f09099b2b5", Property.Definition);
        }

        [Test]
        public void Parse_Definition_WhenKnownName_IsGuid()
        {
            const string code = @"
                public class AClass {
                    [DataType(""Richtext editor"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("ca90c950-0aff-4e72-b976-a30b1ac57dad", Property.Definition);
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
                Assert.AreEqual("0cc0eba1-9960-42c9-bf9b-60e150b429ae", Property.Definition);
            }
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Default datatype could not be found. Set a known datatype in TypeMappings.DefaultDefinitionId.")]
        public void Parse_Definition_WhenMissingOrUnknown_AndDefaultIsMissing_Throws()
        {
            CodeGenConfig = new CodeGeneratorConfiguration();
            CodeGenConfig.DefaultDefinitionId = "";
            Configuration = CodeGenConfig.MediaTypes;
            Parser = new PropertyParser(Configuration, DataTypeConfiguration);

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
            Assert.AreEqual("Umbraco.Textbox", Property.Type);
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
            Assert.AreEqual("A tab", Property.Tab);
        }

        [Test]
        public void Parse_Tab_WhenAttributeMissing_IsNull()
        {
            ParseProperty(PureProperty);
            Assert.IsNull(Property.Tab);
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
            Assert.IsTrue(Property.Mandatory);
        }

        [Test]
        public void Parse_Mandatory_WhenAttributeMissing_IsFalse()
        {
            ParseProperty(PureProperty);
            Assert.IsFalse(Property.Mandatory);
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
            Assert.AreEqual("[a-z]", Property.Validation);
        }

        [Test]
        public void Parse_Validation_WhenMissingAttribute_IsNull()
        {
            ParseProperty(PureProperty);
            Assert.IsNull(Property.Validation);
        }
    }
}
