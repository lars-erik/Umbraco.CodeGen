using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.CodeGen.Configuration;
using Umbraco.Core.Logging;

namespace Umbraco.CodeGen.Integration
{
    public class Configuration
    {
        private static CodeGeneratorConfiguration configuration;
        private static USyncConfiguration uSyncConfiguration;
        private static IEnumerable<DataTypeDefinition> dataTypeDefinitions;
        private static USyncDataTypeProvider dataTypesProvider;

        public static CodeGeneratorConfiguration CodeGen
        {
            get
            {
                return configuration;
            }
        }

        public static USyncConfiguration USync
        {
            get
            {
                return uSyncConfiguration;
            }
        }

        public static IEnumerable<DataTypeDefinition> DataTypes
        {
            get
            {
                return dataTypeDefinitions;
            }
        }

        public static USyncDataTypeProvider DataTypesProvider
        {
            get { return dataTypesProvider; }
        }

        public static void Load()
        {
            LoadConfiguration();
            LoadUSyncConfiguration();
            LoadDataTypes();
        }

        public static void SaveConfiguration(CodeGeneratorConfiguration newConfiguration)
        {
            var configurationProvider =
                new CodeGeneratorConfigurationFileProvider(HttpContext.Current.Server.MapPath("~/config/CodeGen.config"));
            configurationProvider.SaveConfiguration(newConfiguration);
            LoadConfiguration();
        }


        private static void LoadConfiguration()
        {
            var configurationProvider =
                new CodeGeneratorConfigurationFileProvider(HttpContext.Current.Server.MapPath("~/config/CodeGen.config"));
            configuration = configurationProvider.GetConfiguration();
        }

        private static void LoadUSyncConfiguration()
        {
            var uSyncConfigurationProvider =
                new USyncConfigurationProvider(HttpContext.Current.Server.MapPath("~/config/uSyncSettings.config"),
                    new HttpContextPathResolver());
            uSyncConfiguration = uSyncConfigurationProvider.GetConfiguration();
        }

        private static void LoadDataTypes()
        {
            dataTypesProvider = new USyncDataTypeProvider(USync.USyncFolder);

            dataTypeDefinitions = DataTypesProvider.GetDataTypes();
            if (!dataTypeDefinitions.Any())
            {
                LogHelper.Error<CodeGenerator>("Could not find data types in usync folder", new Exception());
                dataTypeDefinitions = null;
            }
        }
    }
}