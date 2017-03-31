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

		private static bool ApplyMaterialModificationToAnimationRecording(MaterialProperty materialProp, UnityEngine.Object target, float value)
		{
			UndoPropertyModification[] array = MaterialAnimationUtility.CreateUndoPropertyModifications(1, target);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name, value, array[0]);
			UndoPropertyModification[] array2 = Undo.postprocessModifications(array);
			return array2.Length != array.Length;
		}

		private static bool ApplyMaterialModificationToAnimationRecording(MaterialProperty materialProp, UnityEngine.Object target, Color color)
		{
			UndoPropertyModification[] array = MaterialAnimationUtility.CreateUndoPropertyModifications(4, target);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name + ".r", color.r, array[0]);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name + ".g", color.g, array[1]);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name + ".b", color.b, array[2]);
			MaterialAnimationUtility.SetupPropertyModification(materialProp.name + ".a", color.a, array[3]);
			UndoPropertyModification[] array2 = Undo.postprocessModifications(array);
			return array2.Length != array.Length;
		}

		private static bool ApplyMaterialModificationToAnimationRecording(string name, UnityEngine.Object target, Vector4 vec)
		{
			UndoPropertyModification[] array = MaterialAnimationUtility.CreateUndoPropertyModifications(4, target);
			MaterialAnimationUtility.SetupPropertyModification(name + ".x", vec.x, array[0]);
			MaterialAnimationUtility.SetupPropertyModification(name + ".y", vec.y, array[1]);
			MaterialAnimationUtility.SetupPropertyModification(name + ".z", vec.z, array[2]);
			MaterialAnimationUtility.SetupPropertyModification(name + ".w", vec.w, array[3]);
			UndoPropertyModification[] array2 = Undo.postprocessModifications(array);
			return array2.Length != array.Length;
		}

		public static bool IsAnimated(MaterialProperty materialProp, Renderer target)
		{
			bool result;
			if (materialProp.type == MaterialProperty.PropType.Texture)
			{
				result = AnimationMode.IsPropertyAnimated(target, "material." + materialProp.name + "_ST");
			}
			else
			{
				result = AnimationMode.IsPropertyAnimated(target, "material." + materialProp.name);
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
				bool flag = MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(materialProp, target, (Color)oldValue);
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
				bool flag = MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(materialProp, target, (Vector4)oldValue);
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
				bool flag = MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(materialProp, target, (float)oldValue);
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
					bool flag = MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(name, target, (Vector4)oldValue);
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
	}
}
