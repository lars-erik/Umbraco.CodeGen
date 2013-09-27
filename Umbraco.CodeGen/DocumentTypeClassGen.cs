using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Samples.VisualStudio.GeneratorSample;
using Microsoft.VisualStudio.Shell;

namespace Umbraco.CodeGen
{
	[ComVisible(true)]
	[Guid("D699EB4E-221A-44DA-80C0-437CB6FAAEB8")]
	[CodeGeneratorRegistration(typeof(DocumentTypeClassGen), "C# Umbraco Document Type Class Generator", VsContextGuids.VsContextGuidVCSProject, GeneratesDesignTimeSource = false)]
	[ProvideObject(typeof(DocumentTypeClassGen))]
	public class DocumentTypeClassGen : BaseCodeGeneratorWithSite
    {
		private string path;
		private string inputFileContent;
		private CodeGeneratorConfiguration configuration;
		private IEnumerable<ContentTypeDefinition> contentTypes;
		private ContentTypeCodeGenerator generator;

		protected override byte[] GenerateCode(string path, string inputFileContent)
		{
			SetInput(path, inputFileContent);

			try
			{
				using (var writer = new StringWriter(new StringBuilder()))
				{
					GenerateCode(writer);
					return CreatePreamble(writer);
				}
			}
			catch (Exception e)
			{
				GeneratorError(4, e.ToString(), 1, 1);
				return null;
			}
		}

		public void SetInput(string path, string inputFileContent)
		{
			this.inputFileContent = inputFileContent;
			this.path = path;
		}

		public void SetNamespace(string nameSpace)
		{
			FileNameSpace = nameSpace;
		}

		public void GenerateCode(StringWriter writer)
		{
			LoadConfiguration();
			LoadContentTypes();
			CreateGenerator();

			generator.BuildCode(writer);
		}

		private void LoadConfiguration()
		{
			var configProvider = new UsyncConfigurationProvider(inputFileContent);
			configuration = configProvider.GetConfiguration();
		}

		private void LoadContentTypes()
		{
			var contentTypeProvider = new USyncContentTypeProvider(path, inputFileContent);
			contentTypes = contentTypeProvider.GetContentTypeDefinitions();
		}

		private void CreateGenerator()
		{
			generator = new ContentTypeCodeGenerator(configuration, contentTypes, GetCodeProvider(), FileNameSpace);
		}

		private static byte[] CreatePreamble(StringWriter writer)
		{
//Get the Encoding used by the writer. We're getting the WindowsCodePage encoding, 
			//which may not work with all languages
			Encoding enc = Encoding.GetEncoding(writer.Encoding.WindowsCodePage);

			//Get the preamble (byte-order mark) for our encoding
			byte[] preamble = enc.GetPreamble();
			int preambleLength = preamble.Length;

			//Convert the writer contents to a byte array
			byte[] body = enc.GetBytes(writer.ToString());

			//Prepend the preamble to body (store result in resized preamble array)
			Array.Resize<byte>(ref preamble, preambleLength + body.Length);
			Array.Copy(body, 0, preamble, preambleLength, body.Length);

			//Return the combined byte array
			return preamble;
		}
    }
}
