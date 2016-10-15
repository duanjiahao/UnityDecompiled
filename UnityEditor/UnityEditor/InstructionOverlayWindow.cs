using System;
using UnityEngine;

namespace UnityEditor
{
	internal class InstructionOverlayWindow : EditorWindow
	{
		private class Styles
		{
			public GUIStyle solidColor;

			public Styles()
			{
				this.solidColor = new GUIStyle();
				this.solidColor.normal.background = EditorGUIUtility.whiteTexture;
			}
		}

		private static InstructionOverlayWindow.Styles s_Styles;

		private GUIView m_InspectedGUIView;

		private Rect m_InstructionRect;

		private GUIStyle m_InstructionStyle;

		private RenderTexture m_RenderTexture;

		[NonSerialized]
		private bool m_RenderTextureNeedsRefresh;

		private InstructionOverlayWindow.Styles styles
		{
			get
			{
				if (InstructionOverlayWindow.s_Styles == null)
				{
					InstructionOverlayWindow.s_Styles = new InstructionOverlayWindow.Styles();
				}
				return InstructionOverlayWindow.s_Styles;
			}
		}

		private void Start()
		{
			base.minSize = Vector2.zero;
			this.m_Parent.window.m_DontSaveToLayout = true;
		}

		public void SetTransparent(float d)
		{
			this.m_Parent.window.SetAlpha(d);
			this.m_Parent.window.SetInvisible();
		}

		public void Show(GUIView view, Rect instructionRect, GUIStyle style)
		{
			base.minSize = Vector2.zero;
			this.m_InstructionStyle = style;
			this.m_InspectedGUIView = view;
			this.m_InstructionRect = instructionRect;
			Rect position = new Rect(instructionRect);
			position.x += this.m_InspectedGUIView.screenPosition.x;
			position.y += this.m_InspectedGUIView.screenPosition.y;
			base.position = position;
			this.m_RenderTextureNeedsRefresh = true;
			base.ShowWithMode(ShowMode.NoShadow);
			this.m_Parent.window.m_DontSaveToLayout = true;
			base.Repaint();
		}

		private void DoRefreshRenderTexture()
		{
			if (this.m_RenderTexture == null)
			{
				this.m_RenderTexture = new RenderTexture(Mathf.CeilToInt(this.m_InstructionRect.width), Mathf.CeilToInt(this.m_InstructionRect.height), 24);
				this.m_RenderTexture.Create();
			}
			else if ((float)this.m_RenderTexture.width != this.m_InstructionRect.width || (float)this.m_RenderTexture.height != this.m_InstructionRect.height)
			{
				this.m_RenderTexture.Release();
				this.m_RenderTexture.width = Mathf.CeilToInt(this.m_InstructionRect.width);
				this.m_RenderTexture.height = Mathf.CeilToInt(this.m_InstructionRect.height);
				this.m_RenderTexture.Create();
			}
			this.m_RenderTextureNeedsRefresh = false;
			base.Repaint();
		}

		private void Update()
		{
			if (this.m_RenderTextureNeedsRefresh)
			{
				this.DoRefreshRenderTexture();
			}
		}

		private void OnFocus()
		{
			EditorWindow.GetWindow<GUIViewDebuggerWindow>();
		}

		private void OnGUI()
		{
			Color backgroundColor = new Color(0.76f, 0.87f, 0.71f);
			Color backgroundColor2 = new Color(0.62f, 0.77f, 0.9f);
			Rect rect = new Rect(0f, 0f, this.m_InstructionRect.width, this.m_InstructionRect.height);
			GUI.backgroundColor = backgroundColor;
			GUI.Box(rect, GUIContent.none, this.styles.solidColor);
			RectOffset padding = this.m_InstructionStyle.padding;
			Rect position = padding.Remove(rect);
			GUI.backgroundColor = backgroundColor2;
			GUI.Box(position, GUIContent.none, this.styles.solidColor);
		}
	}
}
