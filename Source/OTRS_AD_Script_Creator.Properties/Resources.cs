using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace OTRS_AD_Script_Creator.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Resources.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("OTRS_AD_Script_Creator.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = resourceManager;
				}
				return Resources.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		internal static Bitmap Next
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("Next", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap otrs_ad_pic
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("otrs_ad_pic", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap otrs_intro
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("otrs_intro", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap Previous
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("Previous", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap Search
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("Search", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap ViewPassword
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("ViewPassword", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal Resources()
		{
		}
	}
}
