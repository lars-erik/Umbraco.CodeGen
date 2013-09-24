using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.CodeGen
{
	public class ContentTypeCodeGenerator
	{
		private readonly IEnumerable<ContentTypeDefinition> contentTypes;
		private readonly CodeDomProvider codeDomProvider;
		private readonly string nameSpace;
		private readonly CodeGeneratorConfiguration configuration;

		private CodeCompileUnit compileUnit;
		private CodeNamespace ns;
		private CodeTypeReference baseType;

		public ContentTypeCodeGenerator(
			CodeGeneratorConfiguration configuration, 
			IEnumerable<ContentTypeDefinition> contentTypes,
			CodeDomProvider codeDomProvider,
			string nameSpace
			)
		{
			this.configuration = configuration;
			this.contentTypes = contentTypes;
			this.codeDomProvider = codeDomProvider;
			this.nameSpace = nameSpace;
		}

		public void BuildCode(StringWriter writer)
		{
			CreateCompileUnit();
			CreateAndAddNamespace();
			CreateAndAddImports();

			if (String.IsNullOrEmpty(configuration.CustomBaseClass))
				baseType = CreateAndAddBaseType();
			else
				baseType = CreateAndCustomBaseTypeRef();

			foreach (var contentType in this.contentTypes)
				CreateAndAddType(contentType);

			WriteCode(writer, compileUnit);
			writer.Flush();
		}

		private CodeTypeReference CreateAndCustomBaseTypeRef()
		{
			return new CodeTypeReference(configuration.CustomBaseClass);
		}

		private void CreateAndAddType(ContentTypeDefinition definition)
		{

			var type = CreateType(definition.ClassName, definition.BaseClassName);
			foreach (var property in definition.Properties)
				CreateAndAddProperty(property, type);

			ns.Types.Add(type);
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
				baseClassName = baseClassName.RemovePrefix(configuration.RemovePrefix);
				type.BaseTypes.Add(baseClassName);
			}
			else
			{
				type.BaseTypes.Add(baseType);
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

		private void CreateAndAddProperty(PropertyDefinition property, CodeTypeDeclaration type)
		{
			var codeProp = new CodeMemberProperty();
			var typeName = configuration.GetTypeName(property);
			codeProp.Name = property.Name;
			codeProp.Type = new CodeTypeReference(typeName);
			codeProp.Attributes = MemberAttributes.Public | MemberAttributes.Final;

			var getValueRef = new CodeMethodReferenceExpression(null, "GetValue");
			var getValueCall = new CodeMethodInvokeExpression(getValueRef);
			getValueCall.Parameters.Add(new CodeMethodInvokeExpression(null, "ThisProperty"));
			var castedValue = new CodeCastExpression(typeName, getValueCall);
			codeProp.GetStatements.Add(new CodeMethodReturnStatement(castedValue));

			type.Members.Add(codeProp);
		}

		private CodeTypeReference CreateAndAddBaseType()
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
			baseType.Members.Add(CreateAddPrefixMethod());
			baseType.Members.Add(CreateRemoveGetAndPrefixMethod());
			baseType.Members.Add(CreateGetValueMethod());
			ns.Types.Add(baseType);
			return new CodeTypeReference(baseType.Name);
		}

		private CodeTypeMember CreateRemovePrefixConst()
		{
			return new CodeMemberField("String", "RemovePrefix")
			{
				Attributes = MemberAttributes.Const | MemberAttributes.Private,
				InitExpression = new CodePrimitiveExpression(configuration.RemovePrefix)
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

			var invokeRemoveGetStatement = new CodeMethodInvokeExpression(null, "AddPrefix");
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

		private static CodeMemberMethod CreateRemoveGetAndPrefixMethod()
		{
			var removeGetMethod = new CodeMemberMethod
			{
				Name = "RemovePrefixes",
				Attributes = MemberAttributes.Private,
				ReturnType = new CodeTypeReference(typeof(string)),
			};

			removeGetMethod.Parameters.Add(
				new CodeParameterDeclarationExpression(typeof(MethodBase), "m")
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

		private CodeTypeMember CreateAddPrefixMethod()
		{
			var addPrefixMethod = new CodeMemberMethod
			{
				Name = "AddPrefix",
				Attributes = MemberAttributes.Private,
				ReturnType = new CodeTypeReference(typeof (string))
			};
			addPrefixMethod.Parameters.Add(
				new CodeParameterDeclarationExpression(typeof (MethodBase), "m")
				);
			var methodNameReference = new CodePropertyReferenceExpression(
				new CodeVariableReferenceExpression("m"),
				"Name");

			var replaceGetCall = CreateReplaceCall(methodNameReference, new CodePrimitiveExpression("_get"));

			var concatCall = new CodeMethodInvokeExpression(
				new CodeTypeReferenceExpression("String"),
				"Concat",
				new CodeFieldReferenceExpression(null, "RemovePrefix"),
				replaceGetCall
				);

			addPrefixMethod.Statements.Add(new CodeMethodReturnStatement(concatCall));
			return addPrefixMethod;
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
			ns = new CodeNamespace(nameSpace);
			compileUnit.Namespaces.Add(ns);
		}

		private void CreateCompileUnit()
		{
			compileUnit = new CodeCompileUnit();
		}

		private void WriteCode(StringWriter writer, CodeCompileUnit compileUnit)
		{
			var options = new CodeGeneratorOptions
			{
				BlankLinesBetweenMembers = false,
				BracingStyle = "C"
			};
			codeDomProvider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
		}
	}
}
