using System;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Parsers;
using Umbraco.CodeGen.Parsers.Annotated;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests.Parsers.Annotated
{
    [TestFixture]
    public class PropertyParserTests : PropertyParserTestBase
    {
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
        public void Parse_Description_WhenDescriptionArgument_IsValue()
        {
            const string code = @"
                public class AClass {
                    [GenericProperty(Description=""A description"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual("A description", Property.Description);
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
            Assert.AreEqual("", Property.Description);
        }

        [Test]
        public void Parse_Description_WhenArgumentMissing_IsNull()
        {
            ParseProperty(PureProperty);
            Assert.IsNull(Property.Description);
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
            Assert.AreEqual("2e6d3631-066e-44b8-aec4-96f09099b2b5", Property.Definition);
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
            Assert.AreEqual("2e6d3631-066e-44b8-aec4-96f09099b2b5", Property.Definition);
        }

        [Test]
        public void Parse_Definition_WhenKnownName_IsDefinitionId()
        {
            const string code = @"
                public class AClass {
                    [GenericProperty(Definition=""Richtext editor"")]
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
                    [GenericProperty(Definition=""" + Guid.Empty + @""")]
                    public string AProperty {get;set;}
                }
            "};
            foreach (var snippet in code)
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
                    [GenericProperty(Definition=""Richtext editor"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual(TestDataTypeProvider.Richtexteditor.DataTypeId, Property.Type);
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
            Assert.AreEqual("A tab", Property.Tab);
        }

        [Test]
        public void Parse_Tab_WhenArgumentMissing_IsNull()
        {
            ParseProperty(PureProperty);
            Assert.IsNull(Property.Tab);
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
            Assert.IsTrue(Property.Mandatory);
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
            Assert.IsFalse(Property.Mandatory);
        }

        [Test]
        public void Parse_Mandatory_WhenArgumentMissing_IsFalse()
        {
            ParseProperty(PureProperty);
            Assert.IsFalse(Property.Mandatory);
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
            Assert.AreEqual("[a-z]", Property.Validation);
        }

        [Test]
        public void Parse_Validation_WhenMissingArgument_IsNull()
        {
            ParseProperty(PureProperty);
            Assert.IsNull(Property.Validation);
        }

        protected override ContentTypeCodeParserBase CreateParser()
        {
            return new PropertyParser(Configuration, DataTypeConfiguration);
        }
    }
}
