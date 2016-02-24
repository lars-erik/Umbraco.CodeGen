using System;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Factories
{
    public abstract class CodeGeneratorFactory
    {
        public abstract CodeGeneratorBase Create(Configuration.GeneratorConfig config);

        public static CodeGeneratorFactory CreateFactory(string typeName)
        {
            try
            {
                var factoryType = Type.GetType(typeName);
                if (factoryType == null)
                    factoryType = Type.GetType(String.Format("{0}, Umbraco.CodeGen", typeName));
                if (factoryType == null)
                    throw new Exception(String.Format("Type {0} not found", typeName));
                return (CodeGeneratorFactory)Activator.CreateInstance(factoryType);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Invalid factory '{0}'", typeName), ex);
            }
        }
    }
}