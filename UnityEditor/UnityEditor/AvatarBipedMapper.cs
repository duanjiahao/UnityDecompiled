using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class AvatarBipedMapper
	{
		private struct BipedBone
		{
			public string name;

			public int index;

			public BipedBone(string name, int index)
			{
				this.name = name;
				this.index = index;
			}
		}

		private static AvatarBipedMapper.BipedBone[] s_BipedBones = new AvatarBipedMapper.BipedBone[]
		{
			new AvatarBipedMapper.BipedBone("Pelvis", 0),
			new AvatarBipedMapper.BipedBone("L Thigh", 1),
			new AvatarBipedMapper.BipedBone("R Thigh", 2),
			new AvatarBipedMapper.BipedBone("L Calf", 3),
			new AvatarBipedMapper.BipedBone("R Calf", 4),
			new AvatarBipedMapper.BipedBone("L Foot", 5),
			new AvatarBipedMapper.BipedBone("R Foot", 6),
			new AvatarBipedMapper.BipedBone("Spine", 7),
			new AvatarBipedMapper.BipedBone("Spine1", 8),
			new AvatarBipedMapper.BipedBone("Spine2", 54),
			new AvatarBipedMapper.BipedBone("Neck", 9),
			new AvatarBipedMapper.BipedBone("Head", 10),
			new AvatarBipedMapper.BipedBone("L Clavicle", 11),
			new AvatarBipedMapper.BipedBone("R Clavicle", 12),
			new AvatarBipedMapper.BipedBone("L UpperArm", 13),
			new AvatarBipedMapper.BipedBone("R UpperArm", 14),
			new AvatarBipedMapper.BipedBone("L Forearm", 15),
			new AvatarBipedMapper.BipedBone("R Forearm", 16),
			new AvatarBipedMapper.BipedBone("L Hand", 17),
			new AvatarBipedMapper.BipedBone("R Hand", 18),
			new AvatarBipedMapper.BipedBone("L Toe0", 19),
			new AvatarBipedMapper.BipedBone("R Toe0", 20),
			new AvatarBipedMapper.BipedBone("L Finger0", 24),
			new AvatarBipedMapper.BipedBone("L Finger01", 25),
			new AvatarBipedMapper.BipedBone("L Finger02", 26),
			new AvatarBipedMapper.BipedBone("L Finger1", 27),
			new AvatarBipedMapper.BipedBone("L Finger11", 28),
			new AvatarBipedMapper.BipedBone("L Finger12", 29),
			new AvatarBipedMapper.BipedBone("L Finger2", 30),
			new AvatarBipedMapper.BipedBone("L Finger21", 31),
			new AvatarBipedMapper.BipedBone("L Finger22", 32),
			new AvatarBipedMapper.BipedBone("L Finger3", 33),
			new AvatarBipedMapper.BipedBone("L Finger31", 34),
			new AvatarBipedMapper.BipedBone("L Finger32", 35),
			new AvatarBipedMapper.BipedBone("L Finger4", 36),
			new AvatarBipedMapper.BipedBone("L Finger41", 37),
			new AvatarBipedMapper.BipedBone("L Finger42", 38),
			new AvatarBipedMapper.BipedBone("R Finger0", 39),
			new AvatarBipedMapper.BipedBone("R Finger01", 40),
			new AvatarBipedMapper.BipedBone("R Finger02", 41),
			new AvatarBipedMapper.BipedBone("R Finger1", 42),
			new AvatarBipedMapper.BipedBone("R Finger11", 43),
			new AvatarBipedMapper.BipedBone("R Finger12", 44),
			new AvatarBipedMapper.BipedBone("R Finger2", 45),
			new AvatarBipedMapper.BipedBone("R Finger21", 46),
			new AvatarBipedMapper.BipedBone("R Finger22", 47),
			new AvatarBipedMapper.BipedBone("R Finger3", 48),
			new AvatarBipedMapper.BipedBone("R Finger31", 49),
			new AvatarBipedMapper.BipedBone("R Finger32", 50),
			new AvatarBipedMapper.BipedBone("R Finger4", 51),
			new AvatarBipedMapper.BipedBone("R Finger41", 52),
			new AvatarBipedMapper.BipedBone("R Finger42", 53)
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
			if (!dictionary.ContainsKey(8) && dictionary.ContainsKey(54))
			{
				dictionary.Add(8, dictionary[54]);
				dictionary.Remove(54);
			}
			return dictionary;
		}

		private static bool MapBipedBones(Transform root, ref Transform[] humanToTransform, List<string> report)
		{
			bool result;
			for (int i = 0; i < AvatarBipedMapper.s_BipedBones.Length; i++)
			{
				int index = AvatarBipedMapper.s_BipedBones[i].index;
				int parentBone = HumanTrait.GetParentBone(index);
				bool flag = HumanTrait.RequiredBone(index);
				bool flag2 = parentBone == -1 || HumanTrait.RequiredBone(parentBone);
				Transform transform = (parentBone == -1) ? root : humanToTransform[parentBone];
				if (transform == null && !flag2)
				{
					parentBone = HumanTrait.GetParentBone(parentBone);
					flag2 = (parentBone == -1 || HumanTrait.RequiredBone(parentBone));
					transform = ((parentBone == -1) ? null : humanToTransform[parentBone]);
					if (transform == null && !flag2)
					{
						parentBone = HumanTrait.GetParentBone(parentBone);
						transform = ((parentBone == -1) ? null : humanToTransform[parentBone]);
					}
				}
				humanToTransform[index] = AvatarBipedMapper.MapBipedBone(i, transform, transform, report);
				if (humanToTransform[index] == null && flag)
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}

		private static Transform MapBipedBone(int bipedBoneIndex, Transform transform, Transform parentTransform, List<string> report)
		{
			Transform transform2 = null;
			if (transform != null)
			{
				int childCount = transform.childCount;
				int num = 0;
				while (transform2 == null && num < childCount)
				{
					string name = AvatarBipedMapper.s_BipedBones[bipedBoneIndex].name;
					int index = AvatarBipedMapper.s_BipedBones[bipedBoneIndex].index;
					if (transform.GetChild(num).name.EndsWith(name))
					{
						transform2 = transform.GetChild(num);
						if (transform2 != null && report != null && index != 0 && transform != parentTransform)
						{
							string text = string.Concat(new string[]
							{
								"- Invalid parent for ",
								transform2.name,
								". Expected ",
								parentTransform.name,
								", but found ",
								transform.name,
								"."
							});
							if (index == 1 || index == 2)
							{
								text += " Disable Triangle Pelvis";
							}
							else if (index == 11 || index == 12)
							{
								text += " Enable Triangle Neck";
							}
							else if (index == 9)
							{
								text += " Preferred is three Spine Links";
							}
							else if (index == 10)
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
					transform2 = AvatarBipedMapper.MapBipedBone(bipedBoneIndex, transform.GetChild(num2), parentTransform, report);
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
