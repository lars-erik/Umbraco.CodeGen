using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Definitions.ModelsBuilder
{
    public class EntityDescriptionFactory
    {
        public static IEntityDescription TypeModel(TypeModel model)
        {
            return new TypeModelEntityDescription(model);
        }

        public static IEntityDescription PropertyModel(PropertyModel model)
        {
            return new PropertyModelEntityDescription(model);
        }

        public static IEntityDescription FromObject(object model)
        {
            var typeModel = model as TypeModel;
            if (typeModel != null)
                return TypeModel(typeModel);
            var propModel = model as PropertyModel;
            if (propModel != null)
                return PropertyModel(propModel);
            throw new Exception("model has to be of type TypeModel or PropertyModel");
        }
    }
}
