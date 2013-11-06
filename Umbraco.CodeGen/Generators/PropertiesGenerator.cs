using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

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

        public override void Generate(object codeObject, Entity entity)
        {
            var type = (CodeTypeDeclaration) codeObject;
            var contentType = (ContentType) entity;

            foreach (var property in contentType.GenericProperties)
            {
                var propNode = new CodeMemberProperty();
                foreach(var generator in propertyGenerators)
                    generator.Generate(propNode, property);
                type.Members.Add(propNode);
            }
        }
    }
}
