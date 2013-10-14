using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class PropertyBodyGenerator : CodeGeneratorBase
    {
        public PropertyBodyGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        public override void Generate(CodeObject codeObject, Entity entity)
        {
            var propNode = (CodeMemberProperty) codeObject;
            var property = (GenericProperty) entity;

            var contentRef = new CodePropertyReferenceExpression(null, "Content");
            var getPropertyValueMethod = new CodeMethodReferenceExpression(contentRef, "GetPropertyValue", propNode.Type);
            var getPropertyValueCall = new CodeMethodInvokeExpression(getPropertyValueMethod, new CodePrimitiveExpression(property.Alias));
            propNode.GetStatements.Add(new CodeMethodReturnStatement(getPropertyValueCall));
        }
    }
}
