using System;
using System.CodeDom;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Umbraco.CodeGen
{
	public class ContentTypeBuilder
	{
		private readonly ContentTypeConfiguration configuration;
		private readonly XDocument contentType;
		private readonly XElement info;
		private CodeTypeDeclaration type;
		private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

		public ContentTypeBuilder(ContentTypeConfiguration config, XDocument type)
		{
			configuration = config;
			contentType = type;
			info = contentType.XPathSelectElement(config.ContentTypeName + "/Info");
		}

		public void Build(CodeNamespace ns)
		{
			ns.Types.Add(CreateType());
		}

		private CodeTypeDeclaration CreateType()
		{
			var className = info.ElementValue("Alias").PascalCase();
			var baseClass = info.ElementValue("Master").PascalCase();
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
			AddSimpleCustomAttribute(type, "DisplayName", info.ElementValue("Name"));
			AddSimpleCustomAttribute(type, "Description", info.ElementValue("Description"));
			AddConst("icon", info.ElementValue("Icon"));
			AddConst("thumbnail", info.ElementValue("Thumbnail"));
			AddConst("allowAtRoot", Convert.ToBoolean(info.ElementValue("AllowAtRoot")));

			// TODO: Refactor everything :)
			if (configuration.ContentTypeName == "DocumentType")
			{
				AddAllowedTemplates();
				AddConst("defaultTemplate", info.ElementValue("DefaultTemplate"));
			}
		}

		private void CreateStructureMember()
		{
			var structure = contentType
				.XPathSelectElements(configuration.ContentTypeName + "/Structure/" + configuration.ContentTypeName)
				.Where(e => !String.IsNullOrEmpty(e.Value))
				.Select(e => e.Value.PascalCase());
			type.Members.Add(new CodeMemberField("Type[]", "structure")
			{
				InitExpression = new CodeArrayCreateExpression("Type[]", 
					structure.Select(t => new CodeTypeOfExpression(t))
						.Cast<CodeExpression>()
						.ToArray()
				)
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
			var ctor = new CodeConstructor
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
			var name = property.ElementValue("Alias");
			if (String.Compare(name, "Content", IgnoreCase) == 0)
				name = "ContentProperty";
			codeProp.Name = name.RemovePrefix(configuration.RemovePrefix).PascalCase();
			codeProp.Type = new CodeTypeReference(typeName);
			codeProp.Attributes = (MemberAttributes)((int)MemberAttributes.Public | (int)MemberAttributes.Final);

			AddSimpleCustomAttribute(codeProp, "DisplayName", property.ElementValue("Name"));
			AddSimpleCustomAttribute(codeProp, "Description", property.ElementValue("Description"));
			AddSimpleCustomAttribute(codeProp, "Category", property.ElementValue("Tab"));
			AddSimpleCustomAttribute(codeProp, "DataType", property.ElementValue("Definition").ToLower());
			if (!String.IsNullOrEmpty(property.ElementValue("Validation")))
				AddSimpleCustomAttribute(codeProp, "RegularExpression", property.ElementValue("Validation"));
			if (Convert.ToBoolean(property.ElementValue("Mandatory")))
				AddSimpleCustomAttribute(codeProp, "Required");

			var contentRef = new CodePropertyReferenceExpression(null, "Content");
			var getPropertyValueMethod = new CodeMethodReferenceExpression(contentRef, "GetPropertyValue", codeProp.Type);
			var getPropertyValueCall = new CodeMethodInvokeExpression(getPropertyValueMethod, new CodePrimitiveExpression(name));

			codeProp.GetStatements.Add(new CodeMethodReturnStatement(getPropertyValueCall));

			type.Members.Add(codeProp);
		}

		private string GetTypeName(XElement property)
		{
			var typeId = property.ElementValue("Type").ToLower();
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
