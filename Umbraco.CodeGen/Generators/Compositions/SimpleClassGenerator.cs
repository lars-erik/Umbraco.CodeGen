using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Generators.Compositions
{
    public class SimpleClassGenerator : NamedClassGenerator
    {
        public SimpleClassGenerator(GeneratorConfig config, params CodeGeneratorBase[] memberGenerators) : 
            base(config, new [] { new CtorGenerator(config) }.Concat(memberGenerators).ToArray())
        {
        }
    }
}
