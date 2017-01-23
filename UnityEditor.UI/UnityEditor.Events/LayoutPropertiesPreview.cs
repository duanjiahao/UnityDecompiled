using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.Events
{
	[CustomPreview(typeof(GameObject))]
	internal class LayoutPropertiesPreview : ObjectPreview
	{
		private class Styles
		{
			public GUIStyle labelStyle = new GUIStyle(EditorStyles.label);

			public GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);

			public Styles()
			{
				Color textColor = new Color(0.7f, 0.7f, 0.7f);
				this.labelStyle.padding.right += 4;
				this.labelStyle.normal.textColor = textColor;
				this.headerStyle.padding.right += 4;
				this.headerStyle.normal.textColor = textColor;
			}
		}

		private const float kLabelWidth = 110f;

		private const float kValueWidth = 100f;

		private GUIContent m_Title;

		private LayoutPropertiesPreview.Styles m_Styles = new LayoutPropertiesPreview.Styles();

		public override void Initialize(UnityEngine.Object[] targets)
		{
			base.Initialize(targets);
		}

		public override GUIContent GetPreviewTitle()
		{
			if (this.m_Title == null)
			{
				this.m_Title = new GUIContent("Layout Properties");
			}
			return this.m_Title;
		}

		public override bool HasPreviewGUI()
		{
			GameObject gameObject = this.target as GameObject;
			return gameObject && gameObject.GetComponent(typeof(ILayoutElement)) != null;
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (Event.current.type == EventType.Repaint)
			{
				if (this.m_Styles == null)
				{
					this.m_Styles = new LayoutPropertiesPreview.Styles();
				}
				GameObject gameObject = this.target as GameObject;
				RectTransform rectTransform = gameObject.transform as RectTransform;
				if (!(rectTransform == null))
				{
					RectOffset rectOffset = new RectOffset(-5, -5, -5, -5);
					r = rectOffset.Add(r);
					r.height = EditorGUIUtility.singleLineHeight;
					Rect position = r;
					Rect position2 = r;
					Rect position3 = r;
					position.width = 110f;
					position2.xMin += 110f;
					position2.width = 100f;
					position3.xMin += 210f;
					GUI.Label(position, "Property", this.m_Styles.headerStyle);
					GUI.Label(position2, "Value", this.m_Styles.headerStyle);
					GUI.Label(position3, "Source", this.m_Styles.headerStyle);
					position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
					position2.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
					position3.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
					ILayoutElement source = null;
					this.ShowProp(ref position, ref position2, ref position3, "Min Width", LayoutUtility.GetLayoutProperty(rectTransform, (ILayoutElement e) => e.minWidth, 0f, out source).ToString(), source);
					this.ShowProp(ref position, ref position2, ref position3, "Min Height", LayoutUtility.GetLayoutProperty(rectTransform, (ILayoutElement e) => e.minHeight, 0f, out source).ToString(), source);
					this.ShowProp(ref position, ref position2, ref position3, "Preferred Width", LayoutUtility.GetLayoutProperty(rectTransform, (ILayoutElement e) => e.preferredWidth, 0f, out source).ToString(), source);
					this.ShowProp(ref position, ref position2, ref position3, "Preferred Height", LayoutUtility.GetLayoutProperty(rectTransform, (ILayoutElement e) => e.preferredHeight, 0f, out source).ToString(), source);
					float layoutProperty = LayoutUtility.GetLayoutProperty(rectTransform, (ILayoutElement e) => e.flexibleWidth, 0f, out source);
					this.ShowProp(ref position, ref position2, ref position3, "Flexible Width", (layoutProperty <= 0f) ? "disabled" : ("enabled (" + layoutProperty.ToString() + ")"), source);
					layoutProperty = LayoutUtility.GetLayoutProperty(rectTransform, (ILayoutElement e) => e.flexibleHeight, 0f, out source);
					this.ShowProp(ref position, ref position2, ref position3, "Flexible Height", (layoutProperty <= 0f) ? "disabled" : ("enabled (" + layoutProperty.ToString() + ")"), source);
					if (!rectTransform.GetComponent<LayoutElement>())
					{
						Rect position4 = new Rect(position.x, position.y + 10f, r.width, EditorGUIUtility.singleLineHeight);
						GUI.Label(position4, "Add a LayoutElement to override values.", this.m_Styles.labelStyle);
					}
				}
			}
		}

		private void ShowProp(ref Rect labelRect, ref Rect valueRect, ref Rect sourceRect, string label, string value, ILayoutElement source)
		{
			GUI.Label(labelRect, label, this.m_Styles.labelStyle);
			GUI.Label(valueRect, value, this.m_Styles.labelStyle);
			GUI.Label(sourceRect, (source != null) ? source.GetType().Name : "none", this.m_Styles.labelStyle);
			labelRect.y += EditorGUIUtility.singleLineHeight;
			valueRect.y += EditorGUIUtility.singleLineHeight;
			sourceRect.y += EditorGUIUtility.singleLineHeight;
		}
	}
}
