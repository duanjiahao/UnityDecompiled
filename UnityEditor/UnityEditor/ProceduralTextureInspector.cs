using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(ProceduralTexture))]
	internal class ProceduralTextureInspector : TextureInspector
	{
		private bool m_MightHaveModified;

		protected override void OnDisable()
		{
			base.OnDisable();
			if (!EditorApplication.isPlaying && !InternalEditorUtility.ignoreInspectorChanges && this.m_MightHaveModified)
			{
				this.m_MightHaveModified = false;
				string[] array = new string[base.targets.GetLength(0)];
				int num = 0;
				UnityEngine.Object[] targets = base.targets;
				for (int i = 0; i < targets.Length; i++)
				{
					ProceduralTexture proceduralTexture = (ProceduralTexture)targets[i];
					string assetPath = AssetDatabase.GetAssetPath(proceduralTexture);
					SubstanceImporter substanceImporter = AssetImporter.GetAtPath(assetPath) as SubstanceImporter;
					if (substanceImporter)
					{
						substanceImporter.OnTextureInformationsChanged(proceduralTexture);
					}
					assetPath = AssetDatabase.GetAssetPath(proceduralTexture.GetProceduralMaterial());
					bool flag = false;
					for (int j = 0; j < num; j++)
					{
						if (array[j] == assetPath)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						array[num++] = assetPath;
					}
				}
				for (int k = 0; k < num; k++)
				{
					SubstanceImporter substanceImporter2 = AssetImporter.GetAtPath(array[k]) as SubstanceImporter;
					if (substanceImporter2 && EditorUtility.IsDirty(substanceImporter2.GetInstanceID()))
					{
						AssetDatabase.ImportAsset(array[k], ImportAssetOptions.ForceUncompressedImport);
					}
				}
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUI.changed)
			{
				this.m_MightHaveModified = true;
			}
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				ProceduralTexture proceduralTexture = (ProceduralTexture)targets[i];
				if (proceduralTexture)
				{
					ProceduralMaterial proceduralMaterial = proceduralTexture.GetProceduralMaterial();
					if (proceduralMaterial && proceduralMaterial.isProcessing)
					{
						base.Repaint();
						SceneView.RepaintAll();
						GameView.RepaintAll();
						break;
					}
				}
			}
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			base.OnPreviewGUI(r, background);
			if (this.target)
			{
				ProceduralMaterial proceduralMaterial = (this.target as ProceduralTexture).GetProceduralMaterial();
				if (proceduralMaterial && ProceduralMaterialInspector.ShowIsGenerating(proceduralMaterial) && r.width > 50f)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 20f), "Generating...");
				}
			}
		}
	}
}
