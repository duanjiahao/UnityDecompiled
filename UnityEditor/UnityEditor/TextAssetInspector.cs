using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(TextAsset))]
	internal class TextAssetInspector : Editor
	{
		private const int kMaxChars = 7000;

		[NonSerialized]
		private GUIStyle m_TextStyle;

		public override void OnInspectorGUI()
		{
			if (this.m_TextStyle == null)
			{
				this.m_TextStyle = "ScriptText";
			}
			bool enabled = GUI.enabled;
			GUI.enabled = true;
			TextAsset textAsset = base.target as TextAsset;
			if (textAsset != null)
			{
				string text;
				if (base.targets.Length > 1)
				{
					text = this.targetTitle;
				}
				else
				{
					text = textAsset.ToString();
					if (text.Length > 7000)
					{
						text = text.Substring(0, 7000) + "...\n\n<...etc...>";
					}
				}
				Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.TempContent(text), this.m_TextStyle);
				rect.x = 0f;
				rect.y -= 3f;
				rect.width = GUIClip.visibleRect.width + 1f;
				GUI.Box(rect, text, this.m_TextStyle);
			}
			GUI.enabled = enabled;
		}
	}
}
