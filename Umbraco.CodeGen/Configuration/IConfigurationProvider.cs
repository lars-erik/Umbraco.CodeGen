using System.IO;

namespace Umbraco.CodeGen.Configuration
{
	public interface IConfigurationProvider
	{
		CodeGeneratorConfiguration GetConfiguration();
	}

    public interface IConfigurationPersister
    {
        void SaveConfiguration(CodeGeneratorConfiguration newConfiguration);
    }
}