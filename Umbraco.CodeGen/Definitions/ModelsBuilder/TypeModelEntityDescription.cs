using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Definitions.ModelsBuilder
{
    public class TypeModelEntityDescription : IEntityDescription
    {
        private TypeModel model;

        public TypeModelEntityDescription(TypeModel typeModel)
        {
            model = typeModel;
        }

        public string Name
        {
            get { return model.Name; }
        }

        public string Alias
        {
            get { return model.Alias; }
        }

        public string Description
        {
            get { return model.Description; }
        }
    }
}
