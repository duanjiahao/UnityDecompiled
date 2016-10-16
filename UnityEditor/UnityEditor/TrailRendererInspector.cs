using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(TrailRenderer))]
	internal class TrailRendererInspector : RendererEditorBase
	{
		private string[] m_ExcludedProperties;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_ExcludedProperties = new List<string>
			{
				"m_LightProbeUsage",
				"m_LightProbeVolumeOverride",
				"m_ReflectionProbeUsage",
				"m_ProbeAnchor"
			}.ToArray();
			base.InitializeProbeFields();
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			Editor.DrawPropertiesExcluding(this.m_SerializedObject, this.m_ExcludedProperties);
			base.RenderCommonProbeFields(false);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
