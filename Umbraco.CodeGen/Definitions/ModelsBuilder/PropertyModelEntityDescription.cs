using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Definitions.ModelsBuilder
{
    public class PropertyModelEntityDescription : IEntityDescription
    {
        private PropertyModel model;

        public PropertyModelEntityDescription(PropertyModel propertyModel)
        {
            this.model = propertyModel;
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
