using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers.Annotated
{
    public class PropertyParser : ContentTypeCodeParserBase
    {
        protected readonly IEnumerable<DataTypeDefinition> DataTypes;
        protected readonly DataTypeDefinition DefaultDataType;

        public PropertyParser(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
            : base(configuration)
        {
            this.DataTypes = dataTypes;
            DefaultDataType = FindDataTypeDefinition(configuration.DefaultDefinitionId);
        }

        public override void Parse(AstNode node, ContentType contentType)
        {
            var propNode = (PropertyDeclaration)node;
            var attribute = FindAttribute(propNode.Attributes, "GenericProperty");

            var definitionId = AttributeArgumentValue<string>(attribute, "Definition", null);
            var dataType = FindDataTypeDefinition(definitionId) ?? DefaultDataType;

            if (dataType == null)
                throw new Exception("Default datatype could not be found. Set a known datatype in TypeMappings.DefaultDefinitionId.");

            var property = new GenericProperty
            {
                Alias = propNode.Name.CamelCase(),
                Name = AttributeArgumentValue(attribute, "DisplayName", propNode.Name.SplitPascalCase()),
                Description = AttributeArgumentValue<string>(attribute, "Description", null),
                Definition = dataType.DefinitionId,
                Type = dataType.DataTypeId,
                Tab = AttributeArgumentValue<string>(attribute, "Tab", null),
                Mandatory = AttributeArgumentValue(attribute, "Mandatory", false),
                Validation = AttributeArgumentValue<string>(attribute, "Validation", null)
            };
            contentType.GenericProperties.Add(property);
        }

        // TODO: Dry up
        private DataTypeDefinition FindDataTypeDefinition(string definitionId)
        {
            Guid parsedDefId;
            bool definitionIsGuid = Guid.TryParse(definitionId, out parsedDefId);
            var dataType = DataTypes.SingleOrDefault(dt =>
                definitionIsGuid
                    ? String.Compare(dt.DefinitionId, definitionId, IgnoreCase) == 0
                    : String.Compare(dt.DataTypeName, definitionId, IgnoreCase) == 0
                );
            return dataType;
        }
    }
}
