using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;

namespace Umbraco.CodeGen
{
	public class BaseClassBuilder : INamespaceMemberBuilder
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
			if (String.IsNullOrEmpty(configuration.BaseClass))
			{
				var baseType = CreateBaseType();
				ns.Types.Add(baseType);
				configuration.BaseClass = baseType.Name;
			}
		}

		private CodeTypeDeclaration CreateBaseType()
		{
			var baseType = new CodeTypeDeclaration("DocumentTypeBase")
			{
				IsPartial = true,
				Attributes = MemberAttributes.Public
			};
			baseType.Members.Add(CreatePublishedContentField());
			baseType.Members.Add(CreatePublishedContentGetter());
			baseType.Members.Add(CreateBaseTypeCtor());
			return baseType;
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
						"content"
						),
					new CodeVariableReferenceExpression("content")
					)
				);
			return ctor;
		}

		private CodeTypeMember CreatePublishedContentField()
		{
			return new CodeMemberField("IPublishedContent", "content");
		}

		private CodeTypeMember CreatePublishedContentGetter()
		{
			var prop = new CodeMemberProperty
			{
				Name = "Content",
				Attributes = MemberAttributes.Public | MemberAttributes.Final,
				Type = new CodeTypeReference("IPublishedContent")
			};
			prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(null, "content")));
			return prop;

		}

		private static CodeMethodInvokeExpression CreateReplaceCall(CodeExpression stringReference, CodeExpression removeValue)
		{
			var replaceGetCall = new CodeMethodInvokeExpression(stringReference, "Replace");
			replaceGetCall.Parameters.Add(removeValue);
			replaceGetCall.Parameters.Add(new CodePrimitiveExpression(""));
			return replaceGetCall;
		}
	}
}