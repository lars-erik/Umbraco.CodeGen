using System.IO;

namespace Umbraco.CodeGen.Configuration
{
	public class CodeGeneratorConfigurationFileProvider : IConfigurationProvider
	{
		private readonly IConfigurationProvider provider;

		public CodeGeneratorConfigurationFileProvider(string path)
		{
			using (var reader = File.OpenText(path))
			{
				provider = new CodeGeneratorConfigurationProvider(reader.ReadToEnd());
			}
		}

		public CodeGeneratorConfiguration GetConfiguration()
		{
			return provider.GetConfiguration();
		}
	}
}
