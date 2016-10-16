using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.RestService
{
	internal class ScriptEditorSettings
	{
		public static string Name
		{
			get;
			set;
		}

		public static string ServerURL
		{
			get;
			set;
		}

		public static int ProcessId
		{
			get;
			set;
		}

		public static List<string> OpenDocuments
		{
			get;
			set;
		}

		private static string FilePath
		{
			get
			{
				return Application.dataPath + "/../Library/UnityScriptEditorSettings.json";
			}
		}

		static ScriptEditorSettings()
		{
			ScriptEditorSettings.OpenDocuments = new List<string>();
			ScriptEditorSettings.Clear();
		}

		private static void Clear()
		{
			ScriptEditorSettings.Name = null;
			ScriptEditorSettings.ServerURL = null;
			ScriptEditorSettings.ProcessId = -1;
		}

		public static void Save()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{{\n\t\"name\" : \"{0}\",\n\t\"serverurl\" : \"{1}\",\n\t\"processid\" : {2},\n\t", ScriptEditorSettings.Name, ScriptEditorSettings.ServerURL, ScriptEditorSettings.ProcessId);
			stringBuilder.AppendFormat("\"opendocuments\" : [{0}]\n}}", string.Join(",", (from d in ScriptEditorSettings.OpenDocuments
			select "\"" + d + "\"").ToArray<string>()));
			File.WriteAllText(ScriptEditorSettings.FilePath, stringBuilder.ToString());
		}

		public static void Load()
		{
			try
			{
				string jsondata = File.ReadAllText(ScriptEditorSettings.FilePath);
				JSONValue jSONValue = new JSONParser(jsondata).Parse();
				ScriptEditorSettings.Name = ((!jSONValue.ContainsKey("name")) ? null : jSONValue["name"].AsString());
				ScriptEditorSettings.ServerURL = ((!jSONValue.ContainsKey("serverurl")) ? null : jSONValue["serverurl"].AsString());
				ScriptEditorSettings.ProcessId = ((!jSONValue.ContainsKey("processid")) ? -1 : ((int)jSONValue["processid"].AsFloat()));
				List<string> arg_101_0;
				if (jSONValue.ContainsKey("opendocuments"))
				{
					arg_101_0 = (from d in jSONValue["opendocuments"].AsList()
					select d.AsString()).ToList<string>();
				}
				else
				{
					arg_101_0 = new List<string>();
				}
				ScriptEditorSettings.OpenDocuments = arg_101_0;
				if (ScriptEditorSettings.ProcessId >= 0)
				{
					Process.GetProcessById(ScriptEditorSettings.ProcessId);
				}
			}
			catch (FileNotFoundException)
			{
				ScriptEditorSettings.Clear();
				ScriptEditorSettings.Save();
			}
			catch (Exception an_exception)
			{
				Logger.Log(an_exception);
				ScriptEditorSettings.Clear();
				ScriptEditorSettings.Save();
			}
		}
	}
}
