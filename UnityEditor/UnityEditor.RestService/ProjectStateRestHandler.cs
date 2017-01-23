using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Scripting;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.RestService
{
	internal class ProjectStateRestHandler : Handler
	{
		public class Island
		{
			public MonoIsland MonoIsland
			{
				get;
				set;
			}

			public string Name
			{
				get;
				set;
			}

			public List<string> References
			{
				get;
				set;
			}
		}

		private static string ProjectPath
		{
			get
			{
				return Path.GetDirectoryName(Application.dataPath);
			}
		}

		private static string AssetsPath
		{
			get
			{
				return ProjectStateRestHandler.ProjectPath + "/Assets";
			}
		}

		protected override JSONValue HandleGet(Request request, JSONValue payload)
		{
			AssetDatabase.Refresh();
			return ProjectStateRestHandler.JsonForProject();
		}

		private static JSONValue JsonForProject()
		{
			List<ProjectStateRestHandler.Island> list = (from i in InternalEditorUtility.GetMonoIslands()
			select new ProjectStateRestHandler.Island
			{
				MonoIsland = i,
				Name = Path.GetFileNameWithoutExtension(i._output),
				References = i._references.ToList<string>()
			}).ToList<ProjectStateRestHandler.Island>();
			foreach (ProjectStateRestHandler.Island current in list)
			{
				List<string> list2 = new List<string>();
				List<string> list3 = new List<string>();
				foreach (string current2 in current.References)
				{
					string refName = Path.GetFileNameWithoutExtension(current2);
					if (current2.StartsWith("Library/") && list.Any((ProjectStateRestHandler.Island i) => i.Name == refName))
					{
						list2.Add(refName);
						list3.Add(current2);
					}
					if (current2.EndsWith("/UnityEditor.dll") || current2.EndsWith("/UnityEngine.dll") || current2.EndsWith("\\UnityEditor.dll") || current2.EndsWith("\\UnityEngine.dll"))
					{
						list3.Add(current2);
					}
				}
				current.References.Add(InternalEditorUtility.GetEditorAssemblyPath());
				current.References.Add(InternalEditorUtility.GetEngineAssemblyPath());
				foreach (string current3 in list2)
				{
					current.References.Add(current3);
				}
				foreach (string current4 in list3)
				{
					current.References.Remove(current4);
				}
			}
			string[] array = list.SelectMany((ProjectStateRestHandler.Island i) => i.MonoIsland._files).Concat(ProjectStateRestHandler.GetAllSupportedFiles()).Distinct<string>().ToArray<string>();
			string[] strings = ProjectStateRestHandler.RelativeToProjectPath(ProjectStateRestHandler.FindEmptyDirectories(ProjectStateRestHandler.AssetsPath, array));
			JSONValue result = default(JSONValue);
			result["islands"] = new JSONValue((from i in list
			select ProjectStateRestHandler.JsonForIsland(i) into i2
			where !i2.IsNull()
			select i2).ToList<JSONValue>());
			result["basedirectory"] = ProjectStateRestHandler.ProjectPath;
			JSONValue value = default(JSONValue);
			value["files"] = Handler.ToJSON(array);
			value["emptydirectories"] = Handler.ToJSON(strings);
			result["assetdatabase"] = value;
			return result;
		}

		private static bool IsSupportedExtension(string extension)
		{
			if (extension.StartsWith("."))
			{
				extension = extension.Substring(1);
			}
			IEnumerable<string> source = EditorSettings.projectGenerationBuiltinExtensions.Concat(EditorSettings.projectGenerationUserExtensions);
			return source.Any((string s) => string.Equals(s, extension, StringComparison.InvariantCultureIgnoreCase));
		}

		private static IEnumerable<string> GetAllSupportedFiles()
		{
			return from asset in AssetDatabase.GetAllAssetPaths()
			where ProjectStateRestHandler.IsSupportedExtension(Path.GetExtension(asset))
			select asset;
		}

		private static JSONValue JsonForIsland(ProjectStateRestHandler.Island island)
		{
			JSONValue result;
			if (island.Name == "UnityEngine" || island.Name == "UnityEditor")
			{
				result = null;
			}
			else
			{
				JSONValue jSONValue = default(JSONValue);
				jSONValue["name"] = island.Name;
				jSONValue["language"] = ((!island.Name.Contains("Boo")) ? ((!island.Name.Contains("UnityScript")) ? "C#" : "UnityScript") : "Boo");
				jSONValue["files"] = Handler.ToJSON(island.MonoIsland._files);
				jSONValue["defines"] = Handler.ToJSON(island.MonoIsland._defines);
				jSONValue["references"] = Handler.ToJSON(island.MonoIsland._references);
				jSONValue["basedirectory"] = ProjectStateRestHandler.ProjectPath;
				result = jSONValue;
			}
			return result;
		}

		private static void FindPotentialEmptyDirectories(string path, ICollection<string> result)
		{
			string[] directories = Directory.GetDirectories(path);
			if (directories.Length == 0)
			{
				result.Add(path.Replace('\\', '/'));
			}
			else
			{
				string[] array = directories;
				for (int i = 0; i < array.Length; i++)
				{
					string path2 = array[i];
					ProjectStateRestHandler.FindPotentialEmptyDirectories(path2, result);
				}
			}
		}

		private static IEnumerable<string> FindPotentialEmptyDirectories(string path)
		{
			List<string> result = new List<string>();
			ProjectStateRestHandler.FindPotentialEmptyDirectories(path, result);
			return result;
		}

		private static string[] FindEmptyDirectories(string path, string[] files)
		{
			IEnumerable<string> source = ProjectStateRestHandler.FindPotentialEmptyDirectories(path);
			return (from d in source
			where !files.Any((string f) => f.StartsWith(d))
			select d).ToArray<string>();
		}

		private static string[] RelativeToProjectPath(string[] paths)
		{
			string projectPath = (!ProjectStateRestHandler.ProjectPath.EndsWith("/")) ? (ProjectStateRestHandler.ProjectPath + "/") : ProjectStateRestHandler.ProjectPath;
			return (from d in paths
			select (!d.StartsWith(projectPath)) ? d : d.Substring(projectPath.Length)).ToArray<string>();
		}

		internal static void Register()
		{
			Router.RegisterHandler("/unity/projectstate", new ProjectStateRestHandler());
		}
	}
}
