using System;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using Umbraco.CodeGen.Generators;
using Umbraco.Core.Logging;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Web.WebApi;

namespace Umbraco.CodeGen.Integration.Api
{
    [PluginController("CodeGen")]
    public class PreviewController : UmbracoApiController
    {
        private readonly ContentTypeSerializer serializer = new ContentTypeSerializer();

        public CodeDto GetPreview(int id)
        {
            var docType = ApplicationContext.Services.ContentTypeService.GetAllContentTypes(id).Single();
            var contentPath = ApplicationContext.Services.ContentTypeService.GetAllContentTypes(docType.Path.Split(',').Select(p => Convert.ToInt32(p)).ToArray());
            var defPath = "~/usync/" + "DocumentType/" + String.Join("/", contentPath.Select(c => c.Alias)) + "/def.config";
            var inputPath = HttpContext.Current.Server.MapPath(defPath);

            var typeConfig = inputPath.Contains("DocumentType")
                ? Integration.Configuration.CodeGen.DocumentTypes
                : Integration.Configuration.CodeGen.MediaTypes;

            Definitions.ContentType contentType;
            using (var reader = File.OpenText(inputPath))
                contentType = serializer.Deserialize(reader);

            var generatorFactory = ApplicationEvents.CreateFactory<CodeGeneratorFactory>(Integration.Configuration.CodeGen.GeneratorFactory);
            var classGenerator = new CodeGenerator(typeConfig, Integration.Configuration.DataTypesProvider, generatorFactory);
            var builder = new StringBuilder();
            using (var stream = new StringWriter(builder))
                classGenerator.Generate(contentType, stream);

            return new CodeDto {Name = docType.Name, Code = builder.ToString()};
        }

        public static void RegisterMenu()
        {
            TreeControllerBase.MenuRendering += AddPreviewMenuItem;
        }

        private static void AddPreviewMenuItem(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            var section = e.QueryStrings.Get("section");
            var treeType = e.QueryStrings.Get("treeType");
            var isDocumentType = section == "settings" && (treeType == "nodeTypes" || treeType == "mediaTypes");
            if (!isDocumentType || e.NodeId == "-1")
                return;

            var insertAfter = e.Menu.Items.FirstOrDefault(extItem => extItem.Alias == "exportDocumentType");

              //creates a menu action that will open /umbraco/currentSection/itemAlias.html
            var i = new MenuItem("previewModelClass", "Preview CodeGen class");
            i.AdditionalData.Add("actionRoute", "settings/codegen/codegen.preview/" + e.NodeId);
            i.Icon = "brackets";

            if (insertAfter != null)
                e.Menu.Items.Insert(e.Menu.Items.IndexOf(insertAfter) + 1, i);
            else
                e.Menu.Items.Add(i);
        }
    }

    public class CodeDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    [Tree("developer", "codegen", "CodeGen", "icon-brackets", "icon-brackets", true, 200)]
    [PluginController("CodeGen")]
    public class PreviewTreeController : TreeController
    {
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            return new TreeNodeCollection();
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            return new MenuItemCollection();
        }

        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var node = CreateTreeNode(Guid.NewGuid().ToString("N"), "", queryStrings, "CodeGen", "icon-brackets", "developer/codegen/codegen.configuration/null");
            return node;
        }
    }
}
