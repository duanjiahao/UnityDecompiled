using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Utils;
using UnityEngine;

namespace UnityEditorInternal
{
	public class ScriptEditorUtility
	{
		public enum ScriptEditor
		{
			Internal,
			MonoDevelop,
			VisualStudio,
			VisualStudioExpress,
			VisualStudioCode,
			Rider,
			Other = 32
		}

		public static ScriptEditorUtility.ScriptEditor GetScriptEditorFromPath(string path)
		{
			string text = path.ToLower();
			ScriptEditorUtility.ScriptEditor result;
			if (text == "internal")
			{
				result = ScriptEditorUtility.ScriptEditor.Internal;
			}
			else if (text.Contains("monodevelop") || text.Contains("xamarinstudio") || text.Contains("xamarin studio"))
			{
				result = ScriptEditorUtility.ScriptEditor.MonoDevelop;
			}
			else if (text.EndsWith("devenv.exe"))
			{
				result = ScriptEditorUtility.ScriptEditor.VisualStudio;
			}
			else if (text.EndsWith("vcsexpress.exe"))
			{
				result = ScriptEditorUtility.ScriptEditor.VisualStudioExpress;
			}
			else
			{
				string a = Path.GetFileName(Paths.UnifyDirectorySeparator(text)).Replace(" ", "");
				if (a == "visualstudio.app")
				{
					result = ScriptEditorUtility.ScriptEditor.MonoDevelop;
				}
				else if (a == "code.exe" || a == "visualstudiocode.app" || a == "vscode.app" || a == "code.app" || a == "code")
				{
					result = ScriptEditorUtility.ScriptEditor.VisualStudioCode;
				}
				else if (a == "rider.exe" || a == "rider64.exe" || a == "rider32.exe" || a == "ridereap.app" || a == "rider.app" || a == "rider.sh")
				{
					result = ScriptEditorUtility.ScriptEditor.Rider;
				}
				else
				{
					result = ScriptEditorUtility.ScriptEditor.Other;
				}
			}
			return result;
		}

		public static bool IsScriptEditorSpecial(string path)
		{
			return ScriptEditorUtility.GetScriptEditorFromPath(path) != ScriptEditorUtility.ScriptEditor.Other;
		}

		public static string GetExternalScriptEditor()
		{
			return EditorPrefs.GetString("kScriptsDefaultApp");
		}

		public static void SetExternalScriptEditor(string path)
		{
			EditorPrefs.SetString("kScriptsDefaultApp", path);
		}

		private static string GetScriptEditorArgsKey(string path)
		{
			string result;
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				result = "kScriptEditorArgs_" + path;
			}
			else
			{
				result = "kScriptEditorArgs" + path;
			}
			return result;
		}

		private static string GetDefaultStringEditorArgs()
		{
			string result;
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				result = "";
			}
			else
			{
				result = "\"$(File)\"";
			}
			return result;
		}

		public static string GetExternalScriptEditorArgs()
		{
			string externalScriptEditor = ScriptEditorUtility.GetExternalScriptEditor();
			string result;
			if (ScriptEditorUtility.IsScriptEditorSpecial(externalScriptEditor))
			{
				result = "";
			}
			else
			{
				result = EditorPrefs.GetString(ScriptEditorUtility.GetScriptEditorArgsKey(externalScriptEditor), ScriptEditorUtility.GetDefaultStringEditorArgs());
			}
			return result;
		}

		public static void SetExternalScriptEditorArgs(string args)
		{
			string externalScriptEditor = ScriptEditorUtility.GetExternalScriptEditor();
			EditorPrefs.SetString(ScriptEditorUtility.GetScriptEditorArgsKey(externalScriptEditor), args);
		}

		public static ScriptEditorUtility.ScriptEditor GetScriptEditorFromPreferences()
		{
			return ScriptEditorUtility.GetScriptEditorFromPath(ScriptEditorUtility.GetExternalScriptEditor());
		}

		public static string[] GetFoundScriptEditorPaths(RuntimePlatform platform)
		{
			List<string> list = new List<string>();
			if (platform == RuntimePlatform.OSXEditor)
			{
				ScriptEditorUtility.AddIfDirectoryExists("/Applications/Visual Studio.app", list);
			}
			return list.ToArray();
		}

		private static void AddIfDirectoryExists(string path, List<string> list)
		{
			if (Directory.Exists(path))
			{
				list.Add(path);
			}
		}
	}
}
