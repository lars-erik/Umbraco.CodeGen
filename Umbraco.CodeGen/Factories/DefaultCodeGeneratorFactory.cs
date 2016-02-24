using System.ComponentModel;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Factories
{
    [Description("Use whichever generator the dev deems appropriate for this version")]
    public class DefaultCodeGeneratorFactory : CodeGeneratorFactory
    {
        private readonly CodeGeneratorFactory inner = new SimpleModelGeneratorFactory();

        public override CodeGeneratorBase Create(Configuration.GeneratorConfig config)
        {
            return inner.Create(config);
        }
    }
}
