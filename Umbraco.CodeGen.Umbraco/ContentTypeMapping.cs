using System;
using System.Linq;
using Umbraco.CodeGen.Definitions;
using Umbraco.Core.Models;
using ContentType = Umbraco.CodeGen.Definitions.ContentType;

namespace Umbraco.CodeGen.Umbraco
{
    public class ContentTypeMapping
    {
        public static ContentType Map(IContentTypeBase umbracoContentType)
        {
            if (umbracoContentType is IMediaType)
                throw new NotImplementedException();

            return MapDocumentType(umbracoContentType);
        }

        private static ContentType MapDocumentType(IContentTypeBase contentType)
        {
            var type = new DocumentType();
            var umbracoContentType = (IContentType)contentType;

            MapContentTypeBase(contentType, type);
            MapDocumentTypeInfo(umbracoContentType, type.Info);
            MapComposition(umbracoContentType, type);
            
            return type;
        }

        private static ContentType MapContentTypeBase(IContentTypeBase contentTypeBase)
        {
            var type = new ContentType();
            MapContentTypeBase(contentTypeBase, type);
            return type;
        }

        private static void MapContentTypeBase(IContentTypeBase contentTypeBase, ContentType type)
        {
            MapInfo(contentTypeBase, type.Info);
            MapStructure(contentTypeBase, type);
            MapGenericProperties(contentTypeBase, type);
            MapTabs(contentTypeBase, type);
        }

        private static void MapComposition(IContentType umbracoContentType, ContentType type)
        {
            type.Composition = umbracoContentType.ContentTypeComposition
                .Where(cmp => cmp.Id != umbracoContentType.ParentId)
                .Select(MapContentTypeBase)
                .ToList();
        }

        private static void MapInfo(IContentTypeBase umbracoContentType, Info info)
        {
            info.Alias = umbracoContentType.Alias;
            info.AllowAtRoot = umbracoContentType.AllowedAsRoot;
            info.Description = umbracoContentType.Description;
            info.Icon = umbracoContentType.Icon;
            info.Name = umbracoContentType.Name;
            info.Thumbnail = umbracoContentType.Thumbnail;
        }

        private static void MapStructure(IContentTypeBase contentTypeBase, ContentType type)
        {
            type.Structure = contentTypeBase.AllowedContentTypes.Select(ct => ct.Alias).ToList();
        }

        private static void MapGenericProperties(IContentTypeBase contentTypeBase, ContentType type)
        {
            type.GenericProperties = contentTypeBase.PropertyGroups.SelectMany(pg =>
                pg.PropertyTypes.Select(p =>
                    new GenericProperty
                    {
                        Alias = p.Alias,
                        Description = p.Description,
                        Mandatory = p.Mandatory,
                        Name = p.Name,
                        Tab = pg.Name,
                        Type = p.PropertyEditorAlias,
                        Validation = p.ValidationRegExp
                    }
                    )
                ).ToList();
        }

        private static void MapTabs(IContentTypeBase contentTypeBase, ContentType type)
        {
            type.Tabs = contentTypeBase.PropertyGroups.Select(pg => new Tab {Caption = pg.Name}).ToList();
        }

        private static void MapDocumentTypeInfo(IContentType umbracoContentType, Info info)
        {
            if (!(info is DocumentTypeInfo)) throw new Exception("info is not of type DocumentTypeInfo");
            MapDocumentTypeInfo(umbracoContentType, (DocumentTypeInfo)info);
        }

        private static void MapDocumentTypeInfo(IContentType umbracoContentType, DocumentTypeInfo info)
        {
            MapInfo(umbracoContentType, info);
            info.AllowedTemplates = umbracoContentType.AllowedTemplates.Select(t => t.Alias).ToList();
            info.DefaultTemplate = umbracoContentType.DefaultTemplate != null ? umbracoContentType.DefaultTemplate.Alias : null;
            info.Master = umbracoContentType.ParentId > -1
                        ? umbracoContentType.ContentTypeComposition.First(cmp => cmp.Id == umbracoContentType.ParentId).Alias
                        : "";
        }
    }
}
