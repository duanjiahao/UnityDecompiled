using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMButtonGrid : IMElement
	{
		private GUIContent[] s_EmptyContents = new GUIContent[0];

		private GUIContent[] m_Contents;

		private GUIStyle m_FirstStyle = GUIStyle.none;

		private GUIStyle m_MidStyle = GUIStyle.none;

		private GUIStyle m_LastStyle = GUIStyle.none;

		public GUIContent[] contents
		{
			get
			{
				return this.m_Contents;
			}
			set
			{
				this.m_Contents = (value ?? this.s_EmptyContents);
			}
		}

		public int xCount
		{
			get;
			set;
		}

		public int selected
		{
			get;
			set;
		}

		public GUIStyle firstStyle
		{
			get
			{
				return this.m_FirstStyle;
			}
			set
			{
				this.m_FirstStyle = (value ?? GUIStyle.none);
			}
		}

		public GUIStyle midStyle
		{
			get
			{
				return this.m_MidStyle;
			}
			set
			{
				this.m_MidStyle = (value ?? GUIStyle.none);
			}
		}

		public GUIStyle lastStyle
		{
			get
			{
				return this.m_LastStyle;
			}
			set
			{
				this.m_LastStyle = (value ?? GUIStyle.none);
			}
		}

		protected override int DoGenerateControlID()
		{
			return GUIUtility.GetControlID("IMButtonGrid".GetHashCode(), base.focusType, base.position);
		}

		protected override bool DoMouseDown(MouseEventArgs args)
		{
			bool result;
			if (base.position.Contains(args.mousePosition))
			{
				int count;
				float elemWidth;
				float elemHeight;
				if (this.ComputeElemDimensions(out count, out elemWidth, out elemHeight))
				{
					Rect[] buttonRects = IMButtonGrid.CalcMouseRects(base.position, count, this.xCount, elemWidth, elemHeight, base.style, this.firstStyle, this.midStyle, this.lastStyle, false);
					if (this.GetButtonGridMouseSelection(buttonRects, args.mousePosition, true) != -1)
					{
						GUIUtility.hotControl = base.id;
						result = true;
						return result;
					}
				}
			}
			result = false;
			return result;
		}

		protected override bool DoMouseDrag(MouseEventArgs args)
		{
			return GUIUtility.hotControl == base.id;
		}

		protected override bool DoMouseUp(MouseEventArgs args)
		{
			bool result;
			if (GUIUtility.hotControl == base.id)
			{
				int count;
				float elemWidth;
				float elemHeight;
				if (this.ComputeElemDimensions(out count, out elemWidth, out elemHeight))
				{
					GUIUtility.hotControl = 0;
					Rect[] buttonRects = IMButtonGrid.CalcMouseRects(base.position, count, this.xCount, elemWidth, elemHeight, base.style, this.firstStyle, this.midStyle, this.lastStyle, false);
					int buttonGridMouseSelection = this.GetButtonGridMouseSelection(buttonRects, args.mousePosition, true);
					GUI.changed = true;
					this.selected = buttonGridMouseSelection;
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		internal override void DoRepaint(IStylePainter args)
		{
			int num;
			float elemWidth;
			float elemHeight;
			if (this.ComputeElemDimensions(out num, out elemWidth, out elemHeight))
			{
				GUIStyle gUIStyle = GUIStyle.none;
				GUIClip.Internal_Push(base.position, Vector2.zero, Vector2.zero, false);
				Rect position = new Rect(0f, 0f, base.position.width, base.position.height);
				Rect[] array = IMButtonGrid.CalcMouseRects(position, num, this.xCount, elemWidth, elemHeight, base.style, this.firstStyle, this.midStyle, this.lastStyle, false);
				Vector2 mousePos = args.mousePosition - base.position.position;
				int buttonGridMouseSelection = this.GetButtonGridMouseSelection(array, mousePos, base.id == GUIUtility.hotControl);
				bool flag = position.Contains(args.mousePosition);
				GUIUtility.mouseUsed |= flag;
				for (int i = 0; i < num; i++)
				{
					GUIStyle gUIStyle2;
					if (i != 0)
					{
						gUIStyle2 = this.midStyle;
					}
					else
					{
						gUIStyle2 = this.firstStyle;
					}
					if (i == num - 1)
					{
						gUIStyle2 = this.lastStyle;
					}
					if (num == 1)
					{
						gUIStyle2 = base.style;
					}
					if (i != this.selected)
					{
						gUIStyle2.Draw(array[i], this.contents[i], i == buttonGridMouseSelection && (this.enabled || base.id == GUIUtility.hotControl) && (base.id == GUIUtility.hotControl || GUIUtility.hotControl == 0), base.id == GUIUtility.hotControl && this.enabled, false, false);
					}
					else
					{
						gUIStyle = gUIStyle2;
					}
				}
				if (this.selected < num && this.selected > -1)
				{
					gUIStyle.Draw(array[this.selected], this.contents[this.selected], this.selected == buttonGridMouseSelection && (this.enabled || base.id == GUIUtility.hotControl) && (base.id == GUIUtility.hotControl || GUIUtility.hotControl == 0), base.id == GUIUtility.hotControl, true, false);
				}
				if (buttonGridMouseSelection >= 0)
				{
					GUI.tooltip = this.contents[buttonGridMouseSelection].tooltip;
				}
				GUIClip.Internal_Pop();
			}
		}

		private static Rect[] CalcMouseRects(Rect position, int count, int xCount, float elemWidth, float elemHeight, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle, bool addBorders)
		{
			int num = 0;
			float num2 = position.xMin;
			float num3 = position.yMin;
			GUIStyle gUIStyle = style;
			Rect[] array = new Rect[count];
			if (count > 1)
			{
				gUIStyle = firstStyle;
			}
			for (int i = 0; i < count; i++)
			{
				if (!addBorders)
				{
					array[i] = new Rect(num2, num3, elemWidth, elemHeight);
				}
				else
				{
					array[i] = gUIStyle.margin.Add(new Rect(num2, num3, elemWidth, elemHeight));
				}
				array[i].width = Mathf.Round(array[i].xMax) - Mathf.Round(array[i].x);
				array[i].x = Mathf.Round(array[i].x);
				GUIStyle gUIStyle2 = midStyle;
				if (i == count - 2)
				{
					gUIStyle2 = lastStyle;
				}
				num2 += elemWidth + (float)Mathf.Max(gUIStyle.margin.right, gUIStyle2.margin.left);
				num++;
				if (num >= xCount)
				{
					num = 0;
					num3 += elemHeight + (float)Mathf.Max(style.margin.top, style.margin.bottom);
					num2 = position.xMin;
				}
			}
			return array;
		}

		private int GetButtonGridMouseSelection(Rect[] buttonRects, Vector2 mousePos, bool findNearest)
		{
			int result;
			for (int i = 0; i < buttonRects.Length; i++)
			{
				if (buttonRects[i].Contains(mousePos))
				{
					result = i;
					return result;
				}
			}
			if (!findNearest)
			{
				result = -1;
				return result;
			}
			float num = 3.40282347E+38f;
			int num2 = -1;
			for (int j = 0; j < buttonRects.Length; j++)
			{
				Rect rect = buttonRects[j];
				Vector2 b = new Vector2(Mathf.Clamp(mousePos.x, rect.xMin, rect.xMax), Mathf.Clamp(mousePos.y, rect.yMin, rect.yMax));
				float sqrMagnitude = (mousePos - b).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num2 = j;
					num = sqrMagnitude;
				}
			}
			result = num2;
			return result;
		}

		private int CalcTotalHorizSpacing()
		{
			int result;
			if (this.xCount < 2)
			{
				result = 0;
			}
			else if (this.xCount == 2)
			{
				result = Mathf.Max(this.firstStyle.margin.right, this.lastStyle.margin.left);
			}
			else
			{
				int num = Mathf.Max(this.midStyle.margin.left, this.midStyle.margin.right);
				result = Mathf.Max(this.firstStyle.margin.right, this.midStyle.margin.left) + Mathf.Max(this.midStyle.margin.right, this.lastStyle.margin.left) + num * (this.xCount - 3);
			}
			return result;
		}

		private bool ComputeElemDimensions(out int count, out float elemWidth, out float elemHeight)
		{
			count = this.contents.Length;
			elemWidth = 0f;
			elemHeight = 0f;
			bool result;
			if (count == 0)
			{
				result = false;
			}
			else if (this.xCount <= 0)
			{
				Debug.LogWarning("You are trying to create a SelectionGrid with zero or less elements to be displayed in the horizontal direction. Set xCount to a positive value.");
				result = false;
			}
			else
			{
				int num = count / this.xCount;
				if (count % this.xCount != 0)
				{
					num++;
				}
				float num2 = (float)this.CalcTotalHorizSpacing();
				float num3 = (float)(Mathf.Max(base.style.margin.top, base.style.margin.bottom) * (num - 1));
				elemWidth = (base.position.width - num2) / (float)this.xCount;
				elemHeight = (base.position.height - num3) / (float)num;
				if (base.style.fixedWidth != 0f)
				{
					elemWidth = base.style.fixedWidth;
				}
				if (base.style.fixedHeight != 0f)
				{
					elemHeight = base.style.fixedHeight;
				}
				result = true;
			}
			return result;
		}
	}
}
