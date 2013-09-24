using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using EnvDTE;
using Microsoft.Samples.VisualStudio.GeneratorSample;
using Microsoft.VisualStudio.Shell;
using CodeNamespace = System.CodeDom.CodeNamespace;

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
		private IEnumerable<XElement> uSyncNode;
		private string uSyncPath;
		private string removePrefix;
		private string inputFolderPath;
		private Dictionary<string, string[]> files;
		private CodeCompileUnit compileUnit;
		private CodeNamespace ns;
		private CodeTypeDeclaration baseType;
		private Dictionary<string, string> typeMappings;
		private string defaultTypeMapping;

		protected override byte[] GenerateCode(string path, string inputFileContent)
		{
			SetInput(path, inputFileContent);

			try
			{
				using (var writer = new StringWriter(new StringBuilder()))
				{
					BuildCode(writer);

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

		public void BuildCode(StringWriter writer)
		{
			LoadInputConfiguration();
			FindFiles();

			CreateCompileUnit();
			CreateAndAddNamespace();
			CreateAndAddImports();

			baseType = CreateAndAddBaseType();

			foreach (var nodeType in files.Keys)
				foreach (var file in files[nodeType])
					CreateAndAddType(nodeType, file);

			WriteCode(writer, compileUnit);
			writer.Flush();
		}

		private void CreateAndAddType(string nodeType, string file)
		{
			var typeNode = GetTypeNode(nodeType, file);
			var infoNode = typeNode.Element("Info");
			var propNode = typeNode.Element("GenericProperties");
			var className = infoNode.Element("Alias").Value;
			var baseClassName = infoNode.Elements("Master").Select(e => e.Value).SingleOrDefault();
			var properties = propNode.Elements("GenericProperty");

			className = ProperCase(RemovePrefix(removePrefix, className));
			if (HasPropertyWithSameName(properties, className))
				className += "Class";
			var type = CreateType(className, baseClassName);
			foreach (var property in properties)
				CreateAndAddProperty(property, type);

			ns.Types.Add(type);
		}

		private static bool HasPropertyWithSameName(IEnumerable<XElement> properties, string className)
		{
			return properties.Any(p => String.Compare(p.Element("Alias").Value, className, StringComparison.OrdinalIgnoreCase) == 0);
		}

		private void CreateAndAddImports()
		{
			ns.Imports.Add(new CodeNamespaceImport("System"));
			ns.Imports.Add(new CodeNamespaceImport("System.Reflection"));
			ns.Imports.Add(new CodeNamespaceImport("Umbraco.Core.Models"));
		}

		private CodeTypeDeclaration CreateType(string className, string baseClassName)
		{
			var type = new CodeTypeDeclaration(className)
			{
				Attributes = MemberAttributes.Public, 
				IsPartial = true
			};
			if (!String.IsNullOrEmpty(baseClassName))
			{
				baseClassName = RemovePrefix(removePrefix, baseClassName);
				type.BaseTypes.Add(baseClassName);
			}
			else
			{
				type.BaseTypes.Add(new CodeTypeReference(baseType.Name));
			}

			type.Members.Add(CreateTypeConstructor());

			return type;
		}

		private CodeTypeMember CreateTypeConstructor()
		{
			var ctor = new CodeConstructor()
			{
				Attributes = MemberAttributes.Public
			};
			ctor.Parameters.Add(new CodeParameterDeclarationExpression("IPublishedContent", "content"));
			ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("content"));
			return ctor;
		}

		private void CreateAndAddProperty(XElement property, CodeTypeDeclaration type)
		{
			var codeProp = new CodeMemberProperty();
			var typeName = typeMappings.ContainsKey(property.Element("Type").Value) ?
				typeMappings[property.Element("Type").Value] :
				defaultTypeMapping;
			codeProp.Name = ProperCase(property.Element("Alias").Value);
			codeProp.Type = new CodeTypeReference(
				typeName
				);
			codeProp.Attributes = MemberAttributes.Public | MemberAttributes.Final;

			var getValueRef = new CodeMethodReferenceExpression(null, "GetValue");
			var getValueCall = new CodeMethodInvokeExpression(getValueRef);
			getValueCall.Parameters.Add(new CodeMethodInvokeExpression(null, "ThisProperty"));
			var castedValue = new CodeCastExpression(typeName, getValueCall);
			codeProp.GetStatements.Add(new CodeMethodReturnStatement(castedValue));

			type.Members.Add(codeProp);
		}

		private static XElement GetTypeNode(string nodeType, string file)
		{
			var fileDoc = XDocument.Load(file);
			var typeNode = fileDoc
				.Element(nodeType);
			return typeNode;
		}

		private CodeTypeDeclaration CreateAndAddBaseType()
		{
			var baseType = new CodeTypeDeclaration("DocumentTypeBase")
			{
				IsPartial = true,
				Attributes = MemberAttributes.Public
			};
			baseType.Members.Add(CreateRemovePrefixConst());
			baseType.Members.Add(CreateThisPropertyFunc());
			baseType.Members.Add(CreatePublishedContentMember());
			baseType.Members.Add(CreateBaseTypeCtor());
			baseType.Members.Add(CreateRemoveGetMethod());
			baseType.Members.Add(CreateGetValueMethod());
			ns.Types.Add(baseType);
			return baseType;
		}

		private CodeTypeMember CreateRemovePrefixConst()
		{
			return new CodeMemberField("String", "RemovePrefix")
			{
				Attributes = MemberAttributes.Const | MemberAttributes.Private,
				InitExpression = new CodePrimitiveExpression(removePrefix)
			};

		}

		private CodeTypeMember CreateBaseTypeCtor()
		{
			var ctor = new CodeConstructor()
			{
				Attributes = MemberAttributes.Family
			};
			ctor.Parameters.Add(new CodeParameterDeclarationExpression("IPublishedContent", "content"));
			ctor.Statements.Add(
				new CodeAssignStatement(
					new CodeFieldReferenceExpression(
						new CodeThisReferenceExpression(),
						"Content"
						),
					new CodeVariableReferenceExpression("content")
				)
			);
			return ctor;
		}

		private CodeTypeMember CreatePublishedContentMember()
		{
			var member = new CodeSnippetTypeMember("        protected readonly IPublishedContent Content;");
			return member;
		}

		private static CodeMemberMethod CreateGetValueMethod()
		{
			var getValueMethod = new CodeMemberMethod();
			getValueMethod.Name = "GetValue";
			getValueMethod.Attributes = MemberAttributes.Family | MemberAttributes.Final;
			getValueMethod.ReturnType = new CodeTypeReference("Object");

			getValueMethod.Parameters.Add(new CodeParameterDeclarationExpression("MethodBase", "m"));

			var invokeRemoveGetStatement = new CodeMethodInvokeExpression(null, "RemovePrefixes");
			invokeRemoveGetStatement.Parameters.Add(new CodeVariableReferenceExpression("m"));

			var contentGetPropertyCall = new CodeMethodInvokeExpression(
				new CodeFieldReferenceExpression(null, "Content"),
				"GetProperty"
				);
			contentGetPropertyCall.Parameters.Add(invokeRemoveGetStatement);
			var invokeContentGetPropertyValue =
				new CodePropertyReferenceExpression(
					contentGetPropertyCall,
					"Value"
				);

			var getValueReturnStatement = new CodeMethodReturnStatement(invokeContentGetPropertyValue);
			getValueMethod.Statements.Add(getValueReturnStatement);
			return getValueMethod;
		}

		private static CodeMemberField CreateThisPropertyFunc()
		{
			var methodBaseFunc = new CodeMemberField("Func<MethodBase>", "ThisProperty")
			{
				Attributes = MemberAttributes.Family,
				InitExpression = new CodeMethodReferenceExpression(
					new CodeTypeReferenceExpression("MethodBase"),
					"GetCurrentMethod"
				)
			};
			return methodBaseFunc;
		}

		private static CodeMemberMethod CreateRemoveGetMethod()
		{
			var removeGetMethod = new CodeMemberMethod
			{
				Name = "RemovePrefixes",
				Attributes = MemberAttributes.Private,
				ReturnType = new CodeTypeReference(typeof (string)),
			};

			removeGetMethod.Parameters.Add(
				new CodeParameterDeclarationExpression(typeof (MethodBase), "m")
				);

			var methodNameReference = new CodePropertyReferenceExpression(
				new CodeVariableReferenceExpression("m"),
				"Name"
				);

			var replaceGetCall = CreateReplaceCall(methodNameReference, new CodePrimitiveExpression("_get"));
			var replaceGetCall2 = CreateReplaceCall(replaceGetCall, new CodeFieldReferenceExpression(null, "RemovePrefix"));

			removeGetMethod.Statements.Add(new CodeMethodReturnStatement(replaceGetCall2));
			return removeGetMethod;
		}

		private static CodeMethodInvokeExpression CreateReplaceCall(CodeExpression stringReference, CodeExpression removeValue)
		{
			var replaceGetCall = new CodeMethodInvokeExpression(stringReference, "Replace");
			replaceGetCall.Parameters.Add(removeValue);
			replaceGetCall.Parameters.Add(new CodePrimitiveExpression(""));
			return replaceGetCall;
		}

		private void CreateAndAddNamespace()
		{
			ns = new CodeNamespace(FileNameSpace);
			compileUnit.Namespaces.Add(ns);
		}

		private void CreateCompileUnit()
		{
			compileUnit = new CodeCompileUnit();
		}

		private void WriteCode(StringWriter writer, CodeCompileUnit compileUnit)
		{
			var provider = GetCodeProvider();
			var options = new CodeGeneratorOptions
			{
				BlankLinesBetweenMembers = false,
				BracingStyle = "C"
			};
			provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
		}

		private void FindFiles()
		{
			var nodeTypes = new[] {"DocumentType", "MediaType"};
			files = new Dictionary<string, string[]>();
			foreach(var nodeType in nodeTypes)
			{
				var absoluteUSyncPath = Path.Combine(inputFolderPath, uSyncPath, nodeType);
				files.Add(nodeType, Directory.GetFiles(absoluteUSyncPath, "def.config", SearchOption.AllDirectories));
			}
		}

		private void LoadInputConfiguration()
		{
			var doc = XDocument.Parse(inputFileContent);
			uSyncNode = doc.Descendants("USync");
			uSyncPath = uSyncNode.Attributes("path").Single().Value;
			removePrefix = uSyncNode.Attributes("removePrefix").Select(a => a.Value).SingleOrDefault();
			inputFolderPath = Path.GetDirectoryName(path);
			defaultTypeMapping = doc.Descendants("TypeMappings").Attributes("default").Select(a => a.Value).SingleOrDefault() ?? "String";
			typeMappings = doc.Descendants("TypeMapping")
				.Select(e => new {DataType = e.Attribute("dataType").Value, Type = e.Attribute("type").Value})
				.ToDictionary(a => a.DataType, a => a.Type);
		}

		private static string ProperCase(string value)
		{
			return value.Substring(0, 1).ToUpper() + value.Substring(1);
		}

		private static string RemovePrefix(string removePrefix, string name)
		{
			if (removePrefix != null && name.StartsWith(removePrefix))
				name = name.Substring(removePrefix.Length);
			return name;
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
