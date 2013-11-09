using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.Annotated
{
    public class PropertyInfoGenerator : EntityDescriptionGenerator
    {
        private readonly IList<DataTypeDefinition> dataTypes;

        public PropertyInfoGenerator(
            ContentTypeConfiguration config,
            IList<DataTypeDefinition> dataTypes
            ) : base(config)
        {
            this.dataTypes = dataTypes;
        }

        public override void Generate(object codeObject, Entity entity)
        {
            base.Generate(codeObject, entity);

            var property = (GenericProperty) entity;
            var attribute = (CodeAttributeDeclaration) codeObject;

            AddDataType(attribute, property);
            AddAttributeArgumentIfValue(attribute, "Tab", property.Tab);
            if (property.Mandatory)
                AddAttributePrimitiveArgument(attribute, "Mandatory", true);
            AddAttributeArgumentIfValue(attribute, "Validation", property.Validation);
        }

        private void AddDataType(CodeAttributeDeclaration attribute, GenericProperty property)
        {
            var dataType = dataTypes.SingleOrDefault(dt =>
            String.Compare(dt.DefinitionId, property.Definition, IgnoreCase) == 0 ||
            String.Compare(dt.DataTypeName, property.Definition, IgnoreCase) == 0);
            var dataTypeValue = dataType != null
                ? dataType.DataTypeName
                : Config.DefaultDefinitionId;
            if (dataTypeValue == null)
                throw new Exception("TypeMappings/DefaultDefinitionId not set. Cannot guess default definition.");

            AddAttributePrimitiveArgument(attribute, "Definition", dataTypeValue);
        }
    }
}
