using System;
using System.Collections.Generic;
using System.Net.Mime;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Generators
{
    public abstract class CodeGeneratorFactory
    {
        public abstract CodeGeneratorBase Create(string configuredNamespace);

        public static T CreateFactory<T>(string typeName)
        {
            try
            {
                var factoryType = Type.GetType(typeName);
                if (factoryType == null)
                    factoryType = Type.GetType(String.Format("{0}, Umbraco.CodeGen", typeName));
                if (factoryType == null)
                    throw new Exception(String.Format("Type {0} not found", typeName));
                return (T)Activator.CreateInstance(factoryType);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Invalid factory '{0}'", typeName), ex);
            }
        }
    }
}