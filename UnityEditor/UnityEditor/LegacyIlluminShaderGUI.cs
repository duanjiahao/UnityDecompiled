using System;
using UnityEngine;

namespace UnityEditor
{
	internal class LegacyIlluminShaderGUI : ShaderGUI
	{
		public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
		{
			base.OnGUI(materialEditor, props);
			materialEditor.LightmapEmissionProperty(0);
			UnityEngine.Object[] targets = materialEditor.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				Material material = (Material)targets[i];
				material.globalIlluminationFlags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
			}
		}
	}
}
