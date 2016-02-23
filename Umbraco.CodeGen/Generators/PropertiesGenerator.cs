using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Generators
{
    public class PropertiesGenerator : CodeGeneratorBase
    {
        private readonly CodeGeneratorBase[] propertyGenerators;

        public PropertiesGenerator(
            ContentTypeConfiguration config,
            params CodeGeneratorBase[] propertyGenerators
            ) : base(config)
        {
            this.propertyGenerators = propertyGenerators;
        }

        public override void Generate(object codeObject, object typeOrPropertyModel)
        {
            var type = (CodeTypeDeclaration) codeObject;
            var typeModel = (TypeModel) typeOrPropertyModel;

            foreach (var property in typeModel.Properties.Union(typeModel.MixinTypes.SelectMany(c => c.Properties)))
            {
                var propNode = new CodeMemberProperty();
                foreach(var generator in propertyGenerators)
                    generator.Generate(propNode, property);
                type.Members.Add(propNode);
            }
        }
    }
}
