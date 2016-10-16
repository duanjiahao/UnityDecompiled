using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CanEditMultipleObjects]
	internal class SpeedTreeMaterialInspector : MaterialEditor
	{
		private enum SpeedTreeGeometryType
		{
			Branch,
			BranchDetail,
			Frond,
			Leaf,
			Mesh
		}

		private string[] speedTreeGeometryTypeString = new string[]
		{
			"GEOM_TYPE_BRANCH",
			"GEOM_TYPE_BRANCH_DETAIL",
			"GEOM_TYPE_FROND",
			"GEOM_TYPE_LEAF",
			"GEOM_TYPE_MESH"
		};

		private bool ShouldEnableAlphaTest(SpeedTreeMaterialInspector.SpeedTreeGeometryType geomType)
		{
			return geomType == SpeedTreeMaterialInspector.SpeedTreeGeometryType.Frond || geomType == SpeedTreeMaterialInspector.SpeedTreeGeometryType.Leaf;
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			SerializedProperty serializedProperty = base.serializedObject.FindProperty("m_Shader");
			if (!base.isVisible || serializedProperty.hasMultipleDifferentValues || serializedProperty.objectReferenceValue == null)
			{
				return;
			}
			List<MaterialProperty> list = new List<MaterialProperty>(MaterialEditor.GetMaterialProperties(base.targets));
			base.SetDefaultGUIWidths();
			SpeedTreeMaterialInspector.SpeedTreeGeometryType[] array = new SpeedTreeMaterialInspector.SpeedTreeGeometryType[base.targets.Length];
			for (int i = 0; i < base.targets.Length; i++)
			{
				array[i] = SpeedTreeMaterialInspector.SpeedTreeGeometryType.Branch;
				for (int j = 0; j < this.speedTreeGeometryTypeString.Length; j++)
				{
					if (((Material)base.targets[i]).shaderKeywords.Contains(this.speedTreeGeometryTypeString[j]))
					{
						array[i] = (SpeedTreeMaterialInspector.SpeedTreeGeometryType)j;
						break;
					}
				}
			}
			EditorGUI.showMixedValue = (array.Distinct<SpeedTreeMaterialInspector.SpeedTreeGeometryType>().Count<SpeedTreeMaterialInspector.SpeedTreeGeometryType>() > 1);
			EditorGUI.BeginChangeCheck();
			SpeedTreeMaterialInspector.SpeedTreeGeometryType speedTreeGeometryType = (SpeedTreeMaterialInspector.SpeedTreeGeometryType)EditorGUILayout.EnumPopup("Geometry Type", array[0], new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				bool flag = this.ShouldEnableAlphaTest(speedTreeGeometryType);
				CullMode value = (!flag) ? CullMode.Back : CullMode.Off;
				foreach (Material current in base.targets.Cast<Material>())
				{
					for (int k = 0; k < this.speedTreeGeometryTypeString.Length; k++)
					{
						current.DisableKeyword(this.speedTreeGeometryTypeString[k]);
					}
					current.EnableKeyword(this.speedTreeGeometryTypeString[(int)speedTreeGeometryType]);
					current.renderQueue = ((!flag) ? 2000 : 2450);
					current.SetInt("_Cull", (int)value);
				}
			}
			EditorGUI.showMixedValue = false;
			MaterialProperty materialProperty = list.Find((MaterialProperty prop) => prop.name == "_MainTex");
			if (materialProperty != null)
			{
				list.Remove(materialProperty);
				base.ShaderProperty(materialProperty, materialProperty.displayName);
			}
			MaterialProperty materialProperty2 = list.Find((MaterialProperty prop) => prop.name == "_BumpMap");
			if (materialProperty2 != null)
			{
				list.Remove(materialProperty2);
				IEnumerable<bool> source = from t in base.targets
				select ((Material)t).shaderKeywords.Contains("EFFECT_BUMP");
				bool? flag2 = this.ToggleShaderProperty(materialProperty2, source.First<bool>(), source.Distinct<bool>().Count<bool>() > 1);
				if (flag2.HasValue)
				{
					foreach (Material current2 in base.targets.Cast<Material>())
					{
						if (flag2.Value)
						{
							current2.EnableKeyword("EFFECT_BUMP");
						}
						else
						{
							current2.DisableKeyword("EFFECT_BUMP");
						}
					}
				}
			}
			MaterialProperty materialProperty3 = list.Find((MaterialProperty prop) => prop.name == "_DetailTex");
			if (materialProperty3 != null)
			{
				list.Remove(materialProperty3);
				if (array.Contains(SpeedTreeMaterialInspector.SpeedTreeGeometryType.BranchDetail))
				{
					base.ShaderProperty(materialProperty3, materialProperty3.displayName);
				}
			}
			IEnumerable<bool> enumerable = from t in base.targets
			select ((Material)t).shaderKeywords.Contains("EFFECT_HUE_VARIATION");
			MaterialProperty materialProperty4 = list.Find((MaterialProperty prop) => prop.name == "_HueVariation");
			if (enumerable != null && materialProperty4 != null)
			{
				list.Remove(materialProperty4);
				bool? flag3 = this.ToggleShaderProperty(materialProperty4, enumerable.First<bool>(), enumerable.Distinct<bool>().Count<bool>() > 1);
				if (flag3.HasValue)
				{
					foreach (Material current3 in base.targets.Cast<Material>())
					{
						if (flag3.Value)
						{
							current3.EnableKeyword("EFFECT_HUE_VARIATION");
						}
						else
						{
							current3.DisableKeyword("EFFECT_HUE_VARIATION");
						}
					}
				}
			}
			MaterialProperty materialProperty5 = list.Find((MaterialProperty prop) => prop.name == "_Cutoff");
			if (materialProperty5 != null)
			{
				list.Remove(materialProperty5);
				if (array.Any((SpeedTreeMaterialInspector.SpeedTreeGeometryType t) => this.ShouldEnableAlphaTest(t)))
				{
					base.ShaderProperty(materialProperty5, materialProperty5.displayName);
				}
			}
			foreach (MaterialProperty current4 in list)
			{
				if ((current4.flags & (MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData)) == MaterialProperty.PropFlags.None)
				{
					base.ShaderProperty(current4, current4.displayName);
				}
			}
		}

		private bool? ToggleShaderProperty(MaterialProperty prop, bool enable, bool hasMixedEnable)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = hasMixedEnable;
			enable = EditorGUI.ToggleLeft(EditorGUILayout.GetControlRect(false, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}), prop.displayName, enable);
			EditorGUI.showMixedValue = false;
			bool? result = (!EditorGUI.EndChangeCheck()) ? null : new bool?(enable);
			GUILayout.Space(-EditorGUIUtility.singleLineHeight);
			using (new EditorGUI.DisabledScope(!enable && !hasMixedEnable))
			{
				EditorGUI.showMixedValue = prop.hasMixedValue;
				base.ShaderProperty(prop, " ");
				EditorGUI.showMixedValue = false;
			}
			return result;
		}
	}
}
