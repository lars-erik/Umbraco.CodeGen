using System;
using System.CodeDom;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Umbraco.CodeGen
{
	public class ContentTypeBuilder
	{
		private readonly CodeGeneratorConfiguration configuration;
		private readonly XDocument contentType;
		private readonly XElement info;
		private CodeTypeDeclaration type;

		public ContentTypeBuilder(CodeGeneratorConfiguration config, XDocument type)
		{
			configuration = config;
			contentType = type;
			info = contentType.XPathSelectElement("DocumentType/Info");
		}

		public void Build(CodeNamespace ns)
		{
			ns.Types.Add(CreateType());
		}

		private CodeTypeDeclaration CreateType()
		{
			var className = info.Element("Alias").Value.PascalCase();
			var baseClass = info.Element("Master") != null ? info.Element("Master").Value.PascalCase() : null;
			baseClass = String.IsNullOrEmpty(baseClass) ? configuration.BaseClass : baseClass;

			CreateType(className, baseClass);

			CreateInfoMembers();
			CreateStructureMember();

			var properties = contentType.XPathSelectElements("//GenericProperty");
			foreach (var property in properties)
				CreateAndAddProperty(property);

			return type;
		}

		private void CreateInfoMembers()
		{
			AddSimpleCustomAttribute(type, "DisplayName", info.Element("Name").Value);
			AddSimpleCustomAttribute(type, "Description", info.Element("Description").Value);
			AddConst("icon", info.Element("Icon").Value);
			AddConst("thumbnail", info.Element("Thumbnail").Value);
			AddConst("allowAtRoot", Convert.ToBoolean(info.Element("AllowAtRoot").Value));
			AddAllowedTemplates();
			AddConst("defaultTemplate", info.Element("DefaultTemplate").Value);
		}

		private void CreateStructureMember()
		{
			var structure = contentType.XPathSelectElements("DocumentType/Structure/DocumentType").Select(e => e.Value.PascalCase());
			type.Members.Add(new CodeMemberField("Type[]", "structure")
			{
				InitExpression = new CodeArrayCreateExpression("Type[]", structure.Select(t => new CodeTypeOfExpression(t)).Cast<CodeExpression>().ToArray())
			});
		}

		private void CreateType(string className, string baseClassName)
		{
			type = new CodeTypeDeclaration(className)
			{
				Attributes = MemberAttributes.Public,
				IsPartial = true
			};

			type.BaseTypes.Add(CreateBaseTypeReference(baseClassName));

			type.Members.Add(CreateTypeConstructor());
		}

		private CodeTypeReference CreateBaseTypeReference(string baseClassName)
		{
			if (String.IsNullOrEmpty(baseClassName))
				return new CodeTypeReference(configuration.BaseClass);

			return new CodeTypeReference(
				baseClassName.RemovePrefix(configuration.RemovePrefix)
			);
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

		private void AddAllowedTemplates()
		{
			var templateNames = info.XPathSelectElements("//Template").Select(e => e.Value);
			type.Members.Add(new CodeMemberField("String[]", "allowedTemplates")
			{
				InitExpression = new CodeArrayCreateExpression("String[]", templateNames.Select(n => new CodePrimitiveExpression(n)).Cast<CodeExpression>().ToArray())
			});
		}

		private void CreateAndAddProperty(XElement property)
		{
			var codeProp = new CodeMemberProperty();
			var typeName = GetTypeName(property);
			var name = property.Element("Alias").Value;
			codeProp.Name = name.RemovePrefix(configuration.RemovePrefix).PascalCase();
			codeProp.Type = new CodeTypeReference(typeName);
			codeProp.Attributes = MemberAttributes.Public | MemberAttributes.Final;

			AddSimpleCustomAttribute(codeProp, "DisplayName", property.Element("Name").Value);
			AddSimpleCustomAttribute(codeProp, "Description", property.Element("Description").Value);
			AddSimpleCustomAttribute(codeProp, "Category", property.Element("Tab").Value);
			AddSimpleCustomAttribute(codeProp, "DataType", property.Element("Definition").Value);
			if (!String.IsNullOrEmpty(property.Element("Validation").Value))
				AddSimpleCustomAttribute(codeProp, "RegularExpression", property.Element("Validation").Value);
			if (Convert.ToBoolean(property.Element("Mandatory").Value))
				AddSimpleCustomAttribute(codeProp, "Required");

			var contentRef = new CodePropertyReferenceExpression(null, "Content");
			var getPropertyValueMethod = new CodeMethodReferenceExpression(contentRef, "GetPropertyValue", codeProp.Type);
			var getPropertyValueCall = new CodeMethodInvokeExpression(getPropertyValueMethod, new CodePrimitiveExpression(name));

			codeProp.GetStatements.Add(new CodeMethodReturnStatement(getPropertyValueCall));

			type.Members.Add(codeProp);
		}

		private string GetTypeName(XElement property)
		{
			var typeId = property.Element("Type").Value;
			var typeName = configuration.TypeMappings.ContainsKey(typeId)
				? configuration.TypeMappings[typeId]
				: configuration.DefaultTypeMapping;
			return typeName;
		}

		private static void AddSimpleCustomAttribute(CodeTypeMember codeTypeDeclaration, string attributeName, string value = null)
		{
			var codeAttributeDeclaration = value != null ?
				new CodeAttributeDeclaration(attributeName, new CodeAttributeArgument(new CodePrimitiveExpression(value))) :
				new CodeAttributeDeclaration(attributeName);
			codeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
		}

		private void AddConst<T>(string name, T value)
		{
			type.Members.Add(new CodeMemberField(typeof(T).Name, name)
			{
				Attributes = MemberAttributes.Const,
				InitExpression = new CodePrimitiveExpression(value)
			});
		}
	}
}
