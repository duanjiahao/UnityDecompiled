using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor
{
	internal abstract class WebTemplateManagerBase
	{
		private class Styles
		{
			public GUIStyle thumbnail = "IN ThumbnailShadow";

			public GUIStyle thumbnailLabel = "IN ThumbnailSelection";
		}

		private static WebTemplateManagerBase.Styles s_Styles;

		private WebTemplate[] s_Templates = null;

		private GUIContent[] s_TemplateGUIThumbnails = null;

		private const float kWebTemplateGridPadding = 15f;

		private const float kThumbnailSize = 80f;

		private const float kThumbnailLabelHeight = 20f;

		private const float kThumbnailPadding = 5f;

		public abstract string customTemplatesFolder
		{
			get;
		}

		public abstract string builtinTemplatesFolder
		{
			get;
		}

		public abstract Texture2D defaultIcon
		{
			get;
		}

		public WebTemplate[] Templates
		{
			get
			{
				if (this.s_Templates == null || this.s_TemplateGUIThumbnails == null)
				{
					this.BuildTemplateList();
				}
				return this.s_Templates;
			}
		}

		public GUIContent[] TemplateGUIThumbnails
		{
			get
			{
				if (this.s_Templates == null || this.s_TemplateGUIThumbnails == null)
				{
					this.BuildTemplateList();
				}
				return this.s_TemplateGUIThumbnails;
			}
		}

		public int GetTemplateIndex(string path)
		{
			int result;
			for (int i = 0; i < this.Templates.Length; i++)
			{
				if (path.Equals(this.Templates[i].ToString()))
				{
					result = i;
					return result;
				}
			}
			result = 0;
			return result;
		}

		public void ClearTemplates()
		{
			this.s_Templates = null;
			this.s_TemplateGUIThumbnails = null;
		}

		private void BuildTemplateList()
		{
			List<WebTemplate> list = new List<WebTemplate>();
			if (Directory.Exists(this.customTemplatesFolder))
			{
				list.AddRange(this.ListTemplates(this.customTemplatesFolder));
			}
			if (Directory.Exists(this.builtinTemplatesFolder))
			{
				list.AddRange(this.ListTemplates(this.builtinTemplatesFolder));
			}
			else
			{
				Debug.LogError("Did not find built-in templates.");
			}
			this.s_Templates = list.ToArray();
			this.s_TemplateGUIThumbnails = new GUIContent[this.s_Templates.Length];
			for (int i = 0; i < this.s_TemplateGUIThumbnails.Length; i++)
			{
				this.s_TemplateGUIThumbnails[i] = this.s_Templates[i].ToGUIContent(this.defaultIcon);
			}
		}

		private WebTemplate Load(string path)
		{
			WebTemplate result;
			if (!Directory.Exists(path) || Directory.GetFiles(path, "index.*").Length < 1)
			{
				result = null;
			}
			else
			{
				string[] array = path.Split(new char[]
				{
					'/',
					'\\'
				});
				WebTemplate webTemplate = new WebTemplate();
				webTemplate.m_Name = array[array.Length - 1];
				if (array.Length > 3 && array[array.Length - 3].Equals("Assets"))
				{
					webTemplate.m_Path = "PROJECT:" + webTemplate.m_Name;
				}
				else
				{
					webTemplate.m_Path = "APPLICATION:" + webTemplate.m_Name;
				}
				string[] files = Directory.GetFiles(path, "thumbnail.*");
				if (files.Length > 0)
				{
					webTemplate.m_Thumbnail = new Texture2D(2, 2);
					webTemplate.m_Thumbnail.LoadImage(File.ReadAllBytes(files[0]));
				}
				List<string> list = new List<string>();
				Regex regex = new Regex("\\%UNITY_CUSTOM_([A-Z_]+)\\%");
				MatchCollection matchCollection = regex.Matches(File.ReadAllText(Directory.GetFiles(path, "index.*")[0]));
				IEnumerator enumerator = matchCollection.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Match match = (Match)enumerator.Current;
						string text = match.Value.Substring("%UNITY_CUSTOM_".Length);
						text = text.Substring(0, text.Length - 1);
						if (!list.Contains(text))
						{
							list.Add(text);
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				webTemplate.m_CustomKeys = list.ToArray();
				result = webTemplate;
			}
			return result;
		}

		private List<WebTemplate> ListTemplates(string path)
		{
			List<WebTemplate> list = new List<WebTemplate>();
			string[] directories = Directory.GetDirectories(path);
			string[] array = directories;
			for (int i = 0; i < array.Length; i++)
			{
				string path2 = array[i];
				WebTemplate webTemplate = this.Load(path2);
				if (webTemplate != null)
				{
					list.Add(webTemplate);
				}
			}
			return list;
		}

		public void SelectionUI(SerializedProperty templateProp)
		{
			if (WebTemplateManagerBase.s_Styles == null)
			{
				WebTemplateManagerBase.s_Styles = new WebTemplateManagerBase.Styles();
			}
			if (this.TemplateGUIThumbnails.Length < 1)
			{
				GUILayout.Label(EditorGUIUtility.TextContent("No templates found."), new GUILayoutOption[0]);
			}
			else
			{
				int num = Mathf.Min((int)Mathf.Max(((float)Screen.width - 30f) / 80f, 1f), this.TemplateGUIThumbnails.Length);
				int num2 = Mathf.Max((int)Mathf.Ceil((float)this.TemplateGUIThumbnails.Length / (float)num), 1);
				bool changed = GUI.changed;
				templateProp.stringValue = this.Templates[WebTemplateManagerBase.ThumbnailList(GUILayoutUtility.GetRect((float)num * 80f, (float)num2 * 100f, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}), this.GetTemplateIndex(templateProp.stringValue), this.TemplateGUIThumbnails, num)].ToString();
				bool flag = !changed && GUI.changed;
				bool changed2 = GUI.changed;
				GUI.changed = false;
				string[] templateCustomKeys = PlayerSettings.templateCustomKeys;
				for (int i = 0; i < templateCustomKeys.Length; i++)
				{
					string text = templateCustomKeys[i];
					string text2 = PlayerSettings.GetTemplateCustomValue(text);
					text2 = EditorGUILayout.TextField(WebTemplateManagerBase.PrettyTemplateKeyName(text), text2, new GUILayoutOption[0]);
					PlayerSettings.SetTemplateCustomValue(text, text2);
				}
				if (GUI.changed)
				{
					templateProp.serializedObject.Update();
				}
				GUI.changed |= changed2;
				if (flag)
				{
					GUIUtility.hotControl = 0;
					GUIUtility.keyboardControl = 0;
					templateProp.serializedObject.ApplyModifiedProperties();
					PlayerSettings.templateCustomKeys = this.Templates[this.GetTemplateIndex(templateProp.stringValue)].CustomKeys;
					templateProp.serializedObject.Update();
				}
			}
		}

		private static int ThumbnailList(Rect rect, int selection, GUIContent[] thumbnails, int maxRowItems)
		{
			int num = 0;
			int i = 0;
			while (i < thumbnails.Length)
			{
				int num2 = 0;
				while (num2 < maxRowItems && i < thumbnails.Length)
				{
					if (WebTemplateManagerBase.ThumbnailListItem(new Rect(rect.x + (float)num2 * 80f, rect.y + (float)num * 100f, 80f, 100f), i == selection, thumbnails[i]))
					{
						selection = i;
					}
					num2++;
					i++;
				}
				num++;
			}
			return selection;
		}

		private static bool ThumbnailListItem(Rect rect, bool selected, GUIContent content)
		{
			EventType type = Event.current.type;
			if (type != EventType.MouseDown)
			{
				if (type == EventType.Repaint)
				{
					Rect position = new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, rect.height - 20f - 10f);
					WebTemplateManagerBase.s_Styles.thumbnail.Draw(position, content.image, false, false, selected, selected);
					WebTemplateManagerBase.s_Styles.thumbnailLabel.Draw(new Rect(rect.x, rect.y + rect.height - 20f, rect.width, 20f), content.text, false, false, selected, selected);
				}
			}
			else if (rect.Contains(Event.current.mousePosition))
			{
				if (!selected)
				{
					GUI.changed = true;
				}
				selected = true;
				Event.current.Use();
			}
			return selected;
		}

		private static string PrettyTemplateKeyName(string name)
		{
			string[] array = name.Split(new char[]
			{
				'_'
			});
			array[0] = WebTemplateManagerBase.UppercaseFirst(array[0].ToLower());
			for (int i = 1; i < array.Length; i++)
			{
				array[i] = array[i].ToLower();
			}
			return string.Join(" ", array);
		}

		private static string UppercaseFirst(string target)
		{
			string result;
			if (string.IsNullOrEmpty(target))
			{
				result = string.Empty;
			}
			else
			{
				result = char.ToUpper(target[0]) + target.Substring(1);
			}
			return result;
		}
	}
}
