using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(ParticleRenderer))]
	internal class ParticleRendererEditor : RendererEditorBase
	{
		public override void OnEnable()
		{
			base.OnEnable();
			base.InitializeProbeFields();
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			Editor.DrawPropertiesExcluding(base.serializedObject, RendererEditorBase.Probes.GetFieldsStringArray());
			base.RenderProbeFields();
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
