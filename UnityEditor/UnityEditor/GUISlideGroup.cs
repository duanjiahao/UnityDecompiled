using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class GUISlideGroup
	{
		private class SlideGroupInternal : GUILayoutGroup
		{
			private int m_ID;

			private GUISlideGroup m_Owner;

			internal Rect m_FinalRect;

			public void SetID(GUISlideGroup owner, int id)
			{
				this.m_ID = id;
				this.m_Owner = owner;
			}

			public override void SetHorizontal(float x, float width)
			{
				this.m_FinalRect.x = x;
				this.m_FinalRect.width = width;
				base.SetHorizontal(x, width);
			}

			public override void SetVertical(float y, float height)
			{
				this.m_FinalRect.y = y;
				this.m_FinalRect.height = height;
				Rect rect = new Rect(this.rect.x, y, this.rect.width, height);
				bool flag;
				rect = this.m_Owner.GetRect(this.m_ID, rect, out flag);
				if (flag)
				{
					base.SetHorizontal(rect.x, rect.width);
				}
				base.SetVertical(rect.y, rect.height);
			}
		}

		internal static GUISlideGroup current = null;

		private Dictionary<int, Rect> animIDs = new Dictionary<int, Rect>();

		private const float kLerp = 0.1f;

		private const float kSnap = 0.5f;

		public void Begin()
		{
			if (GUISlideGroup.current != null)
			{
				Debug.LogError("You cannot nest animGroups");
			}
			else
			{
				GUISlideGroup.current = this;
			}
		}

		public void End()
		{
			GUISlideGroup.current = null;
		}

		public void Reset()
		{
			GUISlideGroup.current = null;
			this.animIDs.Clear();
		}

		public Rect BeginHorizontal(int id, params GUILayoutOption[] options)
		{
			GUISlideGroup.SlideGroupInternal slideGroupInternal = (GUISlideGroup.SlideGroupInternal)GUILayoutUtility.BeginLayoutGroup(GUIStyle.none, options, typeof(GUISlideGroup.SlideGroupInternal));
			slideGroupInternal.SetID(this, id);
			slideGroupInternal.isVertical = false;
			return slideGroupInternal.m_FinalRect;
		}

		public void EndHorizontal()
		{
			GUILayoutUtility.EndLayoutGroup();
		}

		public Rect GetRect(int id, Rect r)
		{
			Rect result;
			if (Event.current.type != EventType.Repaint)
			{
				result = r;
			}
			else
			{
				bool flag;
				result = this.GetRect(id, r, out flag);
			}
			return result;
		}

		private Rect GetRect(int id, Rect r, out bool changed)
		{
			Rect result;
			if (!this.animIDs.ContainsKey(id))
			{
				this.animIDs.Add(id, r);
				changed = false;
				result = r;
			}
			else
			{
				Rect rect = this.animIDs[id];
				if (rect.y != r.y || rect.height != r.height || rect.x != r.x || rect.width != r.width)
				{
					float t = 0.1f;
					if (Mathf.Abs(rect.y - r.y) > 0.5f)
					{
						r.y = Mathf.Lerp(rect.y, r.y, t);
					}
					if (Mathf.Abs(rect.height - r.height) > 0.5f)
					{
						r.height = Mathf.Lerp(rect.height, r.height, t);
					}
					if (Mathf.Abs(rect.x - r.x) > 0.5f)
					{
						r.x = Mathf.Lerp(rect.x, r.x, t);
					}
					if (Mathf.Abs(rect.width - r.width) > 0.5f)
					{
						r.width = Mathf.Lerp(rect.width, r.width, t);
					}
					this.animIDs[id] = r;
					changed = true;
					HandleUtility.Repaint();
				}
				else
				{
					changed = false;
				}
				result = r;
			}
			return result;
		}
	}
}
