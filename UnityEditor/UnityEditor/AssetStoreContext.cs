using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal sealed class AssetStoreContext : ScriptableObject
	{
		private static Regex standardPackageRe = new Regex("/Standard Packages/(Character\\ Controller|Glass\\ Refraction\\ \\(Pro\\ Only\\)|Image\\ Effects\\ \\(Pro\\ Only\\)|Light\\ Cookies|Light\\ Flares|Particles|Physic\\ Materials|Projectors|Scripts|Standard\\ Assets\\ \\(Mobile\\)|Skyboxes|Terrain\\ Assets|Toon\\ Shading|Tree\\ Creator|Water\\ \\(Basic\\)|Water\\ \\(Pro\\ Only\\))\\.unitypackage$", RegexOptions.IgnoreCase);
		private static Regex generatedIDRe = new Regex("^\\{(.*)\\}$");
		private static Regex invalidPathChars = new Regex("[^a-zA-Z0-9() _-]");
		public bool docked;
		public string initialOpenURL;
		public AssetStoreWindow window;
		private WebScriptObject JS
		{
			get
			{
				return this.window.scriptObject;
			}
		}
		public WebScriptObject packages
		{
			get
			{
				WebScriptObject packageList = this.GetPackageList();
				return packageList.Get("results");
			}
		}
		private AssetStoreContext()
		{
		}
		public string GetInitialOpenURL()
		{
			if (this.initialOpenURL != null)
			{
				string result = this.initialOpenURL;
				this.initialOpenURL = null;
				return result;
			}
			return string.Empty;
		}
		public string GetAuthToken()
		{
			return InternalEditorUtility.GetAuthToken();
		}
		public int[] GetLicenseFlags()
		{
			return InternalEditorUtility.GetLicenseFlags();
		}
		public void Download(WebScriptObject package, WebScriptObject downloadInfo)
		{
			string url = downloadInfo.Get("url");
			string key = downloadInfo.Get("key");
			string package_id = downloadInfo.Get("id");
			string package_name = package.Get("title");
			string publisher_name = package.Get("publisher.label");
			string category_name = package.Get("category.label");
			AssetStoreContext.Download(package_id, url, key, package_name, publisher_name, category_name, null);
		}
		public static string[] PackageStorePath(string publisher_name, string category_name, string package_name, string package_id, string url)
		{
			string[] array = new string[]
			{
				publisher_name,
				category_name,
				package_name
			};
			for (int i = 0; i < 3; i++)
			{
				array[i] = AssetStoreContext.invalidPathChars.Replace(array[i], string.Empty);
			}
			if (array[2] == string.Empty)
			{
				array[2] = AssetStoreContext.invalidPathChars.Replace(package_id, string.Empty);
			}
			if (array[2] == string.Empty)
			{
				array[2] = AssetStoreContext.invalidPathChars.Replace(url, string.Empty);
			}
			return array;
		}
		public static void Download(string package_id, string url, string key, string package_name, string publisher_name, string category_name, AssetStoreUtils.DownloadDoneCallback doneCallback)
		{
			string[] destination = AssetStoreContext.PackageStorePath(publisher_name, category_name, package_name, package_id, url);
			JSONValue jSONValue = JSONParser.SimpleParse(AssetStoreUtils.CheckDownload(package_id, url, destination, key));
			if (jSONValue.Get("in_progress").AsBool(true))
			{
				Debug.Log("Will not download " + package_name + ". Download is already in progress.");
				return;
			}
			string a = jSONValue.Get("download.url").AsString(true);
			string a2 = jSONValue.Get("download.key").AsString(true);
			bool resumeOK = a == url && a2 == key;
			JSONValue value = default(JSONValue);
			value["url"] = url;
			value["key"] = key;
			JSONValue jSONValue2 = default(JSONValue);
			jSONValue2["download"] = value;
			AssetStoreUtils.Download(package_id, url, destination, key, jSONValue2.ToString(), resumeOK, doneCallback);
		}
		public string GetString(string key)
		{
			return EditorPrefs.GetString(key);
		}
		public int GetInt(string key)
		{
			return EditorPrefs.GetInt(key);
		}
		public float GetFloat(string key)
		{
			return EditorPrefs.GetFloat(key);
		}
		public void SetString(string key, string value)
		{
			EditorPrefs.SetString(key, value);
		}
		public void SetInt(string key, int value)
		{
			EditorPrefs.SetInt(key, value);
		}
		public void SetFloat(string key, float value)
		{
			EditorPrefs.SetFloat(key, value);
		}
		public bool HasKey(string key)
		{
			return EditorPrefs.HasKey(key);
		}
		public void DeleteKey(string key)
		{
			EditorPrefs.DeleteKey(key);
		}
		private bool IsBuiltinStandardAsset(string path)
		{
			return AssetStoreContext.standardPackageRe.IsMatch(path);
		}
		public WebScriptObject GetPackageList()
		{
			Dictionary<string, WebScriptObject> dictionary = new Dictionary<string, WebScriptObject>();
			WebScriptObject webScriptObject = this.JS.ParseJSON("{}");
			WebScriptObject webScriptObject2 = this.JS.ParseJSON("[]");
			PackageInfo[] packageList = PackageInfo.GetPackageList();
			PackageInfo[] array = packageList;
			for (int i = 0; i < array.Length; i++)
			{
				PackageInfo packageInfo = array[i];
				WebScriptObject webScriptObject3;
				if (packageInfo.jsonInfo == string.Empty)
				{
					webScriptObject3 = this.JS.ParseJSON("{}");
					webScriptObject3.Set<string>("title", Path.GetFileNameWithoutExtension(packageInfo.packagePath));
					webScriptObject3.Set<string>("id", "{" + packageInfo.packagePath + "}");
					if (this.IsBuiltinStandardAsset(packageInfo.packagePath))
					{
						webScriptObject3.Set<WebScriptObject>("publisher", this.JS.ParseJSON("{\"label\": \"Unity Technologies\",\"id\": \"1\"}"));
						webScriptObject3.Set<WebScriptObject>("category", this.JS.ParseJSON("{\"label\": \"Prefab Packages\",\"id\": \"4\"}"));
						webScriptObject3.Set<string>("version", "3.5.0.0");
						webScriptObject3.Set<string>("version_id", "-1");
					}
				}
				else
				{
					webScriptObject3 = this.JS.ParseJSON(packageInfo.jsonInfo);
					if (webScriptObject3.Get("id") == null)
					{
						WebScriptObject webScriptObject4 = webScriptObject3.Get("link");
						if (webScriptObject4 != null)
						{
							webScriptObject3.Set<string>("id", webScriptObject4.Get("id"));
						}
						else
						{
							webScriptObject3.Set<string>("id", "{" + packageInfo.packagePath + "}");
						}
					}
				}
				string key = webScriptObject3.Get("id");
				webScriptObject3.Set<string>("local_icon", packageInfo.iconURL);
				webScriptObject3.Set<string>("local_path", packageInfo.packagePath);
				if (!dictionary.ContainsKey(key) || dictionary[key].Get("version_id") == null || (webScriptObject3.Get("version_id") != null && dictionary[key].Get("version_id") <= webScriptObject3.Get("version_id")))
				{
					dictionary[key] = webScriptObject3;
				}
			}
			int num = 0;
			foreach (KeyValuePair<string, WebScriptObject> current in dictionary)
			{
				webScriptObject2.Set<WebScriptObject>(num++, current.Value);
			}
			webScriptObject.Set<WebScriptObject>("results", webScriptObject2);
			return webScriptObject;
		}
		public int GetSkinIndex()
		{
			return (!EditorGUIUtility.isProSkin) ? 0 : 1;
		}
		public bool GetDockedStatus()
		{
			return this.docked;
		}
		public bool OpenPackage(string id)
		{
			return this.OpenPackage(id, "default");
		}
		public bool OpenPackage(string id, string action)
		{
			return AssetStoreContext.OpenPackageInternal(id);
		}
		public static bool OpenPackageInternal(string id)
		{
			Match match = AssetStoreContext.generatedIDRe.Match(id);
			if (match.Success && File.Exists(match.Groups[1].Value))
			{
				AssetDatabase.ImportPackage(match.Groups[1].Value, true);
				return true;
			}
			PackageInfo[] packageList = PackageInfo.GetPackageList();
			for (int i = 0; i < packageList.Length; i++)
			{
				PackageInfo packageInfo = packageList[i];
				if (packageInfo.jsonInfo != string.Empty)
				{
					JSONValue jSONValue = JSONParser.SimpleParse(packageInfo.jsonInfo);
					string text = (!jSONValue.Get("id").IsNull()) ? jSONValue["id"].AsString(true) : null;
					if (text != null && text == id && File.Exists(packageInfo.packagePath))
					{
						AssetDatabase.ImportPackage(packageInfo.packagePath, true);
						return true;
					}
				}
			}
			Debug.LogError("Unknown package ID " + id);
			return false;
		}
		public void MakeMenu(WebScriptObject contextMenu)
		{
			this.MakeMenu(contextMenu, 0f, 0f);
		}
		public void MakeMenu(WebScriptObject contextMenu, float deltaX, float deltaY)
		{
			WebScriptObject webScriptObject = contextMenu.Get("commands");
			int num = webScriptObject.Get("length");
			string[] array = new string[num];
			string[] array2 = new string[num];
			for (int i = 0; i < webScriptObject.Get("length"); i++)
			{
				WebScriptObject webScriptObject2 = webScriptObject.Get(i);
				array[i] = webScriptObject2.Get("label");
				array2[i] = webScriptObject2.Get("action");
			}
			Vector2 mousePosition = Event.current.mousePosition;
			Rect position = new Rect(mousePosition.x + deltaX, mousePosition.y + deltaY, 0f, 0f);
			EditorUtility.DisplayCustomMenu(position, array, null, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), array2);
		}
		public void OpenBrowser(string url)
		{
			Application.OpenURL(url);
		}
		private void ContextMenuClick(object userData, string[] options, int selected)
		{
			if (selected >= 0)
			{
				string[] array = userData as string[];
				this.JS.EvalJavaScript(array[selected]);
			}
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SessionSetString(string key, string value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string SessionGetString(string key);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SessionRemoveString(string key);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SessionHasString(string key);
	}
}
