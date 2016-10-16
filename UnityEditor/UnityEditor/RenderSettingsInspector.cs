using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(RenderSettings))]
	internal class RenderSettingsInspector : Editor
	{
		private Editor m_LightingEditor;

		private Editor m_FogEditor;

		private Editor m_OtherRenderingEditor;

		private Editor lightingEditor
		{
			get
			{
				Editor arg_2B_0;
				if ((arg_2B_0 = this.m_LightingEditor) == null)
				{
					arg_2B_0 = (this.m_LightingEditor = Editor.CreateEditor(this.target, typeof(LightingEditor)));
				}
				return arg_2B_0;
			}
		}

		private Editor fogEditor
		{
			get
			{
				Editor arg_2B_0;
				if ((arg_2B_0 = this.m_FogEditor) == null)
				{
					arg_2B_0 = (this.m_FogEditor = Editor.CreateEditor(this.target, typeof(FogEditor)));
				}
				return arg_2B_0;
			}
		}

		private Editor otherRenderingEditor
		{
			get
			{
				Editor arg_2B_0;
				if ((arg_2B_0 = this.m_OtherRenderingEditor) == null)
				{
					arg_2B_0 = (this.m_OtherRenderingEditor = Editor.CreateEditor(this.target, typeof(OtherRenderingEditor)));
				}
				return arg_2B_0;
			}
		}

		public virtual void OnEnable()
		{
			this.m_LightingEditor = null;
			this.m_FogEditor = null;
			this.m_OtherRenderingEditor = null;
		}

		public override void OnInspectorGUI()
		{
			this.lightingEditor.OnInspectorGUI();
			this.fogEditor.OnInspectorGUI();
			this.otherRenderingEditor.OnInspectorGUI();
		}
	}
}
