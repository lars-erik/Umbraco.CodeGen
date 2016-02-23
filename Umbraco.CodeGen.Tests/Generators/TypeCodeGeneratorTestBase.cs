using System.CodeDom;
using System.Linq;
using Umbraco.CodeGen.Definitions;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Tests.Generators
{
    public abstract class TypeCodeGeneratorTestBase : CodeGeneratorTestBase
    {
        protected TypeModel ContentType;
        protected CodeTypeDeclaration Type;

        protected object PrimitiveFieldValue(string fieldName)
        {
            var field = FindField(fieldName);
            if (field == null) return null;
            return ((CodePrimitiveExpression)field.InitExpression).Value;
        }

        protected CodeMemberField FindField(string fieldName)
        {
            return Type.Members.Cast<CodeMemberField>()
                       .SingleOrDefault(f => f.Name == fieldName);
        }
    }
}