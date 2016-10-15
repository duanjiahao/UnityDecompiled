using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	internal class AddShaderVariantWindow : EditorWindow
	{
		internal class PopupData
		{
			public Shader shader;

			public ShaderVariantCollection collection;

			public int[] types;

			public string[][] keywords;
		}

		private class Styles
		{
			public static readonly GUIStyle sMenuItem = "MenuItem";

			public static readonly GUIStyle sSeparator = "sv_iconselector_sep";
		}

		private const float kMargin = 2f;

		private const float kSpaceHeight = 6f;

		private const float kSeparatorHeight = 3f;

		private const float kMinWindowWidth = 400f;

		private const float kMiscUIHeight = 120f;

		private const float kMinWindowHeight = 264f;

		private AddShaderVariantWindow.PopupData m_Data;

		private List<string> m_SelectedKeywords;

		private List<string> m_AvailableKeywords;

		private List<int> m_FilteredVariants;

		private List<int> m_SelectedVariants;

		public AddShaderVariantWindow()
		{
			base.position = new Rect(100f, 100f, 600f, 396f);
			base.minSize = new Vector2(400f, 264f);
			base.wantsMouseMove = true;
		}

		private void Initialize(AddShaderVariantWindow.PopupData data)
		{
			this.m_Data = data;
			this.m_SelectedKeywords = new List<string>();
			this.m_AvailableKeywords = new List<string>();
			this.m_SelectedVariants = new List<int>();
			this.m_AvailableKeywords.Sort();
			this.m_FilteredVariants = new List<int>();
			this.ApplyKeywordFilter();
		}

		public static void ShowAddVariantWindow(AddShaderVariantWindow.PopupData data)
		{
			AddShaderVariantWindow window = EditorWindow.GetWindow<AddShaderVariantWindow>(true, "Add shader " + data.shader.name + " variants to collection");
			window.Initialize(data);
			window.m_Parent.window.m_DontSaveToLayout = true;
		}

		private void ApplyKeywordFilter()
		{
			this.m_FilteredVariants.Clear();
			this.m_AvailableKeywords.Clear();
			for (int i = 0; i < this.m_Data.keywords.Length; i++)
			{
				bool flag = true;
				for (int j = 0; j < this.m_SelectedKeywords.Count; j++)
				{
					if (!this.m_Data.keywords[i].Contains(this.m_SelectedKeywords[j]))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					this.m_FilteredVariants.Add(i);
					string[] array = this.m_Data.keywords[i];
					for (int k = 0; k < array.Length; k++)
					{
						string item = array[k];
						if (!this.m_AvailableKeywords.Contains(item) && !this.m_SelectedKeywords.Contains(item))
						{
							this.m_AvailableKeywords.Add(item);
						}
					}
				}
			}
			this.m_AvailableKeywords.Sort();
		}

		public void OnGUI()
		{
			if (this.m_Data == null || this.m_Data.shader == null || this.m_Data.collection == null)
			{
				base.Close();
				return;
			}
			if (Event.current.type == EventType.Layout)
			{
				return;
			}
			Rect windowRect = new Rect(0f, 0f, base.position.width, base.position.height);
			this.Draw(windowRect);
			if (Event.current.type == EventType.MouseMove)
			{
				base.Repaint();
			}
		}

		private bool KeywordButton(Rect buttonRect, string k, Vector2 areaSize)
		{
			Color color = GUI.color;
			if (buttonRect.yMax > areaSize.y)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.4f);
			}
			bool result = GUI.Button(buttonRect, EditorGUIUtility.TempContent(k), EditorStyles.miniButton);
			GUI.color = color;
			return result;
		}

		private float CalcVerticalSpaceForKeywords()
		{
			return Mathf.Floor((base.position.height - 120f) / 4f);
		}

		private float CalcVerticalSpaceForVariants()
		{
			return (base.position.height - 120f) / 2f;
		}

		private void DrawKeywordsList(ref Rect rect, List<string> keywords, bool clickingAddsToSelected)
		{
			rect.height = this.CalcVerticalSpaceForKeywords();
			List<string> list = (from k in keywords
			select (!string.IsNullOrEmpty(k)) ? k.ToLowerInvariant() : "<no keyword>").ToList<string>();
			GUI.BeginGroup(rect);
			Rect rect2 = new Rect(4f, 0f, rect.width, rect.height);
			List<Rect> flowLayoutedRects = EditorGUIUtility.GetFlowLayoutedRects(rect2, EditorStyles.miniButton, 2f, 2f, list);
			for (int i = 0; i < list.Count; i++)
			{
				if (this.KeywordButton(flowLayoutedRects[i], list[i], rect.size))
				{
					if (clickingAddsToSelected)
					{
						this.m_SelectedKeywords.Add(keywords[i]);
						this.m_SelectedKeywords.Sort();
					}
					else
					{
						this.m_SelectedKeywords.Remove(keywords[i]);
					}
					this.ApplyKeywordFilter();
					GUIUtility.ExitGUI();
				}
			}
			GUI.EndGroup();
			rect.y += rect.height;
		}

		private void DrawSectionHeader(ref Rect rect, string titleString, bool separator)
		{
			rect.y += 6f;
			if (separator)
			{
				rect.height = 3f;
				GUI.Label(rect, GUIContent.none, AddShaderVariantWindow.Styles.sSeparator);
				rect.y += rect.height;
			}
			rect.height = 16f;
			GUI.Label(rect, titleString);
			rect.y += rect.height;
		}

		private void Draw(Rect windowRect)
		{
			Rect position = new Rect(2f, 2f, windowRect.width - 4f, 16f);
			this.DrawSectionHeader(ref position, "Pick shader keywords to narrow down variant list:", false);
			this.DrawKeywordsList(ref position, this.m_AvailableKeywords, true);
			this.DrawSectionHeader(ref position, "Selected keywords:", true);
			this.DrawKeywordsList(ref position, this.m_SelectedKeywords, false);
			this.DrawSectionHeader(ref position, "Shader variants with these keywords (click to select):", true);
			if (this.m_FilteredVariants.Count > 0)
			{
				int num = (int)(this.CalcVerticalSpaceForVariants() / 16f);
				for (int i = 0; i < Mathf.Min(this.m_FilteredVariants.Count, num); i++)
				{
					int num2 = this.m_FilteredVariants[i];
					PassType passType = (PassType)this.m_Data.types[num2];
					bool flag = this.m_SelectedVariants.Contains(num2);
					string text = passType.ToString() + " " + string.Join(" ", this.m_Data.keywords[num2]).ToLowerInvariant();
					bool flag2 = GUI.Toggle(position, flag, text, AddShaderVariantWindow.Styles.sMenuItem);
					position.y += position.height;
					if (flag2 && !flag)
					{
						this.m_SelectedVariants.Add(num2);
					}
					else if (!flag2 && flag)
					{
						this.m_SelectedVariants.Remove(num2);
					}
				}
				if (this.m_FilteredVariants.Count > num)
				{
					GUI.Label(position, string.Format("[{0} more variants skipped]", this.m_FilteredVariants.Count - num), EditorStyles.miniLabel);
					position.y += position.height;
				}
			}
			else
			{
				GUI.Label(position, "No variants with these keywords");
				position.y += position.height;
			}
			position.y = windowRect.height - 2f - 6f - 16f;
			position.height = 16f;
			using (new EditorGUI.DisabledScope(this.m_SelectedVariants.Count == 0))
			{
				if (GUI.Button(position, string.Format("Add {0} selected variants", this.m_SelectedVariants.Count)))
				{
					Undo.RecordObject(this.m_Data.collection, "Add variant");
					for (int j = 0; j < this.m_SelectedVariants.Count; j++)
					{
						int num3 = this.m_SelectedVariants[j];
						ShaderVariantCollection.ShaderVariant variant = new ShaderVariantCollection.ShaderVariant(this.m_Data.shader, (PassType)this.m_Data.types[num3], this.m_Data.keywords[num3]);
						this.m_Data.collection.Add(variant);
					}
					base.Close();
					GUIUtility.ExitGUI();
				}
			}
		}
	}
}
