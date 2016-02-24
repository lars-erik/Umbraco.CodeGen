using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Generators.Compositions
{
    public class InterfacePropertyGenerator : InterfacePropertyDeclarationGenerator
    {
        public InterfacePropertyGenerator(GeneratorConfig config, params CodeGeneratorBase[] generators) : 
            base(config, 
                new CodeGeneratorBase[]
                {
                    new EntityNameGenerator(config),
                    new InterfacePropertyBodyGenerator(config)
                }
                .Concat(generators)
                .ToArray()
                )
        {
        }
    }
}
