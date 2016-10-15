using System;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(ModelImporter))]
	internal class ModelImporterEditor : AssetImporterTabbedEditor
	{
		internal override bool showImportedObject
		{
			get
			{
				return base.activeEditor is ModelImporterModelEditor;
			}
		}

		protected override bool useAssetDrawPreview
		{
			get
			{
				return false;
			}
		}

		internal override void OnEnable()
		{
			if (this.m_SubEditorTypes == null)
			{
				this.m_SubEditorTypes = new Type[]
				{
					typeof(ModelImporterModelEditor),
					typeof(ModelImporterRigEditor),
					typeof(ModelImporterClipEditor)
				};
				this.m_SubEditorNames = new string[]
				{
					"Model",
					"Rig",
					"Animations"
				};
			}
			base.OnEnable();
		}

		public override bool HasPreviewGUI()
		{
			return base.HasPreviewGUI() && base.targets.Length < 2;
		}
	}
}
