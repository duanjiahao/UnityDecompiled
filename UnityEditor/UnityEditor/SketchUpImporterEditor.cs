using System;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SketchUpImporter))]
	internal class SketchUpImporterEditor : ModelImporterEditor
	{
		public override bool showImportedObject
		{
			get
			{
				return base.activeTab is SketchUpImporterModelEditor;
			}
		}

		public override void OnEnable()
		{
			if (base.tabs == null)
			{
				base.tabs = new BaseAssetImporterTabUI[]
				{
					new SketchUpImporterModelEditor(this),
					new ModelImporterRigEditor(this),
					new ModelImporterClipEditor(this)
				};
				this.m_TabNames = new string[]
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
