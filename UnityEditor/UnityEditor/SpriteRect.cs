using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class SpriteRect
	{
		[SerializeField]
		private string m_Name;

		[SerializeField]
		private string m_OriginalName;

		[SerializeField]
		private Vector2 m_Pivot;

		[SerializeField]
		private SpriteAlignment m_Alignment;

		[SerializeField]
		private Vector4 m_Border;

		[SerializeField]
		private Rect m_Rect;

		[SerializeField]
		private List<SpriteOutline> m_Outline = new List<SpriteOutline>();

		[SerializeField]
		private float m_TessellationDetail;

		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		public string originalName
		{
			get
			{
				if (this.m_OriginalName == null)
				{
					this.m_OriginalName = this.name;
				}
				return this.m_OriginalName;
			}
			set
			{
				this.m_OriginalName = value;
			}
		}

		public Vector2 pivot
		{
			get
			{
				return this.m_Pivot;
			}
			set
			{
				this.m_Pivot = value;
			}
		}

		public SpriteAlignment alignment
		{
			get
			{
				return this.m_Alignment;
			}
			set
			{
				this.m_Alignment = value;
			}
		}

		public Vector4 border
		{
			get
			{
				return this.m_Border;
			}
			set
			{
				this.m_Border = value;
			}
		}

		public Rect rect
		{
			get
			{
				return this.m_Rect;
			}
			set
			{
				this.m_Rect = value;
			}
		}

		public List<SpriteOutline> outline
		{
			get
			{
				return this.m_Outline;
			}
			set
			{
				this.m_Outline = value;
			}
		}

		public float tessellationDetail
		{
			get
			{
				return this.m_TessellationDetail;
			}
			set
			{
				this.m_TessellationDetail = value;
			}
		}

		public static List<SpriteOutline> AcquireOutline(SerializedProperty outlineSP)
		{
			List<SpriteOutline> list = new List<SpriteOutline>();
			for (int i = 0; i < outlineSP.arraySize; i++)
			{
				SpriteOutline spriteOutline = new SpriteOutline();
				SerializedProperty arrayElementAtIndex = outlineSP.GetArrayElementAtIndex(i);
				for (int j = 0; j < arrayElementAtIndex.arraySize; j++)
				{
					Vector2 vector2Value = arrayElementAtIndex.GetArrayElementAtIndex(j).vector2Value;
					spriteOutline.Add(vector2Value);
				}
				list.Add(spriteOutline);
			}
			return list;
		}

		public static void ApplyOutlineChanges(SerializedProperty outlineSP, List<SpriteOutline> outline)
		{
			outlineSP.ClearArray();
			for (int i = 0; i < outline.Count; i++)
			{
				outlineSP.InsertArrayElementAtIndex(i);
				SpriteOutline spriteOutline = outline[i];
				SerializedProperty arrayElementAtIndex = outlineSP.GetArrayElementAtIndex(i);
				arrayElementAtIndex.ClearArray();
				for (int j = 0; j < spriteOutline.Count; j++)
				{
					arrayElementAtIndex.InsertArrayElementAtIndex(j);
					arrayElementAtIndex.GetArrayElementAtIndex(j).vector2Value = spriteOutline[j];
				}
			}
		}

		public void ApplyToSerializedProperty(SerializedProperty sp)
		{
			sp.FindPropertyRelative("m_Rect").rectValue = this.rect;
			sp.FindPropertyRelative("m_Border").vector4Value = this.border;
			sp.FindPropertyRelative("m_Name").stringValue = this.name;
			sp.FindPropertyRelative("m_Alignment").intValue = (int)this.alignment;
			sp.FindPropertyRelative("m_Pivot").vector2Value = this.pivot;
			sp.FindPropertyRelative("m_TessellationDetail").floatValue = this.tessellationDetail;
			SerializedProperty serializedProperty = sp.FindPropertyRelative("m_Outline");
			serializedProperty.ClearArray();
			if (this.outline != null)
			{
				SpriteRect.ApplyOutlineChanges(serializedProperty, this.outline);
			}
		}

		public void LoadFromSerializedProperty(SerializedProperty sp)
		{
			this.rect = sp.FindPropertyRelative("m_Rect").rectValue;
			this.border = sp.FindPropertyRelative("m_Border").vector4Value;
			this.name = sp.FindPropertyRelative("m_Name").stringValue;
			this.alignment = (SpriteAlignment)sp.FindPropertyRelative("m_Alignment").intValue;
			this.pivot = SpriteEditorUtility.GetPivotValue(this.alignment, sp.FindPropertyRelative("m_Pivot").vector2Value);
			this.tessellationDetail = sp.FindPropertyRelative("m_TessellationDetail").floatValue;
			SerializedProperty outlineSP = sp.FindPropertyRelative("m_Outline");
			this.outline = SpriteRect.AcquireOutline(outlineSP);
		}
	}
}
