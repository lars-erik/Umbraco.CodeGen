using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests.Parsers
{
    public abstract class PropertyParserTestBase : ContentTypeCodeParserTestBase
    {
        protected GenericProperty Property;
        protected List<DataTypeDefinition> DataTypeConfiguration;
        protected CodeGeneratorConfiguration CodeGenConfig;
        protected const string PureProperty = @"
            public class AClass {
                public string AProperty {get;set;}
            }";

        [SetUp]
        public void SetUp()
        {
            Configuration = CodeGeneratorConfiguration.Create().MediaTypes;
            DataTypeConfiguration = TestDataTypeProvider.All;

            Parser = CreateParser();
        }

        protected abstract ContentTypeCodeParserBase CreateParser();

        protected void ParseProperty(string code)
        {
            const string propertyName = "AProperty";
            ContentType = new MediaType();
            var type = ParseType(code);
            var prop = type.Members.SingleOrDefault(m => m.Name == propertyName);
            Parser.Parse(prop, ContentType);
            Property = ContentType.GenericProperties.SingleOrDefault(p => p.Alias == propertyName.CamelCase());
        }
    }
}