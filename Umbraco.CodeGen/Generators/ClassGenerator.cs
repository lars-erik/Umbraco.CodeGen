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
    public class ClassGenerator : CompositeCodeGenerator
    {
        public ClassGenerator(
            ContentTypeConfiguration config, 
            params CodeGeneratorBase[] memberGenerators
            )
            : base(config, memberGenerators)
        {
        }

        public override void Generate(object codeObject, object typeOrPropertyModel)
        {
            var typeModel = (TypeModel) typeOrPropertyModel;

            var ns = (CodeNamespace)codeObject;
            var type = new CodeTypeDeclaration();
            ns.Types.Add(type);

            SetPartial(type);
            SetBaseClass(type, typeModel);
            AddInterfaces(type, typeModel);

            base.Generate(type, typeOrPropertyModel);
        }

        private void SetPartial(CodeTypeDeclaration type)
        {
            type.IsPartial = true;
        }

        protected void SetBaseClass(CodeTypeDeclaration type, TypeModel typeModel)
        {
            var baseReference = typeModel.HasBase
                                    ? new CodeTypeReference(typeModel.BaseType.ClrName)
                                    : new CodeTypeReference(Config.BaseClass);
            type.BaseTypes.Add(baseReference);
        }

        private void AddInterfaces(CodeTypeDeclaration type, TypeModel typeModel)
        {
            if (typeModel.IsMixin)
                type.BaseTypes.Add((new CodeTypeReference("I" + typeModel.ClrName)));

            foreach (var composition in typeModel.MixinTypes)
                type.BaseTypes.Add((new CodeTypeReference("I" + composition.ClrName)));
        }
    }
}
