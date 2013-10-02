namespace MyWeb.Models
{
	using System;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;	
	using Umbraco.Core.Models;
	using Umbraco.Web;

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
		[DataType("ca90c950-0aff-4e72-b976-a30b1ac57dad")]
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
		[DataType("ca90c950-0aff-4e72-b976-a30b1ac57dad")]
		public string AnotherProperty
		{
			get
			{
				return Content.GetPropertyValue<string>("anotherProperty");
			}
		}
	}
}
