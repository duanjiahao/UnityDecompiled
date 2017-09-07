using System;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(ModelImporter))]
	internal class ModelImporterEditor : AssetImporterTabbedEditor
	{
		public override bool showImportedObject
		{
			get
			{
				return base.activeTab is ModelImporterModelEditor;
			}
		}

		public override void OnEnable()
		{
			if (base.tabs == null)
			{
				base.tabs = new BaseAssetImporterTabUI[]
				{
					new ModelImporterModelEditor(this),
					new ModelImporterRigEditor(this),
					new ModelImporterClipEditor(this)
				};
				this.m_TabNames = new string[]
				{
					"Model",
					"Rig",
					"Animations"
				};
			}
			base.OnEnable();
		}

		public override void OnDisable()
		{
			BaseAssetImporterTabUI[] tabs = base.tabs;
			for (int i = 0; i < tabs.Length; i++)
			{
				BaseAssetImporterTabUI baseAssetImporterTabUI = tabs[i];
				baseAssetImporterTabUI.OnDisable();
			}
			base.OnDisable();
		}

		public override bool HasPreviewGUI()
		{
			return base.HasPreviewGUI() && base.targets.Length < 2;
		}
	}
}
