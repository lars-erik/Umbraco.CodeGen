using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Microsoft.CSharp;

static internal class CodeGenerationHelper
{
    public static CodeNamespace CreateNamespaceWithTypeAndProperty(CodeMemberProperty propNode)
    {
        var type = new CodeTypeDeclaration("AClass");
        type.Members.Add(propNode);
        return CreateNamespaceWithType(type);
    }

    public static CodeNamespace CreateNamespaceWithType(CodeTypeDeclaration type)
    {
        var ns = new CodeNamespace("ANamespace");
        ns.Types.Add(type);
        return ns;
    }

    public static StringBuilder GenerateCode(CodeNamespace ns)
    {
        var compileUnit = new CodeCompileUnit();
        var codeProvider = new CSharpCodeProvider();
        var builder = new StringBuilder();
        var writer = new StringWriter(builder);

        compileUnit.Namespaces.Add(ns);
        codeProvider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions());
        writer.Flush();
        return builder;
    }
}