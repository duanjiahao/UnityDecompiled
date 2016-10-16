using System;
using UnityEngine;

namespace UnityEditor
{
	public class ObjectPreview : IPreviewable
	{
		private class Styles
		{
			public GUIStyle preBackground = "preBackground";

			public GUIStyle preBackgroundSolid = new GUIStyle("preBackground");

			public GUIStyle previewMiniLabel = new GUIStyle(EditorStyles.whiteMiniLabel);

			public GUIStyle dropShadowLabelStyle = new GUIStyle("PreOverlayLabel");

			public Styles()
			{
				this.preBackgroundSolid.overflow = this.preBackgroundSolid.border;
				this.previewMiniLabel.alignment = TextAnchor.UpperCenter;
			}
		}

		private const int kPreviewLabelHeight = 12;

		private const int kPreviewMinSize = 55;

		private const int kGridTargetCount = 25;

		private const int kGridSpacing = 10;

		private const int kPreviewLabelPadding = 5;

		private static ObjectPreview.Styles s_Styles;

		protected UnityEngine.Object[] m_Targets;

		protected int m_ReferenceTargetIndex;

		public virtual UnityEngine.Object target
		{
			get
			{
				return this.m_Targets[this.m_ReferenceTargetIndex];
			}
		}

		public virtual void Initialize(UnityEngine.Object[] targets)
		{
			this.m_ReferenceTargetIndex = 0;
			this.m_Targets = targets;
		}

		public virtual bool MoveNextTarget()
		{
			this.m_ReferenceTargetIndex++;
			return this.m_ReferenceTargetIndex < this.m_Targets.Length - 1;
		}

		public virtual void ResetTarget()
		{
			this.m_ReferenceTargetIndex = 0;
		}

		public virtual bool HasPreviewGUI()
		{
			return false;
		}

		public virtual GUIContent GetPreviewTitle()
		{
			GUIContent gUIContent = new GUIContent();
			if (this.m_Targets.Length == 1)
			{
				gUIContent.text = this.target.name;
			}
			else
			{
				gUIContent.text = this.m_Targets.Length + " ";
				if (this.target is MonoBehaviour)
				{
					GUIContent expr_58 = gUIContent;
					expr_58.text += MonoScript.FromMonoBehaviour(this.target as MonoBehaviour).GetClass().Name;
				}
				else
				{
					GUIContent expr_88 = gUIContent;
					expr_88.text += ObjectNames.NicifyVariableName(ObjectNames.GetClassName(this.target));
				}
				GUIContent expr_A9 = gUIContent;
				expr_A9.text += "s";
			}
			return gUIContent;
		}

		public virtual void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (Event.current.type == EventType.Repaint)
			{
				background.Draw(r, false, false, false, false);
			}
		}

		public virtual void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			this.OnPreviewGUI(r, background);
		}

		public virtual void OnPreviewSettings()
		{
		}

		public virtual string GetInfoString()
		{
			return string.Empty;
		}

		public void DrawPreview(Rect previewArea)
		{
			ObjectPreview.DrawPreview(this, previewArea, this.m_Targets);
		}

		public virtual void ReloadPreviewInstances()
		{
		}

		internal static void DrawPreview(IPreviewable defaultPreview, Rect previewArea, UnityEngine.Object[] targets)
		{
			if (ObjectPreview.s_Styles == null)
			{
				ObjectPreview.s_Styles = new ObjectPreview.Styles();
			}
			string text = string.Empty;
			Event current = Event.current;
			if (targets.Length > 1)
			{
				Rect rect = new RectOffset(16, 16, 20, 25).Remove(previewArea);
				int num = Mathf.Max(1, Mathf.FloorToInt((rect.height + 10f) / 77f));
				int num2 = Mathf.Max(1, Mathf.FloorToInt((rect.width + 10f) / 65f));
				int num3 = num * num2;
				int num4 = Mathf.Min(targets.Length, 25);
				bool flag = true;
				int[] array = new int[]
				{
					num2,
					num
				};
				if (num4 < num3)
				{
					array = ObjectPreview.GetGridDivision(rect, num4, 12);
					flag = false;
				}
				int num5 = Mathf.Min(array[0] * array[1], targets.Length);
				rect.width += 10f;
				rect.height += 10f;
				Vector2 vector = new Vector2((float)Mathf.FloorToInt(rect.width / (float)array[0] - 10f), (float)Mathf.FloorToInt(rect.height / (float)array[1] - 10f));
				float num6 = Mathf.Min(vector.x, vector.y - 12f);
				if (flag)
				{
					num6 = Mathf.Min(num6, 55f);
				}
				bool flag2 = current.type == EventType.MouseDown && current.button == 0 && current.clickCount == 2 && previewArea.Contains(current.mousePosition);
				defaultPreview.ResetTarget();
				for (int i = 0; i < num5; i++)
				{
					Rect position = new Rect(rect.x + (float)(i % array[0]) * rect.width / (float)array[0], rect.y + (float)(i / array[0]) * rect.height / (float)array[1], vector.x, vector.y);
					if (flag2 && position.Contains(Event.current.mousePosition))
					{
						Selection.objects = new UnityEngine.Object[]
						{
							defaultPreview.target
						};
					}
					position.height -= 12f;
					Rect position2 = new Rect(position.x + (position.width - num6) * 0.5f, position.y + (position.height - num6) * 0.5f, num6, num6);
					GUI.BeginGroup(position2);
					Editor.m_AllowMultiObjectAccess = false;
					defaultPreview.OnInteractivePreviewGUI(new Rect(0f, 0f, num6, num6), ObjectPreview.s_Styles.preBackgroundSolid);
					Editor.m_AllowMultiObjectAccess = true;
					GUI.EndGroup();
					position.y = position2.yMax;
					position.height = 16f;
					GUI.Label(position, targets[i].name, ObjectPreview.s_Styles.previewMiniLabel);
					defaultPreview.MoveNextTarget();
				}
				defaultPreview.ResetTarget();
				if (Event.current.type == EventType.Repaint)
				{
					text = string.Format("Previewing {0} of {1} Objects", num5, targets.Length);
				}
			}
			else
			{
				defaultPreview.OnInteractivePreviewGUI(previewArea, ObjectPreview.s_Styles.preBackground);
				if (Event.current.type == EventType.Repaint)
				{
					text = defaultPreview.GetInfoString();
					if (text != string.Empty)
					{
						text = text.Replace("\n", "   ");
						text = string.Format("{0}\n{1}", defaultPreview.target.name, text);
					}
				}
			}
			if (Event.current.type == EventType.Repaint && text != string.Empty)
			{
				float num7 = ObjectPreview.s_Styles.dropShadowLabelStyle.CalcHeight(GUIContent.Temp(text), previewArea.width);
				EditorGUI.DropShadowLabel(new Rect(previewArea.x, previewArea.yMax - num7 - 5f, previewArea.width, num7), text);
			}
		}

		private static int[] GetGridDivision(Rect rect, int minimumNr, int labelHeight)
		{
			float num = Mathf.Sqrt(rect.width * rect.height / (float)minimumNr);
			int num2 = Mathf.FloorToInt(rect.width / num);
			int num3 = Mathf.FloorToInt(rect.height / (num + (float)labelHeight));
			while (num2 * num3 < minimumNr)
			{
				float num4 = ObjectPreview.AbsRatioDiff((float)(num2 + 1) / rect.width, (float)num3 / (rect.height - (float)(num3 * labelHeight)));
				float num5 = ObjectPreview.AbsRatioDiff((float)num2 / rect.width, (float)(num3 + 1) / (rect.height - (float)((num3 + 1) * labelHeight)));
				if (num4 < num5)
				{
					num2++;
					if (num2 * num3 > minimumNr)
					{
						num3 = Mathf.CeilToInt((float)minimumNr / (float)num2);
					}
				}
				else
				{
					num3++;
					if (num2 * num3 > minimumNr)
					{
						num2 = Mathf.CeilToInt((float)minimumNr / (float)num3);
					}
				}
			}
			return new int[]
			{
				num2,
				num3
			};
		}

		private static float AbsRatioDiff(float x, float y)
		{
			return Mathf.Max(x / y, y / x);
		}
	}
}
