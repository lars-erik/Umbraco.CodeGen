using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class CompositeCodeGenerator : CodeGeneratorBase
    {
        private readonly CodeGeneratorBase[] generators;

        public CompositeCodeGenerator(
            ContentTypeConfiguration config,
            params CodeGeneratorBase[] generators
        ) : base(config)
        {
            this.generators = generators;
        }

        public override void Generate(object codeObject, object typeOrPropertyModel)
        {
            if (generators != null)
                foreach(var generator in generators)
                    generator.Generate(codeObject, typeOrPropertyModel);
        }
    }
}
