using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Generators.Compositions
{
    public class NamedClassGenerator : ClassGenerator
    {
        public NamedClassGenerator(GeneratorConfig config, params CodeGeneratorBase[] memberGenerators) 
            : base(config, new [] { new EntityNameGenerator(config) }.Concat(memberGenerators).ToArray())
        {
        }
    }
}
