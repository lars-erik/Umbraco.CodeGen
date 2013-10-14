namespace Umbraco.CodeGen.Configuration
{
	public interface IConfigurationProvider
	{
		CodeGeneratorConfiguration GetConfiguration();
	}
}