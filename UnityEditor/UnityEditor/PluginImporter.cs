using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Modules;
using UnityEditorInternal;

namespace UnityEditor
{
	public sealed class PluginImporter : AssetImporter
	{
		public extern bool isNativePlugin
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern DllType dllType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		private static bool IsCompatible(PluginImporter imp, string platformName)
		{
			return !string.IsNullOrEmpty(imp.assetPath) && (imp.GetCompatibleWithPlatform(platformName) || (imp.GetCompatibleWithAnyPlatform() && !imp.GetExcludeFromAnyPlatform(platformName))) && imp.ShouldIncludeInBuild();
		}

		private static bool IsCompatible(PluginImporter imp, string buildTargetGroup, string buildTarget)
		{
			return !string.IsNullOrEmpty(imp.assetPath) && (imp.GetCompatibleWithPlatform(buildTargetGroup, buildTarget) || imp.GetCompatibleWithAnyPlatform());
		}

		public static PluginImporter[] GetImporters(string platformName)
		{
			List<PluginImporter> list = new List<PluginImporter>();
			Dictionary<string, PluginImporter> dictionary = new Dictionary<string, PluginImporter>();
			PluginImporter[] array = (from imp in PluginImporter.GetAllImporters()
			where PluginImporter.IsCompatible(imp, platformName)
			select imp).ToArray<PluginImporter>();
			IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(platformName);
			if (pluginImporterExtension == null)
			{
				pluginImporterExtension = ModuleManager.GetPluginImporterExtension(BuildPipeline.GetBuildTargetByName(platformName));
			}
			PluginImporter[] result;
			if (pluginImporterExtension == null)
			{
				result = array;
			}
			else
			{
				int i = 0;
				while (i < array.Length)
				{
					PluginImporter pluginImporter = array[i];
					string text = pluginImporterExtension.CalculateFinalPluginPath(platformName, pluginImporter);
					if (!string.IsNullOrEmpty(text))
					{
						PluginImporter pluginImporter2;
						if (!dictionary.TryGetValue(text, out pluginImporter2))
						{
							dictionary.Add(text, pluginImporter);
						}
						else if (pluginImporter2.GetIsOverridable() && !pluginImporter.GetIsOverridable())
						{
							dictionary[text] = pluginImporter;
							list.Remove(pluginImporter2);
						}
						else if (pluginImporter.GetIsOverridable())
						{
							goto IL_106;
						}
						goto IL_FD;
					}
					goto IL_FD;
					IL_106:
					i++;
					continue;
					IL_FD:
					list.Add(pluginImporter);
					goto IL_106;
				}
				result = list.ToArray();
			}
			return result;
		}

		public static PluginImporter[] GetImporters(BuildTarget platform)
		{
			return PluginImporter.GetImporters(BuildPipeline.GetBuildTargetName(platform));
		}

		public static PluginImporter[] GetImporters(string buildTargetGroup, string buildTarget)
		{
			return (from imp in PluginImporter.GetAllImporters()
			where PluginImporter.IsCompatible(imp, buildTargetGroup, buildTarget)
			select imp).ToArray<PluginImporter>();
		}

		public static PluginImporter[] GetImporters(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
		{
			return PluginImporter.GetImporters(BuildPipeline.GetBuildTargetGroupName(buildTargetGroup), BuildPipeline.GetBuildTargetName(buildTarget));
		}

		[DebuggerHidden]
		internal static IEnumerable<PluginDesc> GetExtensionPlugins(BuildTarget target)
		{
			PluginImporter.<GetExtensionPlugins>c__Iterator0 <GetExtensionPlugins>c__Iterator = new PluginImporter.<GetExtensionPlugins>c__Iterator0();
			<GetExtensionPlugins>c__Iterator.target = target;
			PluginImporter.<GetExtensionPlugins>c__Iterator0 expr_0E = <GetExtensionPlugins>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearSettings();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCompatibleWithAnyPlatform(bool enable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetCompatibleWithAnyPlatform();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetExcludeFromAnyPlatform(string platformName, bool excludedFromAny);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetExcludeFromAnyPlatform(string platformName);

		public void SetExcludeFromAnyPlatform(BuildTarget platform, bool excludedFromAny)
		{
			this.SetExcludeFromAnyPlatform(BuildPipeline.GetBuildTargetName(platform), excludedFromAny);
		}

		public bool GetExcludeFromAnyPlatform(BuildTarget platform)
		{
			return this.GetExcludeFromAnyPlatform(BuildPipeline.GetBuildTargetName(platform));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetExcludeEditorFromAnyPlatform(bool excludedFromAny);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetExcludeEditorFromAnyPlatform();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCompatibleWithEditor(bool enable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetCompatibleWithEditorWithBuildTargetsInternal(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, bool enable);

		internal void SetCompatibleWithEditor(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, bool enable)
		{
			this.SetCompatibleWithEditorWithBuildTargetsInternal(buildTargetGroup, buildTarget, enable);
		}

		public bool GetCompatibleWithEditor()
		{
			return this.GetCompatibleWithEditor("", "");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetCompatibleWithEditor(string buildTargetGroup, string buildTarget);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetIsPreloaded(bool isPreloaded);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool GetIsPreloaded();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetIsOverridable();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ShouldIncludeInBuild();

		public void SetCompatibleWithPlatform(BuildTarget platform, bool enable)
		{
			this.SetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(platform), enable);
		}

		internal void SetCompatibleWithPlatform(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, bool enable)
		{
			this.SetCompatibleWithPlatform(BuildPipeline.GetBuildTargetGroupName(buildTargetGroup), BuildPipeline.GetBuildTargetName(buildTarget), enable);
		}

		public bool GetCompatibleWithPlatform(BuildTarget platform)
		{
			return this.GetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(platform));
		}

		internal bool GetCompatibleWithPlatform(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
		{
			return this.GetCompatibleWithPlatform(BuildPipeline.GetBuildTargetGroupName(buildTargetGroup), BuildPipeline.GetBuildTargetName(buildTarget));
		}

		public void SetCompatibleWithPlatform(string platformName, bool enable)
		{
			this.SetCompatibleWithPlatform(BuildPipeline.GetBuildTargetGroupName(BuildPipeline.GetBuildTargetByName(platformName)), platformName, enable);
		}

		public bool GetCompatibleWithPlatform(string platformName)
		{
			return this.GetCompatibleWithPlatform(BuildPipeline.GetBuildTargetGroupName(BuildPipeline.GetBuildTargetByName(platformName)), platformName);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetCompatibleWithPlatform(string buildTargetGroup, string buildTarget, bool enable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool GetCompatibleWithPlatform(string buildTargetGroup, string buildTarget);

		public void SetPlatformData(BuildTarget platform, string key, string value)
		{
			this.SetPlatformData(BuildPipeline.GetBuildTargetName(platform), key, value);
		}

		public string GetPlatformData(BuildTarget platform, string key)
		{
			return this.GetPlatformData(BuildPipeline.GetBuildTargetName(platform), key);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPlatformData(string platformName, string key, string value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetPlatformData(string platformName, string key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetEditorData(string key, string value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetEditorData(string key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern PluginImporter[] GetAllImporters();
	}
}
