using System;
using System.Collections.Generic;
using System.Web;
using Umbraco.CodeGen.Configuration;
using Umbraco.Core.Models;
using DataTypeDefinition = Umbraco.CodeGen.Configuration.DataTypeDefinition;

namespace Umbraco.CodeGen.Tests.TestHelpers
{
    public class TestDataTypeProvider : IDataTypeProvider
    {
        public static readonly DataTypeDefinition ApprovedColor = new DataTypeDefinition("Approved Color","Umbraco.ColorPickerAlias", typeof(object));
        public static readonly DataTypeDefinition Checkboxlist = new DataTypeDefinition("Checkbox list","Umbraco.CheckBoxList", typeof(IEnumerable<string>));
        public static readonly DataTypeDefinition ContentPicker = new DataTypeDefinition("Content Picker","Umbraco.ContentPickerAlias", typeof(IPublishedContent));
        public static readonly DataTypeDefinition DatePickerwithtime = new DataTypeDefinition("Date Picker with time","Umbraco.DateTime", typeof(DateTime));
        public static readonly DataTypeDefinition DatePicker = new DataTypeDefinition("Date Picker","Umbraco.Date", typeof(DateTime));
        public static readonly DataTypeDefinition Dropdownmultiple = new DataTypeDefinition("Dropdown multiple", "Umbraco.DropDownMultiple", typeof(IEnumerable<string>));
        public static readonly DataTypeDefinition Dropdown = new DataTypeDefinition("Dropdown","Umbraco.DropDown", typeof(string));
        public static readonly DataTypeDefinition FolderBrowser = new DataTypeDefinition("Folder Browser","Umbraco.FolderBrowser", typeof(object));
        public static readonly DataTypeDefinition Label = new DataTypeDefinition("Label","Umbraco.NoEdit", typeof(string));
        public static readonly DataTypeDefinition MediaPicker = new DataTypeDefinition("Media Picker","Umbraco.MediaPicker", typeof(IPublishedContent));
        public static readonly DataTypeDefinition MemberPicker = new DataTypeDefinition("Member Picker","Umbraco.MemberPicker", typeof(Member));
        public static readonly DataTypeDefinition MultipleMediaPicker = new DataTypeDefinition("Multiple Media Picker","Umbraco.MultipleMediaPicker", typeof(IEnumerable<IPublishedContent>));
        public static readonly DataTypeDefinition Numeric = new DataTypeDefinition("Numeric","Umbraco.Integer", typeof(int));
        public static readonly DataTypeDefinition Radiobox = new DataTypeDefinition("Radiobox","Umbraco.RadioButtonList", typeof(string));
        public static readonly DataTypeDefinition RelatedLinks = new DataTypeDefinition("Related Links","Umbraco.RelatedLinks", typeof(IEnumerable<string>));
        public static readonly DataTypeDefinition Richtexteditor = new DataTypeDefinition("Richtext editor","Umbraco.TinyMCEv3", typeof(IHtmlString));
        public static readonly DataTypeDefinition Tags = new DataTypeDefinition("Tags","Umbraco.Tags", typeof(List<string>));
        public static readonly DataTypeDefinition Textboxmultiple = new DataTypeDefinition("Textbox multiple","Umbraco.TextboxMultiple", typeof(string));
        public static readonly DataTypeDefinition Textstring = new DataTypeDefinition("Textstring", "Umbraco.Textbox", typeof(string));
        public static readonly DataTypeDefinition Truefalse = new DataTypeDefinition("True/false","Umbraco.TrueFalse", typeof(bool));
        public static readonly DataTypeDefinition Upload = new DataTypeDefinition("Upload","Umbraco.UploadField", typeof(string));
        public static readonly List<DataTypeDefinition> All = new List<DataTypeDefinition>{Richtexteditor, Textstring, Numeric};

        public IEnumerable<DataTypeDefinition> GetDataTypes()
        {
            return All;
        }
    }
}