using System;
using System.CodeDom;
using System.Collections.Generic;

namespace Umbraco.CodeGen
{
	public class ContentTypesBuilder : INamespaceMemberBuilder
	{
		private CodeGeneratorConfiguration configuration;
		private IEnumerable<ContentTypeDefinition> contentTypes;

		public void Configure(CodeGeneratorConfiguration config, IEnumerable<ContentTypeDefinition> types)
		{
			configuration = config;
			contentTypes = types;
		}

		public void Build(CodeNamespace ns)
		{
			foreach (var contentType in contentTypes)
				ns.Types.Add(CreateType(contentType));
		}

		private CodeTypeDeclaration CreateType(ContentTypeDefinition definition)
		{
			var type = CreateType(definition.ClassName, definition.BaseClassName);
			foreach (var property in definition.Properties)
				CreateAndAddProperty(property, type);

			return type;
		}

		private CodeTypeDeclaration CreateType(string className, string baseClassName)
		{
			var type = new CodeTypeDeclaration(className)
			{
				Attributes = MemberAttributes.Public,
				IsPartial = true
			};

			type.BaseTypes.Add(CreateBaseTypeReference(baseClassName));

			type.Members.Add(CreateTypeConstructor());

			return type;
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

		private void CreateAndAddProperty(PropertyDefinition property, CodeTypeDeclaration type)
		{
			var codeProp = new CodeMemberProperty();
			var typeName = configuration.GetTypeName(property);
			codeProp.Name = property.Name.RemovePrefix(configuration.RemovePrefix).PascalCase();
			codeProp.Type = new CodeTypeReference(typeName);
			codeProp.Attributes = MemberAttributes.Public | MemberAttributes.Final;

			var contentRef = new CodePropertyReferenceExpression(null, "Content");
			var getPropertyValueMethod = new CodeMethodReferenceExpression(contentRef, "GetPropertyValue", codeProp.Type);
			var getPropertyValueCall = new CodeMethodInvokeExpression(getPropertyValueMethod, new CodePrimitiveExpression(property.Name));

			codeProp.GetStatements.Add(new CodeMethodReturnStatement(getPropertyValueCall));

			type.Members.Add(codeProp);
		}
	}
}