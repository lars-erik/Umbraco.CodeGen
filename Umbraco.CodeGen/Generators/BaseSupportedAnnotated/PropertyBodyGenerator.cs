using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.BaseSupportedAnnotated
{
    public class PropertyBodyGenerator : CodeGeneratorBase
    {
        public PropertyBodyGenerator(ContentTypeConfiguration config)
            : base(config)
        {
        }

        public override void Generate(object codeObject, Entity entity)
        {
            var propNode = (CodeMemberProperty) codeObject;
            var property = (GenericProperty) entity;

            var getPropertyValueMethod = new CodeMethodReferenceExpression(null, "GetValue", propNode.Type);
            var getPropertyValueCall = new CodeMethodInvokeExpression(getPropertyValueMethod, new CodePrimitiveExpression(property.Alias));
            propNode.GetStatements.Add(new CodeMethodReturnStatement(getPropertyValueCall));
        }
    }
}
