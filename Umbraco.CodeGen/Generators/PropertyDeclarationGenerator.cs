using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Generators
{
    public abstract class PropertyDeclarationGenerator : CodeGeneratorBase
    {
        protected CodeGeneratorBase[] MemberGenerators;

        protected PropertyDeclarationGenerator(
            Configuration.GeneratorConfig config,
            params CodeGeneratorBase[] memberGenerators
            )
            : base(config)
        {
            this.MemberGenerators = memberGenerators;
        }

        public override void Generate(object codeObject, object typeOrPropertyModel)
        {
            var property = (PropertyModel)typeOrPropertyModel;
            var propNode = (CodeMemberProperty)codeObject;

            SetType(propNode, property);

            foreach (var generator in MemberGenerators)
                generator.Generate(codeObject, property);

            SetAttributes(propNode);
        }

        protected abstract void SetAttributes(CodeTypeMember propNode);

        protected void SetType(CodeMemberProperty propNode, PropertyModel property)
        {
            var hasType = property.ClrType != null;
            if (!hasType || property.ClrType == null)
                throw new Exception(String.Format("No ClrType for property {0}. Unable to generate code.", property.Alias));
            propNode.Type = new CodeTypeReference(property.ClrType);
        }
    }
}