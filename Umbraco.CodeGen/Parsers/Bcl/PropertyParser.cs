using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers.Bcl
{
    public class PropertyParser : ContentTypeCodeParserBase
    {
        private readonly IEnumerable<DataTypeDefinition> dataTypes;
        private readonly DataTypeDefinition defaultDataType;

        public PropertyParser(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes) : base(configuration)
        {
            this.dataTypes = dataTypes;
            defaultDataType = FindDataTypeDefinition(configuration.TypeMappings.DefaultDefinitionId);
        }

        public override void Parse(AstNode node, ContentType contentType)
        {
            var propNode = (PropertyDeclaration) node;
            var definitionId = AttributeValue(propNode, "DataType");
            var dataType = FindDataTypeDefinition(definitionId) ?? defaultDataType;

            if (dataType == null)
                throw new Exception("Default datatype could not be found. Set a known datatype in TypeMappings.DefaultDefinitionId.");

            var property = new GenericProperty
            {
                Alias = propNode.Name.CamelCase(),
                Name = AttributeValue(propNode, "DisplayName", propNode.Name.SplitPascalCase()),
                Description = AttributeValue(propNode, "Description"),
                Definition = dataType.DefinitionId,
                Type = dataType.DataTypeId,
                Tab = AttributeValue(propNode, "Category"),
                Mandatory = FindAttribute(propNode.Attributes, "Required") != null,
                Validation = AttributeValue(propNode, "RegularExpression")
            };
            contentType.GenericProperties.Add(property);
        }

        private DataTypeDefinition FindDataTypeDefinition(string definitionId)
        {
            Guid parsedDefId;
            bool definitionIsGuid = Guid.TryParse(definitionId, out parsedDefId);
            var dataType = dataTypes.SingleOrDefault(dt =>
                definitionIsGuid
                    ? String.Compare(dt.DefinitionId, definitionId, IgnoreCase) == 0
                    : String.Compare(dt.DataTypeName, definitionId, IgnoreCase) == 0
                );
            return dataType;
        }
    }
}