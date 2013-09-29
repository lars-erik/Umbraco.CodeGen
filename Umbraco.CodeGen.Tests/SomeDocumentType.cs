namespace MyWeb.Models
{
	using System;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;	
	using Umbraco.Core.Models;

	[DisplayName("Some document type")]
	[Description("A description of some document type")]
	public class SomeDocumentType : DocumentTypeBase
	{
		const string icon = "privateMemberIcon.gif";
		const string thumbnail = "privateMemberThumb.png";
		const bool allowAtRoot = true;
		readonly string[] allowedTemplates = new[] { "ATemplate", "AnotherTemplate" };
		const string defaultTemplate = "ATemplate";
		readonly Type[] structure = new[] { typeof(SomeOtherDocType) };

		[DisplayName("Some property")]
		[Description("A description")]
		[Category("A tab")]
		[DataType("BBBEB697-D751-4A19-8ACE-3A05DE2EEEF6")]
		[RegularExpression("[a-z]")]
		public string SomeProperty
		{
			get
			{
				return Content.GetPropertyValue<string>("someProperty");
			}
		}
		[DisplayName("Another property")]
		[Description("Another description")]
		[Category("A tab")]
		[DataType("BBBEB697-D751-4A19-8ACE-3A05DE2EEEF6")]
		public string AnotherProperty
		{
			get
			{
				return Content.GetPropertyValue<string>("anotherProperty");
			}
		}
	}
}
