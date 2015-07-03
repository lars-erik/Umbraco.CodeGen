using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Umbraco
{
    public class ContentTypeMapping
    {
        public static ContentType Map(Core.Models.IContentType umbct)
        {
            var type = new DocumentType
            {
                Info = new DocumentTypeInfo
                {
                    Alias = umbct.Alias,
                    AllowAtRoot = umbct.AllowedAsRoot,
                    AllowedTemplates = umbct.AllowedTemplates.Select(t => t.Alias).ToList(),
                    DefaultTemplate = umbct.DefaultTemplate.Alias,
                    Description = umbct.Description,
                    Icon = umbct.Icon,
                    Master = umbct.ParentId > -1
                        ? umbct.ContentTypeComposition.First(cmp => cmp.Id == umbct.ParentId).Alias
                        : "",
                    Name = umbct.Name,
                    Thumbnail = umbct.Thumbnail
                },
                Structure = umbct.AllowedContentTypes.Select(ct => ct.Alias).ToList(),
                GenericProperties = umbct.PropertyGroups.SelectMany(pg =>
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
                    ).ToList(),
                Tabs = umbct.PropertyGroups.Select(pg => new Tab {Caption = pg.Name}).ToList()
            };
            return type;
        }
    }
}
