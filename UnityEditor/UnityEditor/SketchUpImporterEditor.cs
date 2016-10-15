using System;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SketchUpImporter))]
	internal class SketchUpImporterEditor : ModelImporterEditor
	{
		internal override bool showImportedObject
		{
			get
			{
				return base.activeEditor is SketchUpImporterModelEditor;
			}
		}

		internal override void OnEnable()
		{
			if (this.m_SubEditorTypes == null)
			{
				this.m_SubEditorTypes = new Type[]
				{
					typeof(SketchUpImporterModelEditor),
					typeof(ModelImporterRigEditor),
					typeof(ModelImporterClipEditor)
				};
				this.m_SubEditorNames = new string[]
				{
					"Sketch Up",
					"Rig",
					"Animations"
				};
			}
			base.OnEnable();
		}
	}
}
