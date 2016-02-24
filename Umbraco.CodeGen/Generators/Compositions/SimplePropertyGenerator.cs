using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Generators.Compositions
{
    public class SimplePropertyGenerator : PublicPropertyDeclarationGenerator
    {
        public SimplePropertyGenerator(GeneratorConfig config, params CodeGeneratorBase[] memberGenerators) : 
            base(config, 
                new CodeGeneratorBase[]
                {
                    new EntityNameGenerator(config),
                    new PropertyBodyGenerator(config)
                }
                .Concat(memberGenerators)
                .ToArray()
                )
        {
        }
    }
}
