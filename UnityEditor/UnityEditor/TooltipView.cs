using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TooltipView : GUIView
	{
		private const float MAX_WIDTH = 300f;

		private GUIContent m_tooltip = new GUIContent();

		private Vector2 m_optimalSize;

		private GUIStyle m_Style;

		private Rect m_hoverRect;

		private ContainerWindow m_tooltipContainer;

		private static TooltipView s_guiView;

		private void OnEnable()
		{
			TooltipView.s_guiView = this;
		}

		private void OnDisable()
		{
			TooltipView.s_guiView = null;
		}

		private void OnGUI()
		{
			if (this.m_tooltipContainer != null)
			{
				GUI.Box(new Rect(0f, 0f, this.m_optimalSize.x, this.m_optimalSize.y), this.m_tooltip, this.m_Style);
			}
		}

		private void Setup(string tooltip, Rect rect)
		{
			this.m_hoverRect = rect;
			this.m_tooltip.text = tooltip;
			this.m_Style = EditorStyles.tooltip;
			this.m_Style.wordWrap = false;
			this.m_optimalSize = this.m_Style.CalcSize(this.m_tooltip);
			if (this.m_optimalSize.x > 300f)
			{
				this.m_Style.wordWrap = true;
				this.m_optimalSize.x = 300f;
				this.m_optimalSize.y = this.m_Style.CalcHeight(this.m_tooltip, 300f);
			}
			this.m_tooltipContainer.position = new Rect(Mathf.Floor(this.m_hoverRect.x + this.m_hoverRect.width / 2f - this.m_optimalSize.x / 2f), Mathf.Floor(this.m_hoverRect.y + this.m_hoverRect.height + 10f), this.m_optimalSize.x, this.m_optimalSize.y);
			base.position = new Rect(0f, 0f, this.m_optimalSize.x, this.m_optimalSize.y);
			this.m_tooltipContainer.ShowPopup();
			this.m_tooltipContainer.SetAlpha(1f);
			TooltipView.s_guiView.mouseRayInvisible = true;
			base.RepaintImmediately();
		}

		public static void Show(string tooltip, Rect rect)
		{
			if (TooltipView.s_guiView == null)
			{
				TooltipView.s_guiView = ScriptableObject.CreateInstance<TooltipView>();
				TooltipView.s_guiView.m_tooltipContainer = ScriptableObject.CreateInstance<ContainerWindow>();
				TooltipView.s_guiView.m_tooltipContainer.m_DontSaveToLayout = true;
				TooltipView.s_guiView.m_tooltipContainer.rootView = TooltipView.s_guiView;
				TooltipView.s_guiView.m_tooltipContainer.SetMinMaxSizes(new Vector2(10f, 10f), new Vector2(2000f, 2000f));
			}
			if (!(TooltipView.s_guiView.m_tooltip.text == tooltip) || !(rect == TooltipView.s_guiView.m_hoverRect))
			{
				TooltipView.s_guiView.Setup(tooltip, rect);
			}
		}

		public static void Close()
		{
			if (TooltipView.s_guiView != null)
			{
				TooltipView.s_guiView.m_tooltipContainer.Close();
			}
		}

		public static void SetAlpha(float percent)
		{
			if (TooltipView.s_guiView != null)
			{
				TooltipView.s_guiView.m_tooltipContainer.SetAlpha(percent);
			}
		}
	}
}
