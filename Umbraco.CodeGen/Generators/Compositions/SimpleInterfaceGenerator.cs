using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.CodeGen.Configuration;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Generators.Compositions
{
    public class SimpleInterfaceGenerator : InterfaceGenerator
    {
        public SimpleInterfaceGenerator(GeneratorConfig config, params CodeGeneratorBase[] memberGenerators) 
            : base(config, new [] { new InterfaceNameGenerator(config) }.Concat(memberGenerators).ToArray())
        {
        }
    }
}
