using System;
using System.Diagnostics;
using System.IO;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.RestService
{
	internal class PairingRestHandler : Handler
	{
		protected override JSONValue HandlePost(Request request, JSONValue payload)
		{
			ScriptEditorSettings.ServerURL = payload["url"].AsString();
			ScriptEditorSettings.Name = ((!payload.ContainsKey("name")) ? null : payload["name"].AsString());
			ScriptEditorSettings.ProcessId = ((!payload.ContainsKey("processid")) ? -1 : ((int)payload["processid"].AsFloat()));
			Logger.Log(string.Concat(new object[]
			{
				"[Pair] Name: ",
				ScriptEditorSettings.Name ?? "<null>",
				" ServerURL ",
				ScriptEditorSettings.ServerURL,
				" Process id: ",
				ScriptEditorSettings.ProcessId
			}));
			JSONValue result = default(JSONValue);
			result["unityprocessid"] = Process.GetCurrentProcess().Id;
			result["unityproject"] = Application.dataPath;
			return result;
		}

		internal static void Register()
		{
			Router.RegisterHandler("/unity/pair", new PairingRestHandler());
		}

		[OnOpenAsset]
		private static bool OnOpenAsset(int instanceID, int line)
		{
			bool result;
			if (ScriptEditorSettings.ServerURL == null)
			{
				result = false;
			}
			else
			{
				string text = Path.GetFullPath(Application.dataPath + "/../" + AssetDatabase.GetAssetPath(instanceID)).Replace('\\', '/');
				string text2 = text.ToLower();
				if (!text2.EndsWith(".cs") && !text2.EndsWith(".js") && !text2.EndsWith(".boo"))
				{
					result = false;
				}
				else if (!PairingRestHandler.IsScriptEditorRunning() || !RestRequest.Send("/openfile", string.Concat(new object[]
				{
					"{ \"file\" : \"",
					text,
					"\", \"line\" : ",
					line,
					" }"
				}), 5000))
				{
					ScriptEditorSettings.ServerURL = null;
					ScriptEditorSettings.Name = null;
					ScriptEditorSettings.ProcessId = -1;
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private static bool IsScriptEditorRunning()
		{
			bool result;
			if (ScriptEditorSettings.ProcessId < 0)
			{
				result = false;
			}
			else
			{
				try
				{
					Process processById = Process.GetProcessById(ScriptEditorSettings.ProcessId);
					result = !processById.HasExited;
				}
				catch (Exception an_exception)
				{
					Logger.Log(an_exception);
					result = false;
				}
			}
			return result;
		}
	}
}
