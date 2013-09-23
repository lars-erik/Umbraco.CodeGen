using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.CodeGen
{
	[Guid("0FECB64A-8779-4A7B-B7CD-226DD6531FB1")]
	public abstract class VsContextGuids
	{
		[MarshalAs(UnmanagedType.LPStr)]
		public const string VsContextGuidVCSProject = "{FAE04EC1-301F-11D3-BF4B-00C04F79EFBC}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string VsContextGuidVCSEditor = "{694DD9B6-B865-4C5B-AD85-86356E9C88DC}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string VsContextGuidVBProject = "{164B10B9-B200-11D0-8C61-00A0C91E29D5}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string VsContextGuidVBEditor = "{E34ACDC0-BAAE-11D0-88BF-00A0C9110049}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string VsContextGuidVJSProject = "{E6FDF8B0-F3D1-11D4-8576-0002A516ECE8}";
		[MarshalAs(UnmanagedType.LPStr)]
		public const string VsContextGuidVJSEditor = "{E6FDF88A-F3D1-11D4-8576-0002A516ECE8}";
	}
}
