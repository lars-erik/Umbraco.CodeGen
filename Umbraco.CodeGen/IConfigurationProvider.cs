using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen
{
	public interface IConfigurationProvider
	{
		CodeGeneratorConfiguration GetConfiguration();
	}
}