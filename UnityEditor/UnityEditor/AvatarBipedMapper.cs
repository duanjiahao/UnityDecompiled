using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class AvatarBipedMapper
	{
		private static string[] kBipedHumanBoneNames = new string[]
		{
			"Pelvis",
			"L Thigh",
			"R Thigh",
			"L Calf",
			"R Calf",
			"L Foot",
			"R Foot",
			"Spine",
			"Spine1",
			"Neck",
			"Head",
			"L Clavicle",
			"R Clavicle",
			"L UpperArm",
			"R UpperArm",
			"L Forearm",
			"R Forearm",
			"L Hand",
			"R Hand",
			"L Toe0",
			"R Toe0",
			"",
			"",
			"",
			"L Finger0",
			"L Finger01",
			"L Finger02",
			"L Finger1",
			"L Finger11",
			"L Finger12",
			"L Finger2",
			"L Finger21",
			"L Finger22",
			"L Finger3",
			"L Finger31",
			"L Finger32",
			"L Finger4",
			"L Finger41",
			"L Finger42",
			"R Finger0",
			"R Finger01",
			"R Finger02",
			"R Finger1",
			"R Finger11",
			"R Finger12",
			"R Finger2",
			"R Finger21",
			"R Finger22",
			"R Finger3",
			"R Finger31",
			"R Finger32",
			"R Finger4",
			"R Finger41",
			"R Finger42"
		};

		public static bool IsBiped(Transform root, List<string> report)
		{
			if (report != null)
			{
				report.Clear();
			}
			Transform[] array = new Transform[HumanTrait.BoneCount];
			return AvatarBipedMapper.MapBipedBones(root, ref array, report);
		}

		public static Dictionary<int, Transform> MapBones(Transform root)
		{
			Dictionary<int, Transform> dictionary = new Dictionary<int, Transform>();
			Transform[] array = new Transform[HumanTrait.BoneCount];
			if (AvatarBipedMapper.MapBipedBones(root, ref array, null))
			{
				for (int i = 0; i < HumanTrait.BoneCount; i++)
				{
					if (array[i] != null)
					{
						dictionary.Add(i, array[i]);
					}
				}
			}
			return dictionary;
		}

		private static bool MapBipedBones(Transform root, ref Transform[] humanToTransform, List<string> report)
		{
			bool result;
			for (int i = 0; i < HumanTrait.BoneCount; i++)
			{
				string a = AvatarBipedMapper.kBipedHumanBoneNames[i];
				int parentBone = HumanTrait.GetParentBone(i);
				bool flag = HumanTrait.RequiredBone(i);
				bool flag2 = parentBone == -1 || HumanTrait.RequiredBone(parentBone);
				Transform transform = (parentBone == -1) ? root : humanToTransform[parentBone];
				if (transform == null && !flag2)
				{
					parentBone = HumanTrait.GetParentBone(parentBone);
					transform = ((parentBone == -1) ? null : humanToTransform[parentBone]);
				}
				if (a != "")
				{
					humanToTransform[i] = AvatarBipedMapper.MapBipedBone(i, transform, transform, report);
					if (humanToTransform[i] == null && flag)
					{
						result = false;
						return result;
					}
				}
			}
			result = true;
			return result;
		}

		private static Transform MapBipedBone(int boneIndex, Transform transform, Transform parentTransform, List<string> report)
		{
			Transform transform2 = null;
			if (transform != null)
			{
				int childCount = transform.childCount;
				int num = 0;
				while (transform2 == null && num < childCount)
				{
					if (transform.GetChild(num).name.EndsWith(AvatarBipedMapper.kBipedHumanBoneNames[boneIndex]))
					{
						transform2 = transform.GetChild(num);
						if (transform2 != null && report != null && boneIndex != 0 && transform != parentTransform)
						{
							string text = string.Concat(new string[]
							{
								"- Invalid parent for ",
								transform2.name,
								".Expected ",
								parentTransform.name,
								", but found ",
								transform.name,
								"."
							});
							if (boneIndex == 1 || boneIndex == 2)
							{
								text += " Disable Triangle Pelvis";
							}
							else if (boneIndex == 11 || boneIndex == 12)
							{
								text += " Enable Triangle Neck";
							}
							else if (boneIndex == 9)
							{
								text += " Preferred is two Spine Links";
							}
							else if (boneIndex == 10)
							{
								text += " Preferred is one Neck Links";
							}
							text += "\n";
							report.Add(text);
						}
					}
					num++;
				}
				int num2 = 0;
				while (transform2 == null && num2 < childCount)
				{
					transform2 = AvatarBipedMapper.MapBipedBone(boneIndex, transform.GetChild(num2), parentTransform, report);
					num2++;
				}
			}
			return transform2;
		}

		internal static void BipedPose(GameObject go, AvatarSetupTool.BoneWrapper[] bones)
		{
			AvatarBipedMapper.BipedPose(go.transform, true);
			Quaternion rotation = AvatarSetupTool.AvatarComputeOrientation(bones);
			go.transform.rotation = Quaternion.Inverse(rotation) * go.transform.rotation;
			AvatarSetupTool.MakeCharacterPositionValid(bones);
		}

		private static void BipedPose(Transform t, bool ignore)
		{
			if (t.name.EndsWith("Pelvis"))
			{
				t.localRotation = Quaternion.Euler(270f, 90f, 0f);
				ignore = false;
			}
			else if (t.name.EndsWith("Thigh"))
			{
				t.localRotation = Quaternion.Euler(0f, 180f, 0f);
			}
			else if (t.name.EndsWith("Toe0"))
			{
				t.localRotation = Quaternion.Euler(0f, 0f, 270f);
			}
			else if (t.name.EndsWith("L Clavicle"))
			{
				t.localRotation = Quaternion.Euler(0f, 270f, 180f);
			}
			else if (t.name.EndsWith("R Clavicle"))
			{
				t.localRotation = Quaternion.Euler(0f, 90f, 180f);
			}
			else if (t.name.EndsWith("L Hand"))
			{
				t.localRotation = Quaternion.Euler(270f, 0f, 0f);
			}
			else if (t.name.EndsWith("R Hand"))
			{
				t.localRotation = Quaternion.Euler(90f, 0f, 0f);
			}
			else if (t.name.EndsWith("L Finger0"))
			{
				t.localRotation = Quaternion.Euler(0f, 315f, 0f);
			}
			else if (t.name.EndsWith("R Finger0"))
			{
				t.localRotation = Quaternion.Euler(0f, 45f, 0f);
			}
			else if (!ignore)
			{
				t.localRotation = Quaternion.identity;
			}
			IEnumerator enumerator = t.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform t2 = (Transform)enumerator.Current;
					AvatarBipedMapper.BipedPose(t2, ignore);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}
