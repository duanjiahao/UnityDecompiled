using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	public sealed class PluginImporter : AssetImporter
	{
		public extern bool isNativePlugin
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern DllType dllType
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCompatibleWithAnyPlatform(bool enable);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetCompatibleWithAnyPlatform();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCompatibleWithEditor(bool enable);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetCompatibleWithEditor();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetIsPreloaded(bool isPreloaded);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool GetIsPreloaded();

		public void SetCompatibleWithPlatform(BuildTarget platform, bool enable)
		{
			this.SetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(platform), enable);
		}

		public bool GetCompatibleWithPlatform(BuildTarget platform)
		{
			return this.GetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(platform));
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCompatibleWithPlatform(string platformName, bool enable);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetCompatibleWithPlatform(string platformName);

		public void SetPlatformData(BuildTarget platform, string key, string value)
		{
			this.SetPlatformData(BuildPipeline.GetBuildTargetName(platform), key, value);
		}

		public string GetPlatformData(BuildTarget platform, string key)
		{
			return this.GetPlatformData(BuildPipeline.GetBuildTargetName(platform), key);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPlatformData(string platformName, string key, string value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetPlatformData(string platformName, string key);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetEditorData(string key, string value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetEditorData(string key);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern PluginImporter[] GetAllImporters();

		private static bool IsCompatible(PluginImporter imp, string platformName)
		{
			return !string.IsNullOrEmpty(imp.assetPath) && (imp.GetCompatibleWithPlatform(platformName) || imp.GetCompatibleWithAnyPlatform());
		}

		public static PluginImporter[] GetImporters(string platformName)
		{
			return (from imp in PluginImporter.GetAllImporters()
			where PluginImporter.IsCompatible(imp, platformName)
			select imp).ToArray<PluginImporter>();
		}

		public static PluginImporter[] GetImporters(BuildTarget platform)
		{
			return PluginImporter.GetImporters(BuildPipeline.GetBuildTargetName(platform));
		}

		[DebuggerHidden]
		internal static IEnumerable<PluginDesc> GetExtensionPlugins(BuildTarget target)
		{
			PluginImporter.<GetExtensionPlugins>c__Iterator0 <GetExtensionPlugins>c__Iterator = new PluginImporter.<GetExtensionPlugins>c__Iterator0();
			<GetExtensionPlugins>c__Iterator.target = target;
			<GetExtensionPlugins>c__Iterator.<$>target = target;
			PluginImporter.<GetExtensionPlugins>c__Iterator0 expr_15 = <GetExtensionPlugins>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}
	}
}
