using System;
using System.Collections.Generic;
using System.IO;

namespace UnityEditorInternal
{
	public static class Il2CppNativeCodeBuilderUtils
	{
		public static IEnumerable<string> AddBuilderArguments(Il2CppNativeCodeBuilder builder, string outputRelativePath, IEnumerable<string> includeRelativePaths, bool debugBuild)
		{
			List<string> list = new List<string>();
			list.Add("--compile-cpp");
			if (builder.LinkLibIl2CppStatically)
			{
				list.Add("--libil2cpp-static");
			}
			list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("platform", builder.CompilerPlatform));
			list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("architecture", builder.CompilerArchitecture));
			if (debugBuild)
			{
				list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("configuration", "Debug"));
			}
			else
			{
				list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("configuration", "Release"));
			}
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
			foreach (string current in builder.ConvertIncludesToFullPaths(includeRelativePaths))
			{
				list.Add(Il2CppNativeCodeBuilderUtils.FormatArgument("additional-include-directories", current));
			}
			list.AddRange(builder.AdditionalIl2CPPArguments);
			return list;
		}

		public static void ClearAndPrepareCacheDirectory(Il2CppNativeCodeBuilder builder)
		{
			string fullUnityVersion = InternalEditorUtility.GetFullUnityVersion();
			Il2CppNativeCodeBuilderUtils.ClearCacheIfEditorVersionDiffers(builder, fullUnityVersion);
			Il2CppNativeCodeBuilderUtils.PrepareCacheDirectory(builder, fullUnityVersion);
		}

		public static void ClearCacheIfEditorVersionDiffers(Il2CppNativeCodeBuilder builder, string currentEditorVersion)
		{
			string path = Il2CppNativeCodeBuilderUtils.CacheDirectoryPathFor(builder.CacheDirectory);
			if (Directory.Exists(path))
			{
				if (!File.Exists(Path.Combine(builder.CacheDirectory, Il2CppNativeCodeBuilderUtils.EditorVersionFilenameFor(currentEditorVersion))))
				{
					Directory.Delete(path, true);
				}
			}
		}

		public static void PrepareCacheDirectory(Il2CppNativeCodeBuilder builder, string currentEditorVersion)
		{
			string path = Il2CppNativeCodeBuilderUtils.CacheDirectoryPathFor(builder.CacheDirectory);
			Directory.CreateDirectory(path);
			string path2 = Path.Combine(builder.CacheDirectory, Il2CppNativeCodeBuilderUtils.EditorVersionFilenameFor(currentEditorVersion));
			if (!File.Exists(path2))
			{
				File.Create(path2).Dispose();
			}
		}

		public static string ObjectFilePathInCacheDirectoryFor(string builderCacheDirectory)
		{
			return Il2CppNativeCodeBuilderUtils.CacheDirectoryPathFor(builderCacheDirectory);
		}

		private static string CacheDirectoryPathFor(string builderCacheDirectory)
		{
			return builderCacheDirectory + "/il2cpp_cache";
		}

		private static string FormatArgument(string name, string value)
		{
			return string.Format("--{0}=\"{1}\"", name, Il2CppNativeCodeBuilderUtils.EscapeEmbeddedQuotes(value));
		}

		private static string EditorVersionFilenameFor(string editorVersion)
		{
			return string.Format("il2cpp_cache {0}", editorVersion);
		}

		private static string EscapeEmbeddedQuotes(string value)
		{
			return value.Replace("\"", "\\\"");
		}
	}
}
