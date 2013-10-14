using System.IO;
using System.Xml.Serialization;

namespace Umbraco.CodeGen
{
	public class CodeGeneratorConfigurationProvider : IConfigurationProvider
	{
		private readonly string inputFileContent;
		private CodeGeneratorConfiguration configuration;


		public CodeGeneratorConfigurationProvider(string inputFileContent)
		{
			this.inputFileContent = inputFileContent;
		}

		public CodeGeneratorConfiguration GetConfiguration()
		{
			if (configuration == null)
				configuration = LoadConfiguration();

			return configuration;
		}

		private CodeGeneratorConfiguration LoadConfiguration()
		{
		    var serializer = new XmlSerializer(typeof (CodeGeneratorConfiguration));
		    return (CodeGeneratorConfiguration) serializer.Deserialize(new StringReader(inputFileContent));
		}
	}
}