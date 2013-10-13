using System.CodeDom;
using System.Collections.Generic;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests.Generators
{
    public class SpyGenerator : CodeGeneratorBase
    {
        public bool Called;
        public List<CodeObject> CodeObjects = new List<CodeObject>();

        public SpyGenerator() : base(null)
        {
        }

        public override void Generate(CodeObject codeObject, Entity entity)
        {
            CodeObjects.Add(codeObject);
            Called = true;
        }
    }
}