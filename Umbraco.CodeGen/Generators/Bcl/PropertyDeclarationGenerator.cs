using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.Bcl
{
    public class PropertyDeclarationGenerator : CodeGeneratorBase
    {
        private readonly IList<DataTypeDefinition> dataTypes;
        private readonly CodeGeneratorBase[] memberGenerators;

        public PropertyDeclarationGenerator(
            ContentTypeConfiguration config,
            IList<DataTypeDefinition> dataTypes,
            params CodeGeneratorBase[] memberGenerators
            ) : base(config)
        {
            this.dataTypes = dataTypes;
            this.memberGenerators = memberGenerators;
        }

        public override void Generate(CodeObject codeObject, Entity entity)
        {
            var property = (GenericProperty)entity;
            var propNode = (CodeMemberProperty) codeObject;
            
            SetType(propNode, property);

            foreach (var generator in memberGenerators)
                generator.Generate(codeObject, property);

            SetPublic(propNode);
            AddDataType(propNode, property);
            AddCategory(propNode, property);
            AddRequired(propNode, property);
            AddValidation(propNode, property);
        }

        private void SetPublic(CodeTypeMember propNode)
        {
            propNode.Attributes = MemberAttributes.Public;
        }

        private void SetType(CodeMemberProperty propNode, GenericProperty property)
        {
            var hasType = property.Type != null &&
                Config.TypeMappings.ContainsKey(property.Type.ToLower());
            var typeName = hasType 
                ? Config.TypeMappings[property.Type.ToLower()]
                : Config.DefaultTypeMapping;
            if (typeName == null)
                throw new Exception("TypeMappings/Default not set. Cannot guess default property type.");
            propNode.Type = new CodeTypeReference(typeName);
        }

        private void AddDataType(CodeMemberProperty propNode, GenericProperty property)
        {
            var dataType = dataTypes.SingleOrDefault(dt =>
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
