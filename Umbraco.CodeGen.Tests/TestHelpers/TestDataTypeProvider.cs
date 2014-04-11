using System.Collections.Generic;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Tests.TestHelpers
{
    public class TestDataTypeProvider : IDataTypeProvider
    {
        public static readonly DataTypeDefinition ApprovedColor = new DataTypeDefinition("Approved Color","Umbraco.ColorPickerAlias","0225af17-b302-49cb-9176-b9f35cab9c17", "305ABD5E-09B5-4DBC-965A-45D8C67617CC");
        public static readonly DataTypeDefinition Checkboxlist = new DataTypeDefinition("Checkbox list","Umbraco.CheckBoxList","fbaf13a8-4036-41f2-93a3-974f678c312a", "B7E0A206-3A40-490C-9A4E-3744FE000B5D");
        public static readonly DataTypeDefinition ContentPicker = new DataTypeDefinition("Content Picker","Umbraco.ContentPickerAlias","a6857c73-d6e9-480c-b6e6-f15f6ad11125", "14D41294-C484-4C85-8431-67857A45E6E6");
        public static readonly DataTypeDefinition DatePickerwithtime = new DataTypeDefinition("Date Picker with time","Umbraco.DateTime","e4d66c0f-b935-4200-81f0-025f7256b89a", "823D6E44-2860-4F32-8A9E-BC575F5144D1");
        public static readonly DataTypeDefinition DatePicker = new DataTypeDefinition("Date Picker","Umbraco.Date","5046194e-4237-453c-a547-15db3a07c4e1", "004480F6-65E7-46F0-9A05-6A89A722A54A");
        public static readonly DataTypeDefinition Dropdownmultiple = new DataTypeDefinition("Dropdown multiple","Umbraco.DropDownMultiple","f38f0ac7-1d27-439c-9f3f-089cd8825a53", "3A47D9B5-9C2D-4382-971B-A7A8849D7EDD");
        public static readonly DataTypeDefinition Dropdown = new DataTypeDefinition("Dropdown","Umbraco.DropDown","0b6a45e7-44ba-430d-9da5-4e46060b9e03", "C8544621-26A7-495C-BE65-A8A410DA5121");
        public static readonly DataTypeDefinition FolderBrowser = new DataTypeDefinition("Folder Browser","Umbraco.FolderBrowser","fd9f1447-6c61-4a7c-9595-5aa39147d318", "0FAB501F-4741-402B-B0BA-B4567A2BF560");
        public static readonly DataTypeDefinition Label = new DataTypeDefinition("Label","Umbraco.NoEdit","f0bc4bfb-b499-40d6-ba86-058885a5178c", "A6C2F562-BA6A-4B29-B88B-9416F115B5B4");
        public static readonly DataTypeDefinition MediaPicker = new DataTypeDefinition("Media Picker","Umbraco.MediaPicker","93929b9a-93a2-4e2a-b239-d99334440a59", "DAA82F94-2E12-40A5-B28A-3613A0B9FF5B");
        public static readonly DataTypeDefinition MemberPicker = new DataTypeDefinition("Member Picker","Umbraco.MemberPicker","2b24165f-9782-4aa3-b459-1de4a4d21f60", "C33F5C6A-AFB3-48E0-9A07-9D61765F56B9");
        public static readonly DataTypeDefinition MultipleMediaPicker = new DataTypeDefinition("Multiple Media Picker","Umbraco.MultipleMediaPicker","7e3962cc-ce20-4ffc-b661-5897a894ba7e", "AA7F6001-0276-4815-AA63-D3AAEAAA1AD6");
        public static readonly DataTypeDefinition Numeric = new DataTypeDefinition("Numeric","Umbraco.Integer","2e6d3631-066e-44b8-aec4-96f09099b2b5", "9E28F2CA-AF08-4E4B-A3D2-7231FAB4BAB6");
        public static readonly DataTypeDefinition Radiobox = new DataTypeDefinition("Radiobox","Umbraco.RadioButtonList","bb5f57c9-ce2b-4bb9-b697-4caca783a805", "637FA886-D406-40CC-9927-D5393DD975A8");
        public static readonly DataTypeDefinition RelatedLinks = new DataTypeDefinition("Related Links","Umbraco.RelatedLinks","21e798da-e06e-4eda-a511-ed257f78d4fa", "BE333AA9-9E60-464F-A9C3-6C9B7DCB2F5F");
        public static readonly DataTypeDefinition Richtexteditor = new DataTypeDefinition("Richtext editor","Umbraco.TinyMCEv3","ca90c950-0aff-4e72-b976-a30b1ac57dad", "E78FA592-86D6-4AC6-A3D5-9DFF8624FF0A");
        public static readonly DataTypeDefinition Tags = new DataTypeDefinition("Tags","Umbraco.Tags","b6b73142-b9c1-4bf8-a16d-e1c23320b549", "141A655E-18DE-4AB0-8C47-1C09E485A3B2");
        public static readonly DataTypeDefinition Textboxmultiple = new DataTypeDefinition("Textbox multiple","Umbraco.TextboxMultiple","c6bac0dd-4ab9-45b1-8e30-e4b619ee5da3", "68E1FB26-FF31-4E58-9470-91F6A20266D1");
        public static readonly DataTypeDefinition Textstring = new DataTypeDefinition("Textstring","Umbraco.Textbox","0cc0eba1-9960-42c9-bf9b-60e150b429ae", "76FD82CF-8AC3-4FF7-9E88-D0A8539AE109");
        public static readonly DataTypeDefinition Truefalse = new DataTypeDefinition("True/false","Umbraco.TrueFalse","92897bc6-a5f3-4ffe-ae27-f2e7e33dda49", "7F5FA60C-6EA5-4766-8968-3FC8E7807335");
        public static readonly DataTypeDefinition Upload = new DataTypeDefinition("Upload","Umbraco.UploadField","84c6b441-31df-4ffe-b67e-67d5bc3ae65a", "7F576C2F-C360-448D-A2FC-623AFC7686CD");
        public static readonly List<DataTypeDefinition> All = new List<DataTypeDefinition>{Richtexteditor, Textstring, Numeric};

        public IEnumerable<DataTypeDefinition> GetDataTypes()
        {
            return All;
        }
    }
}