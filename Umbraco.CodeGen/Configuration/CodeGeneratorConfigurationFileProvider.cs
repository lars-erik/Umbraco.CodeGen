using System.IO;

namespace Umbraco.CodeGen.Configuration
{
    public class CodeGeneratorConfigurationFileProvider : IConfigurationProvider, IConfigurationPersister
	{
	    private readonly string path;
	    private readonly IConfigurationProvider provider;

		public CodeGeneratorConfigurationFileProvider(string path)
		{
		    this.path = path;
		    using (var reader = File.OpenText(path))
			{
				provider = new CodeGeneratorConfigurationProvider(reader.ReadToEnd());
			}
		}

	    public CodeGeneratorConfiguration GetConfiguration()
		{
			return provider.GetConfiguration();
		}

	    public void SaveConfiguration(CodeGeneratorConfiguration configuration)
	    {
	        using (var writer = File.CreateText(path))
	        {
                CodeGeneratorConfigurationProvider.SerializeConfiguration(configuration, writer);
	        }
	    }
	}
}
