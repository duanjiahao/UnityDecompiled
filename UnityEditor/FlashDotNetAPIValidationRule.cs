using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
[AssemblyValidationRule(RuntimePlatform.FlashPlayer)]
internal class FlashDotNetAPIValidationRule : GendarmeValidationRule
{
	private readonly string _configFilePath;
	private readonly FlashFileHelper _fileHelper;
	private readonly FlashPostProcessSettings _settings;
	public FlashDotNetAPIValidationRule(PostProcessFlashPlayerOptions options) : base(FlashFileHelper.CreateFrom(options).PathForGendarmeExe())
	{
		this._fileHelper = FlashFileHelper.CreateFrom(options);
		this._settings = FlashPostProcessSettings.CreateFrom(options);
		this._configFilePath = Path.GetTempFileName();
	}
	~FlashDotNetAPIValidationRule()
	{
		File.Delete(this._configFilePath);
	}
	protected override GendarmeOptions ConfigureGendarme(IEnumerable<string> userAssemblies)
	{
		string[] array = (userAssemblies as string[]) ?? userAssemblies.ToArray<string>();
		if (array.Length != 0)
		{
			this.UpdateRulesTemplate();
		}
		return new GendarmeOptions
		{
			ConfigFilePath = this._configFilePath,
			RuleSet = "UnityFlash",
			UserAssemblies = array
		};
	}
	private void UpdateRulesTemplate()
	{
		GendarmeXMLTemplateTags gendarmeXMLTemplateTags = new GendarmeXMLTemplateTags();
		gendarmeXMLTemplateTags.LimitToNamespaces.Add("System");
		gendarmeXMLTemplateTags.WhiteListFiles.Add(this._fileHelper.PathForFileInCil2As("gendarme-whitelist.txt"));
		FlashDotNetAPIValidationRule.AddHardcodedWhiteListTypes(gendarmeXMLTemplateTags);
		FlashTemplateWriterUtility.WriteGendarmeXMLTemplate(this._settings, this._fileHelper, gendarmeXMLTemplateTags, this._configFilePath);
	}
	private static void AddHardcodedWhiteListTypes(GendarmeXMLTemplateTags gendarmeXMLTemplateTags)
	{
		string[] collection = new string[]
		{
			"System.Array",
			"System.Enum",
			"System.RuntimeTypeHandle",
			"System.IFormattable",
			"System.ValueType",
			"System.Delegate",
			"System.MulticastDelegate",
			"System.Runtime.Serialization.ISerializable",
			"System.Runtime.Serialization.IDeserializationCallback",
			"System.RuntimeFieldHandle",
			"System.Runtime.CompilerServices.RuntimeHelpers",
			"System.Math",
			"System.Func",
			"System.Func`1",
			"System.Func`2",
			"System.Func`3",
			"System.Func`4",
			"System.Action",
			"System.Action`1",
			"System.Action`2",
			"System.Action`3",
			"System.Action`4",
			"System.Predicate`1",
			"System.Runtime.InteropServices._Exception",
			"System.Runtime.InteropServices._Attribute",
			"System.Comparison`1",
			"System.AsyncCallback",
			"System.AttributeTargets",
			"System.StringSplitOptions",
			"System.EventHandler`1",
			"System.EventHandler"
		};
		gendarmeXMLTemplateTags.WhiteListTypes.AddRange(collection);
	}
}
