using System.CodeDom;
using System.Collections.Generic;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests.Generators
{
    public class SpyGenerator : CodeGeneratorBase
    {
        public bool Called;
        public List<object> CodeObjects = new List<object>();

        public SpyGenerator() : base(null)
        {
        }

        public override void Generate(object codeObject, Entity entity)
        {
            CodeObjects.Add(codeObject);
            Called = true;
        }
    }
}