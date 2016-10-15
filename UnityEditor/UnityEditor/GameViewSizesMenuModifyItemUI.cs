using System;
using UnityEngine;

namespace UnityEditor
{
	internal class GameViewSizesMenuModifyItemUI : FlexibleMenuModifyItemUI
	{
		private class Styles
		{
			public GUIContent headerAdd = new GUIContent("Add");

			public GUIContent headerEdit = new GUIContent("Edit");

			public GUIContent typeName = new GUIContent("Type");

			public GUIContent widthHeightText = new GUIContent("Width & Height");

			public GUIContent optionalText = new GUIContent("Label");

			public GUIContent ok = new GUIContent("OK");

			public GUIContent cancel = new GUIContent("Cancel");

			public GUIContent[] typeNames = new GUIContent[]
			{
				new GUIContent("Aspect Ratio"),
				new GUIContent("Fixed Resolution")
			};
		}

		private static GameViewSizesMenuModifyItemUI.Styles s_Styles;

		private GameViewSize m_GameViewSize;

		public override void OnClose()
		{
			this.m_GameViewSize = null;
			base.OnClose();
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(230f, 140f);
		}

		public override void OnGUI(Rect rect)
		{
			if (GameViewSizesMenuModifyItemUI.s_Styles == null)
			{
				GameViewSizesMenuModifyItemUI.s_Styles = new GameViewSizesMenuModifyItemUI.Styles();
			}
			GameViewSize gameViewSize = this.m_Object as GameViewSize;
			if (gameViewSize == null)
			{
				Debug.LogError("Invalid object");
				return;
			}
			if (this.m_GameViewSize == null)
			{
				this.m_GameViewSize = new GameViewSize(gameViewSize);
			}
			bool flag = this.m_GameViewSize.width > 0 && this.m_GameViewSize.height > 0;
			GUILayout.Space(3f);
			GUILayout.Label((this.m_MenuType != FlexibleMenuModifyItemUI.MenuType.Add) ? GameViewSizesMenuModifyItemUI.s_Styles.headerEdit : GameViewSizesMenuModifyItemUI.s_Styles.headerAdd, EditorStyles.boldLabel, new GUILayoutOption[0]);
			Rect rect2 = GUILayoutUtility.GetRect(1f, 1f);
			FlexibleMenu.DrawRect(rect2, (!EditorGUIUtility.isProSkin) ? new Color(0.6f, 0.6f, 0.6f, 1.333f) : new Color(0.32f, 0.32f, 0.32f, 1.333f));
			GUILayout.Space(4f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(GameViewSizesMenuModifyItemUI.s_Styles.optionalText, new GUILayoutOption[]
			{
				GUILayout.Width(90f)
			});
			GUILayout.Space(10f);
			this.m_GameViewSize.baseText = EditorGUILayout.TextField(this.m_GameViewSize.baseText, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(GameViewSizesMenuModifyItemUI.s_Styles.typeName, new GUILayoutOption[]
			{
				GUILayout.Width(90f)
			});
			GUILayout.Space(10f);
			this.m_GameViewSize.sizeType = (GameViewSizeType)EditorGUILayout.Popup((int)this.m_GameViewSize.sizeType, GameViewSizesMenuModifyItemUI.s_Styles.typeNames, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(GameViewSizesMenuModifyItemUI.s_Styles.widthHeightText, new GUILayoutOption[]
			{
				GUILayout.Width(90f)
			});
			GUILayout.Space(10f);
			this.m_GameViewSize.width = EditorGUILayout.IntField(this.m_GameViewSize.width, new GUILayoutOption[0]);
			GUILayout.Space(5f);
			this.m_GameViewSize.height = EditorGUILayout.IntField(this.m_GameViewSize.height, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			float num = 10f;
			float cropWidth = rect.width - 2f * num;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(num);
			GUILayout.FlexibleSpace();
			string text = this.m_GameViewSize.displayText;
			using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(text)))
			{
				if (string.IsNullOrEmpty(text))
				{
					text = "Result";
				}
				else
				{
					text = this.GetCroppedText(text, cropWidth, EditorStyles.label);
				}
				GUILayout.Label(GUIContent.Temp(text), EditorStyles.label, new GUILayoutOption[0]);
			}
			GUILayout.FlexibleSpace();
			GUILayout.Space(num);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			if (GUILayout.Button(GameViewSizesMenuModifyItemUI.s_Styles.cancel, new GUILayoutOption[0]))
			{
				base.editorWindow.Close();
			}
			using (new EditorGUI.DisabledScope(!flag))
			{
				if (GUILayout.Button(GameViewSizesMenuModifyItemUI.s_Styles.ok, new GUILayoutOption[0]))
				{
					gameViewSize.Set(this.m_GameViewSize);
					base.Accepted();
					base.editorWindow.Close();
				}
			}
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
		}

		private string GetCroppedText(string fullText, float cropWidth, GUIStyle style)
		{
			int numCharactersThatFitWithinWidth = style.GetNumCharactersThatFitWithinWidth(fullText, cropWidth);
			if (numCharactersThatFitWithinWidth == -1)
			{
				return fullText;
			}
			if (numCharactersThatFitWithinWidth > 1 && numCharactersThatFitWithinWidth != fullText.Length)
			{
				return fullText.Substring(0, numCharactersThatFitWithinWidth - 1) + "…";
			}
			return fullText;
		}
	}
}
