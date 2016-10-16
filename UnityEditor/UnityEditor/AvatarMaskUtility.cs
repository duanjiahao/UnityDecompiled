using System;
using UnityEditor.Animations;

namespace UnityEditor
{
	internal class AvatarMaskUtility
	{
		private static string sHuman = "m_HumanDescription.m_Human";

		private static string sBoneName = "m_BoneName";

		public static string[] GetAvatarHumanTransform(SerializedObject so, string[] refTransformsPath)
		{
			SerializedProperty serializedProperty = so.FindProperty(AvatarMaskUtility.sHuman);
			if (serializedProperty == null || !serializedProperty.isArray)
			{
				return null;
			}
			string[] humanTransforms = new string[0];
			for (int i = 0; i < serializedProperty.arraySize; i++)
			{
				SerializedProperty serializedProperty2 = serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative(AvatarMaskUtility.sBoneName);
				ArrayUtility.Add<string>(ref humanTransforms, serializedProperty2.stringValue);
			}
			return AvatarMaskUtility.TokeniseHumanTransformsPath(refTransformsPath, humanTransforms);
		}

		public static void UpdateTransformMask(AvatarMask mask, string[] refTransformsPath, string[] humanTransforms)
		{
			AvatarMaskUtility.<UpdateTransformMask>c__AnonStorey94 <UpdateTransformMask>c__AnonStorey = new AvatarMaskUtility.<UpdateTransformMask>c__AnonStorey94();
			<UpdateTransformMask>c__AnonStorey.refTransformsPath = refTransformsPath;
			mask.transformCount = <UpdateTransformMask>c__AnonStorey.refTransformsPath.Length;
			int i;
			for (i = 0; i < <UpdateTransformMask>c__AnonStorey.refTransformsPath.Length; i++)
			{
				mask.SetTransformPath(i, <UpdateTransformMask>c__AnonStorey.refTransformsPath[i]);
				bool value = humanTransforms == null || ArrayUtility.FindIndex<string>(humanTransforms, (string s) => <UpdateTransformMask>c__AnonStorey.refTransformsPath[i] == s) != -1;
				mask.SetTransformActive(i, value);
			}
		}

		public static void UpdateTransformMask(SerializedProperty transformMask, string[] refTransformsPath, string[] humanTransforms)
		{
			AvatarMaskUtility.<UpdateTransformMask>c__AnonStorey96 <UpdateTransformMask>c__AnonStorey = new AvatarMaskUtility.<UpdateTransformMask>c__AnonStorey96();
			<UpdateTransformMask>c__AnonStorey.refTransformsPath = refTransformsPath;
			AvatarMask avatarMask = new AvatarMask();
			avatarMask.transformCount = <UpdateTransformMask>c__AnonStorey.refTransformsPath.Length;
			int i;
			for (i = 0; i < <UpdateTransformMask>c__AnonStorey.refTransformsPath.Length; i++)
			{
				bool value = humanTransforms == null || ArrayUtility.FindIndex<string>(humanTransforms, (string s) => <UpdateTransformMask>c__AnonStorey.refTransformsPath[i] == s) != -1;
				avatarMask.SetTransformPath(i, <UpdateTransformMask>c__AnonStorey.refTransformsPath[i]);
				avatarMask.SetTransformActive(i, value);
			}
			ModelImporter.UpdateTransformMask(avatarMask, transformMask);
		}

		public static void SetActiveHumanTransforms(AvatarMask mask, string[] humanTransforms)
		{
			for (int i = 0; i < mask.transformCount; i++)
			{
				string path = mask.GetTransformPath(i);
				if (ArrayUtility.FindIndex<string>(humanTransforms, (string s) => path == s) != -1)
				{
					mask.SetTransformActive(i, true);
				}
			}
		}

		private static string[] TokeniseHumanTransformsPath(string[] refTransformsPath, string[] humanTransforms)
		{
			AvatarMaskUtility.<TokeniseHumanTransformsPath>c__AnonStorey99 <TokeniseHumanTransformsPath>c__AnonStorey = new AvatarMaskUtility.<TokeniseHumanTransformsPath>c__AnonStorey99();
			<TokeniseHumanTransformsPath>c__AnonStorey.humanTransforms = humanTransforms;
			if (<TokeniseHumanTransformsPath>c__AnonStorey.humanTransforms == null)
			{
				return null;
			}
			string[] array = new string[]
			{
				string.Empty
			};
			int i;
			for (i = 0; i < <TokeniseHumanTransformsPath>c__AnonStorey.humanTransforms.Length; i++)
			{
				int num = ArrayUtility.FindIndex<string>(refTransformsPath, (string s) => <TokeniseHumanTransformsPath>c__AnonStorey.humanTransforms[i] == FileUtil.GetLastPathNameComponent(s));
				if (num != -1)
				{
					int index = array.Length;
					string path = refTransformsPath[num];
					while (path.Length > 0)
					{
						int num2 = ArrayUtility.FindIndex<string>(array, (string s) => path == s);
						if (num2 == -1)
						{
							ArrayUtility.Insert<string>(ref array, index, path);
						}
						int num3 = path.LastIndexOf('/');
						path = path.Substring(0, (num3 == -1) ? 0 : num3);
					}
				}
			}
			return array;
		}
	}
}
