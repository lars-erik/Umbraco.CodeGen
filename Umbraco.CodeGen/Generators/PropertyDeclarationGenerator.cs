using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Generators
{
    public abstract class PropertyDeclarationGenerator : CompositeCodeGenerator
    {
        protected PropertyDeclarationGenerator(GeneratorConfig config, params CodeGeneratorBase[] childGenerators) 
            : base(config, childGenerators)
        {
        }

        public override void Generate(object codeObject, object typeOrPropertyModel)
        {
            var property = (PropertyModel)typeOrPropertyModel;
            var propNode = (CodeMemberProperty)codeObject;

            SetType(propNode, property);
            SetAttributes(propNode);

            base.Generate(codeObject, typeOrPropertyModel);
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