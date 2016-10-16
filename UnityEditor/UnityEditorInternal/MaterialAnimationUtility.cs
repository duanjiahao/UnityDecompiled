using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal static class MaterialAnimationUtility
	{
		private const string kMaterialPrefix = "material.";

		private static UndoPropertyModification[] CreateUndoPropertyModifications(int count, UnityEngine.Object target)
		{
			UndoPropertyModification[] array = new UndoPropertyModification[count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i].previousValue = new PropertyModification();
				array[i].previousValue.target = target;
			}
			return array;
		}

		private static void SetupPropertyModification(string name, float value, UndoPropertyModification prop)
		{
			prop.previousValue.propertyPath = "material." + name;
			prop.previousValue.value = value.ToString();
		}

		private static void ApplyMaterialModificationToAnimationRecording(MaterialProperty materialProp, UnityEngine.Object target, float value)
		{
			UndoPropertyModification[] array = MaterialAnimationUtility.CreateUndoPropertyModifications(1, target);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name, value, array[0]);
			Undo.postprocessModifications(array);
		}

		private static void ApplyMaterialModificationToAnimationRecording(MaterialProperty materialProp, UnityEngine.Object target, Color color)
		{
			UndoPropertyModification[] array = MaterialAnimationUtility.CreateUndoPropertyModifications(4, target);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name + ".r", color.r, array[0]);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name + ".g", color.g, array[1]);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name + ".b", color.b, array[2]);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name + ".a", color.a, array[3]);
			Undo.postprocessModifications(array);
		}

		private static void ApplyMaterialModificationToAnimationRecording(string name, UnityEngine.Object target, Vector4 vec)
		{
			UndoPropertyModification[] array = MaterialAnimationUtility.CreateUndoPropertyModifications(4, target);
			MaterialAnimationUtility.SetupPropertyModification(name + ".x", vec.x, array[0]);
			MaterialAnimationUtility.SetupPropertyModification(name + ".y", vec.y, array[1]);
			MaterialAnimationUtility.SetupPropertyModification(name + ".z", vec.z, array[2]);
			MaterialAnimationUtility.SetupPropertyModification(name + ".w", vec.w, array[3]);
			Undo.postprocessModifications(array);
		}

		public static bool IsAnimated(MaterialProperty materialProp, Renderer target)
		{
			if (materialProp.type == MaterialProperty.PropType.Texture)
			{
				return AnimationMode.IsPropertyAnimated(target, "material." + materialProp.name + "_ST");
			}
			return AnimationMode.IsPropertyAnimated(target, "material." + materialProp.name);
		}

		public static void SetupMaterialPropertyBlock(MaterialProperty materialProp, int changedMask, Renderer target)
		{
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			target.GetPropertyBlock(materialPropertyBlock);
			materialProp.WriteToMaterialPropertyBlock(materialPropertyBlock, changedMask);
			target.SetPropertyBlock(materialPropertyBlock);
		}

		public static bool ApplyMaterialModificationToAnimationRecording(MaterialProperty materialProp, int changedMask, Renderer target, object oldValue)
		{
			switch (materialProp.type)
			{
			case MaterialProperty.PropType.Color:
				MaterialAnimationUtility.SetupMaterialPropertyBlock(materialProp, changedMask, target);
				MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(materialProp, target, (Color)oldValue);
				return true;
			case MaterialProperty.PropType.Vector:
				MaterialAnimationUtility.SetupMaterialPropertyBlock(materialProp, changedMask, target);
				MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(materialProp, target, (Vector4)oldValue);
				return true;
			case MaterialProperty.PropType.Float:
			case MaterialProperty.PropType.Range:
				MaterialAnimationUtility.SetupMaterialPropertyBlock(materialProp, changedMask, target);
				MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(materialProp, target, (float)oldValue);
				return true;
			case MaterialProperty.PropType.Texture:
				if (MaterialProperty.IsTextureOffsetAndScaleChangedMask(changedMask))
				{
					string name = materialProp.name + "_ST";
					MaterialAnimationUtility.SetupMaterialPropertyBlock(materialProp, changedMask, target);
					MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(name, target, (Vector4)oldValue);
					return true;
				}
				return false;
			default:
				return false;
			}
		}
	}
}
