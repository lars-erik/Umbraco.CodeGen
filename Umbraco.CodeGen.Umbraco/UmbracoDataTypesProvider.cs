using System.Collections.Generic;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.Web;

namespace Umbraco.CodeGen.Umbraco
{
    public class UmbracoDataTypesProvider : IDataTypeProvider
    {
        private readonly object lockObj = new object();
        private IEnumerable<DataTypeDefinition> dataTypes;

        public IEnumerable<DataTypeDefinition> GetDataTypes()
        {
            lock(lockObj)
            { 
                if (dataTypes == null)
                { 
                    var umbracoDefinitions = UmbracoContext.Current.Application.Services.DataTypeService.GetAllDataTypeDefinitions();
                    dataTypes = umbracoDefinitions.Select(d => new DataTypeDefinition(d.Name, "", d.PropertyEditorAlias, ""));
                }
            }
            return dataTypes;
        }
    }
}
