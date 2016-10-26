using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(LineRenderer))]
	internal class LineRendererInspector : RendererEditorBase
	{
		private string[] m_ExcludedProperties;

		public override void OnEnable()
		{
			base.OnEnable();
			List<string> list = new List<string>();
			list.AddRange(RendererEditorBase.Probes.GetFieldsStringArray());
			this.m_ExcludedProperties = list.ToArray();
			base.InitializeProbeFields();
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			Editor.DrawPropertiesExcluding(this.m_SerializedObject, this.m_ExcludedProperties);
			base.RenderProbeFields();
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
