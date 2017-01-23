using System;
using UnityEngine;

namespace UnityEditor
{
	internal class IconSelector : EditorWindow
	{
		public delegate void MonoScriptIconChangedCallback(MonoScript monoScript);

		private class Styles
		{
			public GUIStyle background = "sv_iconselector_back";

			public GUIStyle seperator = "sv_iconselector_sep";

			public GUIStyle selection = "sv_iconselector_selection";

			public GUIStyle selectionLabel = "sv_iconselector_labelselection";

			public GUIStyle noneButton = "sv_iconselector_button";
		}

		private static IconSelector s_IconSelector = null;

		private static long s_LastClosedTime = 0L;

		private static int s_LastInstanceID = -1;

		private static int s_HashIconSelector = "IconSelector".GetHashCode();

		private static IconSelector.Styles m_Styles;

		private UnityEngine.Object m_TargetObject;

		private Texture2D m_StartIcon;

		private bool m_ShowLabelIcons;

		private GUIContent[] m_LabelLargeIcons;

		private GUIContent[] m_LabelIcons;

		private GUIContent[] m_LargeIcons;

		private GUIContent[] m_SmallIcons;

		private GUIContent m_NoneButtonContent;

		private IconSelector.MonoScriptIconChangedCallback m_MonoScriptIconChangedCallback;

		private GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
		{
			GUIContent[] array = new GUIContent[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
			}
			return array;
		}

		private void OnEnable()
		{
			base.hideFlags = HideFlags.DontSave;
		}

		private void OnDisable()
		{
			this.SaveIconChanges();
			IconSelector.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
			IconSelector.s_IconSelector = null;
		}

		private void SaveIconChanges()
		{
			Texture2D iconForObject = EditorGUIUtility.GetIconForObject(this.m_TargetObject);
			if (iconForObject != this.m_StartIcon)
			{
				MonoScript monoScript = this.m_TargetObject as MonoScript;
				if (monoScript != null)
				{
					if (this.m_MonoScriptIconChangedCallback != null)
					{
						this.m_MonoScriptIconChangedCallback(monoScript);
					}
					else
					{
						MonoImporter.CopyMonoScriptIconToImporters(monoScript);
					}
				}
			}
		}

		internal static bool ShowAtPosition(UnityEngine.Object targetObj, Rect activatorRect, bool showLabelIcons)
		{
			int instanceID = targetObj.GetInstanceID();
			long num = DateTime.Now.Ticks / 10000L;
			bool flag = num < IconSelector.s_LastClosedTime + 50L;
			bool result;
			if (instanceID != IconSelector.s_LastInstanceID || !flag)
			{
				Event.current.Use();
				IconSelector.s_LastInstanceID = instanceID;
				if (IconSelector.s_IconSelector == null)
				{
					IconSelector.s_IconSelector = ScriptableObject.CreateInstance<IconSelector>();
				}
				IconSelector.s_IconSelector.Init(targetObj, activatorRect, showLabelIcons);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal static void SetMonoScriptIconChangedCallback(IconSelector.MonoScriptIconChangedCallback callback)
		{
			if (IconSelector.s_IconSelector != null)
			{
				IconSelector.s_IconSelector.m_MonoScriptIconChangedCallback = callback;
			}
			else
			{
				Debug.Log("ERROR: setting callback on hidden IconSelector");
			}
		}

		private void Init(UnityEngine.Object targetObj, Rect activatorRect, bool showLabelIcons)
		{
			this.m_TargetObject = targetObj;
			this.m_StartIcon = EditorGUIUtility.GetIconForObject(this.m_TargetObject);
			this.m_ShowLabelIcons = showLabelIcons;
			Rect buttonRect = GUIUtility.GUIToScreenRect(activatorRect);
			GUIUtility.keyboardControl = 0;
			this.m_LabelLargeIcons = this.GetTextures("sv_label_", "", 0, 8);
			this.m_LabelIcons = this.GetTextures("sv_icon_name", "", 0, 8);
			this.m_SmallIcons = this.GetTextures("sv_icon_dot", "_sml", 0, 16);
			this.m_LargeIcons = this.GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);
			this.m_NoneButtonContent = EditorGUIUtility.IconContent("sv_icon_none");
			this.m_NoneButtonContent.text = "None";
			float x = 140f;
			float y = 86f;
			if (this.m_ShowLabelIcons)
			{
				y = 126f;
			}
			base.ShowAsDropDown(buttonRect, new Vector2(x, y));
		}

		private Texture2D ConvertLargeIconToSmallIcon(Texture2D largeIcon, ref bool isLabelIcon)
		{
			Texture2D result;
			if (largeIcon == null)
			{
				result = null;
			}
			else
			{
				isLabelIcon = true;
				for (int i = 0; i < this.m_LabelLargeIcons.Length; i++)
				{
					if (this.m_LabelLargeIcons[i].image == largeIcon)
					{
						result = (Texture2D)this.m_LabelIcons[i].image;
						return result;
					}
				}
				isLabelIcon = false;
				for (int j = 0; j < this.m_LargeIcons.Length; j++)
				{
					if (this.m_LargeIcons[j].image == largeIcon)
					{
						result = (Texture2D)this.m_SmallIcons[j].image;
						return result;
					}
				}
				result = largeIcon;
			}
			return result;
		}

		private Texture2D ConvertSmallIconToLargeIcon(Texture2D smallIcon, bool labelIcon)
		{
			Texture2D result;
			if (labelIcon)
			{
				for (int i = 0; i < this.m_LabelIcons.Length; i++)
				{
					if (this.m_LabelIcons[i].image == smallIcon)
					{
						result = (Texture2D)this.m_LabelLargeIcons[i].image;
						return result;
					}
				}
			}
			else
			{
				for (int j = 0; j < this.m_SmallIcons.Length; j++)
				{
					if (this.m_SmallIcons[j].image == smallIcon)
					{
						result = (Texture2D)this.m_LargeIcons[j].image;
						return result;
					}
				}
			}
			result = smallIcon;
			return result;
		}

		private void DoButton(GUIContent content, Texture2D selectedIcon, bool labelIcon)
		{
			int controlID = GUIUtility.GetControlID(IconSelector.s_HashIconSelector, FocusType.Keyboard);
			if (content.image == selectedIcon)
			{
				Rect position = GUILayoutUtility.topLevel.PeekNext();
				float num = 2f;
				position.x -= num;
				position.y -= num;
				position.width = (float)selectedIcon.width + 2f * num;
				position.height = (float)selectedIcon.height + 2f * num;
				GUI.Label(position, GUIContent.none, (!labelIcon) ? IconSelector.m_Styles.selection : IconSelector.m_Styles.selectionLabel);
			}
			if (EditorGUILayout.IconButton(controlID, content, GUIStyle.none, new GUILayoutOption[0]))
			{
				Texture2D icon = this.ConvertSmallIconToLargeIcon((Texture2D)content.image, labelIcon);
				EditorGUIUtility.SetIconForObject(this.m_TargetObject, icon);
				EditorUtility.ForceReloadInspectors();
				AnnotationWindow.IconChanged();
				if (Event.current.clickCount == 2)
				{
					this.CloseWindow();
				}
			}
		}

		private void DoTopSection(bool anySelected)
		{
			Rect position = new Rect(6f, 4f, 110f, 20f);
			GUI.Label(position, "Select Icon");
			using (new EditorGUI.DisabledScope(!anySelected))
			{
				Rect position2 = new Rect(93f, 6f, 43f, 12f);
				if (GUI.Button(position2, this.m_NoneButtonContent, IconSelector.m_Styles.noneButton))
				{
					EditorGUIUtility.SetIconForObject(this.m_TargetObject, null);
					EditorUtility.ForceReloadInspectors();
					AnnotationWindow.IconChanged();
				}
			}
		}

		private void CloseWindow()
		{
			base.Close();
			GUI.changed = true;
			GUIUtility.ExitGUI();
		}

		internal void OnGUI()
		{
			if (IconSelector.m_Styles == null)
			{
				IconSelector.m_Styles = new IconSelector.Styles();
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
			{
				this.CloseWindow();
			}
			Texture2D texture2D = EditorGUIUtility.GetIconForObject(this.m_TargetObject);
			bool flag = false;
			if (Event.current.type == EventType.Repaint)
			{
				texture2D = this.ConvertLargeIconToSmallIcon(texture2D, ref flag);
			}
			Event current = Event.current;
			EventType type = current.type;
			GUI.BeginGroup(new Rect(0f, 0f, base.position.width, base.position.height), IconSelector.m_Styles.background);
			this.DoTopSection(texture2D != null);
			GUILayout.Space(22f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(1f);
			GUI.enabled = false;
			GUILayout.Label("", IconSelector.m_Styles.seperator, new GUILayoutOption[0]);
			GUI.enabled = true;
			GUILayout.Space(1f);
			GUILayout.EndHorizontal();
			GUILayout.Space(3f);
			if (this.m_ShowLabelIcons)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(6f);
				for (int i = 0; i < this.m_LabelIcons.Length / 2; i++)
				{
					this.DoButton(this.m_LabelIcons[i], texture2D, true);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(6f);
				for (int j = this.m_LabelIcons.Length / 2; j < this.m_LabelIcons.Length; j++)
				{
					this.DoButton(this.m_LabelIcons[j], texture2D, true);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(3f);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(1f);
				GUI.enabled = false;
				GUILayout.Label("", IconSelector.m_Styles.seperator, new GUILayoutOption[0]);
				GUI.enabled = true;
				GUILayout.Space(1f);
				GUILayout.EndHorizontal();
				GUILayout.Space(3f);
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(9f);
			for (int k = 0; k < this.m_SmallIcons.Length / 2; k++)
			{
				this.DoButton(this.m_SmallIcons[k], texture2D, false);
			}
			GUILayout.Space(3f);
			GUILayout.EndHorizontal();
			GUILayout.Space(6f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(9f);
			for (int l = this.m_SmallIcons.Length / 2; l < this.m_SmallIcons.Length; l++)
			{
				this.DoButton(this.m_SmallIcons[l], texture2D, false);
			}
			GUILayout.Space(3f);
			GUILayout.EndHorizontal();
			GUILayout.Space(6f);
			GUI.backgroundColor = new Color(1f, 1f, 1f, 0.7f);
			bool flag2 = false;
			int controlID = GUIUtility.GetControlID(IconSelector.s_HashIconSelector, FocusType.Keyboard);
			if (GUILayout.Button(EditorGUIUtility.TempContent("Other..."), new GUILayoutOption[0]))
			{
				GUIUtility.keyboardControl = controlID;
				flag2 = true;
			}
			GUI.backgroundColor = new Color(1f, 1f, 1f, 1f);
			GUI.EndGroup();
			if (flag2)
			{
				ObjectSelector.get.Show(this.m_TargetObject, typeof(Texture2D), null, false);
				ObjectSelector.get.objectSelectorID = controlID;
				GUI.backgroundColor = new Color(1f, 1f, 1f, 0.7f);
				current.Use();
				GUIUtility.ExitGUI();
			}
			if (type == EventType.ExecuteCommand)
			{
				string commandName = current.commandName;
				if (commandName == "ObjectSelectorUpdated" && ObjectSelector.get.objectSelectorID == controlID && GUIUtility.keyboardControl == controlID)
				{
					Texture2D icon = ObjectSelector.GetCurrentObject() as Texture2D;
					EditorGUIUtility.SetIconForObject(this.m_TargetObject, icon);
					GUI.changed = true;
					current.Use();
				}
			}
		}
	}
}
