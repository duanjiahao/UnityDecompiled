using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal static class MaterialAnimationUtility
	{
		private const string kMaterialPrefix = "material.";

		private static PropertyModification[] CreatePropertyModifications(int count, UnityEngine.Object target)
		{
			PropertyModification[] array = new PropertyModification[count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new PropertyModification();
				array[i].target = target;
			}
			return array;
		}

		private static void SetupPropertyModification(string name, float value, PropertyModification prop)
		{
			prop.propertyPath = "material." + name;
			prop.value = value.ToString();
		}

		private static PropertyModification[] MaterialPropertyToPropertyModifications(MaterialProperty materialProp, UnityEngine.Object target, float value)
		{
			PropertyModification[] array = MaterialAnimationUtility.CreatePropertyModifications(1, target);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name, value, array[0]);
			return array;
		}

		private static PropertyModification[] MaterialPropertyToPropertyModifications(MaterialProperty materialProp, UnityEngine.Object target, Color color)
		{
			PropertyModification[] array = MaterialAnimationUtility.CreatePropertyModifications(4, target);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name + ".r", color.r, array[0]);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name + ".g", color.g, array[1]);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name + ".b", color.b, array[2]);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name + ".a", color.a, array[3]);
			return array;
		}

		private static PropertyModification[] MaterialPropertyToPropertyModifications(string name, UnityEngine.Object target, Vector4 vec)
		{
			PropertyModification[] array = MaterialAnimationUtility.CreatePropertyModifications(4, target);
			MaterialAnimationUtility.SetupPropertyModification(name + ".x", vec.x, array[0]);
			MaterialAnimationUtility.SetupPropertyModification(name + ".y", vec.y, array[1]);
			MaterialAnimationUtility.SetupPropertyModification(name + ".z", vec.z, array[2]);
			MaterialAnimationUtility.SetupPropertyModification(name + ".w", vec.w, array[3]);
			return array;
		}

		private static bool ApplyMaterialModificationToAnimationRecording(PropertyModification[] modifications)
		{
			UndoPropertyModification[] array = new UndoPropertyModification[modifications.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i].previousValue = modifications[i];
			}
			UndoPropertyModification[] array2 = Undo.postprocessModifications(array);
			return array2.Length != modifications.Length;
		}

		public static bool OverridePropertyColor(MaterialProperty materialProp, Renderer target, out Color color)
		{
			List<string> list = new List<string>();
			string text = "material." + materialProp.name;
			if (materialProp.type == MaterialProperty.PropType.Texture)
			{
				list.Add(text + "_ST.x");
				list.Add(text + "_ST.y");
				list.Add(text + "_ST.z");
				list.Add(text + "_ST.w");
			}
			else if (materialProp.type == MaterialProperty.PropType.Color)
			{
				list.Add(text + ".r");
				list.Add(text + ".g");
				list.Add(text + ".b");
				list.Add(text + ".a");
			}
			else
			{
				list.Add(text);
			}
			bool result;
			if (list.Exists((string path) => AnimationMode.IsPropertyAnimated(target, path)))
			{
				color = AnimationMode.animatedPropertyColor;
				if (AnimationMode.InAnimationRecording())
				{
					color = AnimationMode.recordedPropertyColor;
				}
				else if (list.Exists((string path) => AnimationMode.IsPropertyCandidate(target, path)))
				{
					color = AnimationMode.candidatePropertyColor;
				}
				result = true;
			}
			else
			{
				color = Color.white;
				result = false;
			}
			return result;
		}

		public static void SetupMaterialPropertyBlock(MaterialProperty materialProp, int changedMask, Renderer target)
		{
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			target.GetPropertyBlock(materialPropertyBlock);
			materialProp.WriteToMaterialPropertyBlock(materialPropertyBlock, changedMask);
			target.SetPropertyBlock(materialPropertyBlock);
		}

		public static void TearDownMaterialPropertyBlock(Renderer target)
		{
			target.SetPropertyBlock(null);
		}

		public static bool ApplyMaterialModificationToAnimationRecording(MaterialProperty materialProp, int changedMask, Renderer target, object oldValue)
		{
			bool result;
			switch (materialProp.type)
			{
			case MaterialProperty.PropType.Color:
			{
				MaterialAnimationUtility.SetupMaterialPropertyBlock(materialProp, changedMask, target);
				bool flag = MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(MaterialAnimationUtility.MaterialPropertyToPropertyModifications(materialProp, target, (Color)oldValue));
				if (!flag)
				{
					MaterialAnimationUtility.TearDownMaterialPropertyBlock(target);
				}
				result = flag;
				break;
			}
			case MaterialProperty.PropType.Vector:
			{
				MaterialAnimationUtility.SetupMaterialPropertyBlock(materialProp, changedMask, target);
				bool flag = MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(MaterialAnimationUtility.MaterialPropertyToPropertyModifications(materialProp, target, (Vector4)oldValue));
				if (!flag)
				{
					MaterialAnimationUtility.TearDownMaterialPropertyBlock(target);
				}
				result = flag;
				break;
			}
			case MaterialProperty.PropType.Float:
			case MaterialProperty.PropType.Range:
			{
				MaterialAnimationUtility.SetupMaterialPropertyBlock(materialProp, changedMask, target);
				bool flag = MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(MaterialAnimationUtility.MaterialPropertyToPropertyModifications(materialProp, target, (float)oldValue));
				if (!flag)
				{
					MaterialAnimationUtility.TearDownMaterialPropertyBlock(target);
				}
				result = flag;
				break;
			}
			case MaterialProperty.PropType.Texture:
				if (MaterialProperty.IsTextureOffsetAndScaleChangedMask(changedMask))
				{
					string name = materialProp.name + "_ST";
					MaterialAnimationUtility.SetupMaterialPropertyBlock(materialProp, changedMask, target);
					bool flag = MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(MaterialAnimationUtility.MaterialPropertyToPropertyModifications(name, target, (Vector4)oldValue));
					if (!flag)
					{
						MaterialAnimationUtility.TearDownMaterialPropertyBlock(target);
					}
					result = flag;
				}
				else
				{
					result = false;
				}
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public static PropertyModification[] MaterialPropertyToPropertyModifications(MaterialProperty materialProp, Renderer target)
		{
			PropertyModification[] result;
			switch (materialProp.type)
			{
			case MaterialProperty.PropType.Color:
				result = MaterialAnimationUtility.MaterialPropertyToPropertyModifications(materialProp, target, materialProp.colorValue);
				break;
			case MaterialProperty.PropType.Vector:
				result = MaterialAnimationUtility.MaterialPropertyToPropertyModifications(materialProp, target, materialProp.vectorValue);
				break;
			case MaterialProperty.PropType.Float:
			case MaterialProperty.PropType.Range:
				result = MaterialAnimationUtility.MaterialPropertyToPropertyModifications(materialProp, target, materialProp.floatValue);
				break;
			case MaterialProperty.PropType.Texture:
			{
				string name = materialProp.name + "_ST";
				result = MaterialAnimationUtility.MaterialPropertyToPropertyModifications(name, target, materialProp.vectorValue);
				break;
			}
			default:
				result = null;
				break;
			}
			return result;
		}
	}
}
