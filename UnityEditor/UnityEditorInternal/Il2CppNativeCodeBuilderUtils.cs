using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnityEditorInternal
{
	public static class Il2CppNativeCodeBuilderUtils
	{
		public static IEnumerable<string> AddBuilderArguments(Il2CppNativeCodeBuilder builder, string outputRelativePath, IEnumerable<string> includeRelativePaths)
		{
			List<string> list = new List<string>();
			list.Add("--compile-cpp");
			list.Add("--libil2cpp-static");
			list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("platform", builder.CompilerPlatform));
			list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("architecture", builder.CompilerArchitecture));
			list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("configuration", "Release"));
			list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("outputpath", builder.ConvertOutputFileToFullPath(outputRelativePath)));
			if (!string.IsNullOrEmpty(builder.CacheDirectory))
			{
				list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("cachedirectory", Il2CppNativeCodeBuilderUtils.CacheDirectoryPathFor(builder.CacheDirectory)));
			}
			if (!string.IsNullOrEmpty(builder.CompilerFlags))
			{
				list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("compiler-flags", builder.CompilerFlags));
			}
			if (!string.IsNullOrEmpty(builder.LinkerFlags))
			{
				list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("linker-flags", builder.LinkerFlags));
			}
			if (!string.IsNullOrEmpty(builder.PluginPath))
			{
				list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("plugin", builder.PluginPath));
			}
			foreach (string current in builder.ConvertIncludesToFullPaths(includeRelativePaths.Concat(new string[]
			{
				outputRelativePath
			})))
			{
				list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("additional-include-directories", current));
			}
			return list;
		}

		public static void ClearCacheIfEditorVersionDiffers(Il2CppNativeCodeBuilder builder, string currentEditorVersion)
		{
			string text = Il2CppNativeCodeBuilderUtils.CacheDirectoryPathFor(builder.CacheDirectory);
			if (Directory.Exists(text) && !File.Exists(Path.Combine(text, currentEditorVersion)))
			{
				Directory.Delete(text, true);
			}
		}

		public static void PrepareCacheDirectory(Il2CppNativeCodeBuilder builder, string currentEditorVersion)
		{
			string text = Il2CppNativeCodeBuilderUtils.CacheDirectoryPathFor(builder.CacheDirectory);
			Directory.CreateDirectory(text);
			string path = Path.Combine(text, currentEditorVersion);
			if (!File.Exists(path))
			{
				File.Create(path).Dispose();
			}
		}

		public static string ObjectFilePathInCacheDirectoryFor(string builderCacheDirectory)
		{
			return Il2CppNativeCodeBuilderUtils.CacheDirectoryPathFor(builderCacheDirectory) + "/objectfiles";
		}

		private static string CacheDirectoryPathFor(string builderCacheDirectory)
		{
			return builderCacheDirectory + "/il2cpp_cache";
		}

		private static string FormatArgument(string name, string value)
		{
			return string.Format("--{0}=\"{1}\"", name, value);
		}
	}
}
