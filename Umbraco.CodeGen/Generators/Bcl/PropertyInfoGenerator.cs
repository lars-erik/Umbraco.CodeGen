using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.Bcl
{
    public class PropertyInfoGenerator : PropertyDeclarationGenerator
    {
        public PropertyInfoGenerator(
            ContentTypeConfiguration config,
            IList<DataTypeDefinition> dataTypes,
            params CodeGeneratorBase[] memberGenerators
            ) : base(config, dataTypes, memberGenerators)
        {
        }

        public override void Generate(object codeObject, Entity entity)
        {
            base.Generate(codeObject, entity);

            var property = (GenericProperty)entity;
            var propNode = (CodeMemberProperty) codeObject;
            
            AddDataType(propNode, property);
            AddCategory(propNode, property);
            AddRequired(propNode, property);
            AddValidation(propNode, property);
        }

        private void AddDataType(CodeMemberProperty propNode, GenericProperty property)
        {
            var dataType = DataTypes.SingleOrDefault(dt =>
                String.Compare(dt.DefinitionId, property.Definition, IgnoreCase) == 0 ||
                String.Compare(dt.DataTypeName, property.Definition, IgnoreCase) == 0);
            var dataTypeValue = dataType != null
                ? dataType.DataTypeName
                : Config.DefaultDefinitionId;
            if (dataTypeValue == null)
                throw new Exception("TypeMappings/DefaultDefinitionId not set. Cannot guess default definition.");
            AddAttribute(propNode, "DataType", dataTypeValue);
        }

        private void AddCategory(CodeMemberProperty propNode, GenericProperty property)
        {
            if (String.IsNullOrWhiteSpace(property.Tab))
                return;
            AddAttribute(propNode, "Category", property.Tab);
        }

        private void AddRequired(CodeMemberProperty propNode, GenericProperty property)
        {
            if (property.Mandatory)
                AddAttribute(propNode, "Required");
        }

        private void AddValidation(CodeMemberProperty propNode, GenericProperty property)
        {
            if (String.IsNullOrWhiteSpace(property.Validation))
                return;
            AddAttribute(propNode, "RegularExpression", property.Validation);
        }
    }
}
