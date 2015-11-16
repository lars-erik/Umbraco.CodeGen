using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public abstract class PropertyDeclarationGenerator : CodeGeneratorBase
    {
        protected IList<DataTypeDefinition> DataTypes;
        protected CodeGeneratorBase[] MemberGenerators;

        protected PropertyDeclarationGenerator(
            ContentTypeConfiguration config,
            IList<DataTypeDefinition> dataTypes,
            params CodeGeneratorBase[] memberGenerators
            ) : base(config)
        {
            this.DataTypes = dataTypes;
            this.MemberGenerators = memberGenerators;
        }

        public override void Generate(object codeObject, Entity entity)
        {
            var property = (GenericProperty)entity;
            var propNode = (CodeMemberProperty)codeObject;

            SetType(propNode, property);

            foreach (var generator in MemberGenerators)
                generator.Generate(codeObject, property);

            SetAttributes(propNode);
        }

        protected abstract void SetAttributes(CodeTypeMember propNode);

        protected void SetType(CodeMemberProperty propNode, GenericProperty property)
        {
            var hasType = property.PropertyEditorAlias != null;
            var typeName = hasType 
                ? DataTypes.Single(d => d.PropertyEditorAlias == property.PropertyEditorAlias).ClrType.FullName
                : Config.TypeMappings.DefaultType;
            if (typeName == null)
                throw new Exception("TypeMappings/Default not set. Cannot guess default property type.");
            propNode.Type = new CodeTypeReference(typeName);
        }
    }
}