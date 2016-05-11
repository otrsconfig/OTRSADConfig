using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace OTRS_AD_Script_Creator.Properties
{
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0"), CompilerGenerated]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string HostAddress
		{
			get
			{
				return (string)this["HostAddress"];
			}
			set
			{
				this["HostAddress"] = value;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string BaseDNSearch
		{
			get
			{
				return (string)this["BaseDNSearch"];
			}
			set
			{
				this["BaseDNSearch"] = value;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string UserAttributeSearch
		{
			get
			{
				return (string)this["UserAttributeSearch"];
			}
			set
			{
				this["UserAttributeSearch"] = value;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string SeachUserDN
		{
			get
			{
				return (string)this["SeachUserDN"];
			}
			set
			{
				this["SeachUserDN"] = value;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string SearchUserPassword
		{
			get
			{
				return (string)this["SearchUserPassword"];
			}
			set
			{
				this["SearchUserPassword"] = value;
			}
		}

		[DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
		public bool AnonymousAllowed
		{
			get
			{
				return (bool)this["AnonymousAllowed"];
			}
			set
			{
				this["AnonymousAllowed"] = value;
			}
		}

		private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
		{
		}

		private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
		{
		}
	}
}
