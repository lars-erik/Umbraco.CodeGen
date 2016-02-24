using System.CodeDom;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Generators
{
    public class InterfaceGenerator : CompositeCodeGenerator
    {
        public InterfaceGenerator(Configuration.GeneratorConfig config, params CodeGeneratorBase[] generators) : base(config, generators)
        {
        }

        public override void Generate(object codeObject, object typeOrPropertyModel)
        {
            var model = (TypeModel)typeOrPropertyModel;
            if (!model.IsMixin)
                return;

            var ns = (CodeNamespace)codeObject;
            var type = new CodeTypeDeclaration();
            ns.Types.Add(type);

            type.IsInterface = true;
            type.IsPartial = true;
            type.BaseTypes.Add(new CodeTypeReference("IPublishedContent"));

            base.Generate(type, typeOrPropertyModel);
        }
    }
}
