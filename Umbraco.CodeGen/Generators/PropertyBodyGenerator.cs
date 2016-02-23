using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Generators
{
    public class PropertyBodyGenerator : CodeGeneratorBase
    {
        public PropertyBodyGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        public override void Generate(object codeObject, object typeOrPropertyModel)
        {
            var propNode = (CodeMemberProperty) codeObject;
            var property = (PropertyModel) typeOrPropertyModel;

            var contentRef = new CodePropertyReferenceExpression(null, "Content");
            var getPropertyValueMethod = new CodeMethodReferenceExpression(contentRef, "GetPropertyValue", propNode.Type);
            var getPropertyValueCall = new CodeMethodInvokeExpression(getPropertyValueMethod, new CodePrimitiveExpression(property.Alias));
            propNode.GetStatements.Add(new CodeMethodReturnStatement(getPropertyValueCall));
        }
    }
}
