using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor
{
	internal class AvatarAutoMapper
	{
		private enum Side
		{
			None,
			Left,
			Right
		}

		private struct BoneMappingItem
		{
			public int parent;

			public int bone;

			public int minStep;

			public int maxStep;

			public float lengthRatio;

			public Vector3 dir;

			public AvatarAutoMapper.Side side;

			public bool optional;

			public bool alwaysInclude;

			public string[] keywords;

			private int[] children;

			public BoneMappingItem(int parent, int bone, int minStep, int maxStep, float lengthRatio, Vector3 dir, AvatarAutoMapper.Side side, bool optional, bool alwaysInclude, params string[] keywords)
			{
				this.parent = parent;
				this.bone = bone;
				this.minStep = minStep;
				this.maxStep = maxStep;
				this.lengthRatio = lengthRatio;
				this.dir = dir;
				this.side = side;
				this.optional = optional;
				this.alwaysInclude = alwaysInclude;
				this.keywords = keywords;
				this.children = null;
			}

			public BoneMappingItem(int parent, int bone, int minStep, int maxStep, float lengthRatio, AvatarAutoMapper.Side side, bool optional, bool alwaysInclude, params string[] keywords)
			{
				this = new AvatarAutoMapper.BoneMappingItem(parent, bone, minStep, maxStep, lengthRatio, Vector3.zero, side, optional, alwaysInclude, keywords);
			}

			public BoneMappingItem(int parent, int bone, int minStep, int maxStep, float lengthRatio, Vector3 dir, AvatarAutoMapper.Side side, params string[] keywords)
			{
				this = new AvatarAutoMapper.BoneMappingItem(parent, bone, minStep, maxStep, lengthRatio, dir, side, false, false, keywords);
			}

			public BoneMappingItem(int parent, int bone, int minStep, int maxStep, float lengthRatio, AvatarAutoMapper.Side side, params string[] keywords)
			{
				this = new AvatarAutoMapper.BoneMappingItem(parent, bone, minStep, maxStep, lengthRatio, Vector3.zero, side, false, false, keywords);
			}

			public int[] GetChildren(AvatarAutoMapper.BoneMappingItem[] mappingData)
			{
				if (this.children == null)
				{
					List<int> list = new List<int>();
					for (int i = 0; i < mappingData.Length; i++)
					{
						if (mappingData[i].parent == this.bone)
						{
							list.Add(i);
						}
					}
					this.children = list.ToArray();
				}
				return this.children;
			}
		}

		private class BoneMatch : IComparable<AvatarAutoMapper.BoneMatch>
		{
			public AvatarAutoMapper.BoneMatch parent;

			public List<AvatarAutoMapper.BoneMatch> children = new List<AvatarAutoMapper.BoneMatch>();

			public bool doMap = false;

			public AvatarAutoMapper.BoneMappingItem item;

			public Transform bone;

			public float score;

			public float siblingScore;

			public List<string> debugTracker = new List<string>();

			public AvatarAutoMapper.BoneMatch humanBoneParent
			{
				get
				{
					AvatarAutoMapper.BoneMatch boneMatch = this.parent;
					while (boneMatch.parent != null && boneMatch.item.bone < 0)
					{
						boneMatch = boneMatch.parent;
					}
					return boneMatch;
				}
			}

			public float totalSiblingScore
			{
				get
				{
					return this.score + this.siblingScore;
				}
			}

			public BoneMatch(AvatarAutoMapper.BoneMatch parent, Transform bone, AvatarAutoMapper.BoneMappingItem item)
			{
				this.parent = parent;
				this.bone = bone;
				this.item = item;
			}

			public int CompareTo(AvatarAutoMapper.BoneMatch other)
			{
				int result;
				if (other == null)
				{
					result = 1;
				}
				else
				{
					result = other.totalSiblingScore.CompareTo(this.totalSiblingScore);
				}
				return result;
			}
		}

		private struct QueuedBone
		{
			public Transform bone;

			public int level;

			public QueuedBone(Transform bone, int level)
			{
				this.bone = bone;
				this.level = level;
			}
		}

		private static bool kDebug = false;

		private static string[] kShoulderKeywords = new string[]
		{
			"shoulder",
			"collar",
			"clavicle"
		};

		private static string[] kUpperArmKeywords = new string[]
		{
			"up"
		};

		private static string[] kLowerArmKeywords = new string[]
		{
			"lo",
			"fore",
			"elbow"
		};

		private static string[] kHandKeywords = new string[]
		{
			"hand",
			"wrist"
		};

		private static string[] kUpperLegKeywords = new string[]
		{
			"up",
			"thigh"
		};

		private static string[] kLowerLegKeywords = new string[]
		{
			"lo",
			"calf",
			"knee",
			"shin"
		};

		private static string[] kFootKeywords = new string[]
		{
			"foot",
			"ankle"
		};

		private static string[] kToeKeywords = new string[]
		{
			"toe",
			"!end",
			"!top",
			"!nub"
		};

		private static string[] kNeckKeywords = new string[]
		{
			"neck"
		};

		private static string[] kHeadKeywords = new string[]
		{
			"head"
		};

		private static string[] kJawKeywords = new string[]
		{
			"jaw",
			"open",
			"!teeth",
			"!tongue",
			"!pony",
			"!braid",
			"!end",
			"!top",
			"!nub"
		};

		private static string[] kEyeKeywords = new string[]
		{
			"eye",
			"ball",
			"!brow",
			"!lid",
			"!pony",
			"!braid",
			"!end",
			"!top",
			"!nub"
		};

		private static string[] kThumbKeywords = new string[]
		{
			"thu",
			"!palm",
			"!wrist",
			"!end",
			"!top",
			"!nub"
		};

		private static string[] kIndexFingerKeywords = new string[]
		{
			"ind",
			"point",
			"!palm",
			"!wrist",
			"!end",
			"!top",
			"!nub"
		};

		private static string[] kMiddleFingerKeywords = new string[]
		{
			"mid",
			"long",
			"!palm",
			"!wrist",
			"!end",
			"!top",
			"!nub"
		};

		private static string[] kRingFingerKeywords = new string[]
		{
			"rin",
			"!palm",
			"!wrist",
			"!end",
			"!top",
			"!nub"
		};

		private static string[] kLittleFingerKeywords = new string[]
		{
			"lit",
			"pin",
			"!palm",
			"!wrist",
			"!end",
			"!top",
			"!nub"
		};

		private static AvatarAutoMapper.BoneMappingItem[] s_MappingDataBody = new AvatarAutoMapper.BoneMappingItem[]
		{
			new AvatarAutoMapper.BoneMappingItem(-1, 0, 1, 3, 0f, AvatarAutoMapper.Side.None, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(0, 2, 1, 2, 0f, Vector3.right, AvatarAutoMapper.Side.Right, AvatarAutoMapper.kUpperLegKeywords),
			new AvatarAutoMapper.BoneMappingItem(2, 4, 1, 2, 3f, -Vector3.up, AvatarAutoMapper.Side.Right, AvatarAutoMapper.kLowerLegKeywords),
			new AvatarAutoMapper.BoneMappingItem(4, 6, 1, 2, 1f, -Vector3.up, AvatarAutoMapper.Side.Right, AvatarAutoMapper.kFootKeywords),
			new AvatarAutoMapper.BoneMappingItem(6, 20, 1, 2, 0.5f, Vector3.forward, AvatarAutoMapper.Side.Right, true, true, AvatarAutoMapper.kToeKeywords),
			new AvatarAutoMapper.BoneMappingItem(0, 7, 1, 3, 0f, Vector3.up, AvatarAutoMapper.Side.None, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(7, 8, 0, 3, 1.4f, Vector3.up, AvatarAutoMapper.Side.None, true, false, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(8, 12, 1, 3, 0f, Vector3.right, AvatarAutoMapper.Side.Right, true, false, AvatarAutoMapper.kShoulderKeywords),
			new AvatarAutoMapper.BoneMappingItem(12, 14, 0, 2, 0.5f, Vector3.right, AvatarAutoMapper.Side.Right, AvatarAutoMapper.kUpperArmKeywords),
			new AvatarAutoMapper.BoneMappingItem(14, 16, 1, 2, 2f, Vector3.right, AvatarAutoMapper.Side.Right, AvatarAutoMapper.kLowerArmKeywords),
			new AvatarAutoMapper.BoneMappingItem(16, 18, 1, 2, 1f, Vector3.right, AvatarAutoMapper.Side.Right, AvatarAutoMapper.kHandKeywords),
			new AvatarAutoMapper.BoneMappingItem(8, 9, 1, 3, 1.8f, Vector3.up, AvatarAutoMapper.Side.None, true, false, AvatarAutoMapper.kNeckKeywords),
			new AvatarAutoMapper.BoneMappingItem(9, 10, 0, 2, 0.3f, Vector3.up, AvatarAutoMapper.Side.None, AvatarAutoMapper.kHeadKeywords),
			new AvatarAutoMapper.BoneMappingItem(10, 23, 1, 2, 0f, Vector3.forward, AvatarAutoMapper.Side.None, true, false, AvatarAutoMapper.kJawKeywords),
			new AvatarAutoMapper.BoneMappingItem(10, 22, 1, 2, 0f, new Vector3(1f, 1f, 1f), AvatarAutoMapper.Side.Right, true, false, AvatarAutoMapper.kEyeKeywords),
			new AvatarAutoMapper.BoneMappingItem(18, -2, 1, 2, 0f, new Vector3(1f, -1f, 2f), AvatarAutoMapper.Side.Right, true, false, AvatarAutoMapper.kThumbKeywords),
			new AvatarAutoMapper.BoneMappingItem(18, -3, 1, 2, 0f, new Vector3(3f, 0f, 1f), AvatarAutoMapper.Side.Right, true, false, AvatarAutoMapper.kIndexFingerKeywords)
		};

		private static AvatarAutoMapper.BoneMappingItem[] s_LeftMappingDataHand = new AvatarAutoMapper.BoneMappingItem[]
		{
			new AvatarAutoMapper.BoneMappingItem(-2, -1, 1, 2, 0f, AvatarAutoMapper.Side.None, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(-1, 24, 1, 3, 0f, new Vector3(2f, 0f, 1f), AvatarAutoMapper.Side.None, AvatarAutoMapper.kThumbKeywords),
			new AvatarAutoMapper.BoneMappingItem(-1, 27, 1, 3, 0f, new Vector3(4f, 0f, 1f), AvatarAutoMapper.Side.None, AvatarAutoMapper.kIndexFingerKeywords),
			new AvatarAutoMapper.BoneMappingItem(-1, 30, 1, 3, 0f, new Vector3(4f, 0f, 0f), AvatarAutoMapper.Side.None, AvatarAutoMapper.kMiddleFingerKeywords),
			new AvatarAutoMapper.BoneMappingItem(-1, 33, 1, 3, 0f, new Vector3(4f, 0f, -1f), AvatarAutoMapper.Side.None, AvatarAutoMapper.kRingFingerKeywords),
			new AvatarAutoMapper.BoneMappingItem(-1, 36, 1, 3, 0f, new Vector3(4f, 0f, -2f), AvatarAutoMapper.Side.None, AvatarAutoMapper.kLittleFingerKeywords),
			new AvatarAutoMapper.BoneMappingItem(24, 25, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(27, 28, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(30, 31, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(33, 34, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(36, 37, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(25, 26, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(28, 29, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(31, 32, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(34, 35, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(37, 38, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0])
		};

		private static AvatarAutoMapper.BoneMappingItem[] s_RightMappingDataHand = new AvatarAutoMapper.BoneMappingItem[]
		{
			new AvatarAutoMapper.BoneMappingItem(-2, -1, 1, 2, 0f, AvatarAutoMapper.Side.None, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(-1, 39, 1, 3, 0f, new Vector3(2f, 0f, 1f), AvatarAutoMapper.Side.None, AvatarAutoMapper.kThumbKeywords),
			new AvatarAutoMapper.BoneMappingItem(-1, 42, 1, 3, 0f, new Vector3(4f, 0f, 1f), AvatarAutoMapper.Side.None, AvatarAutoMapper.kIndexFingerKeywords),
			new AvatarAutoMapper.BoneMappingItem(-1, 45, 1, 3, 0f, new Vector3(4f, 0f, 0f), AvatarAutoMapper.Side.None, AvatarAutoMapper.kMiddleFingerKeywords),
			new AvatarAutoMapper.BoneMappingItem(-1, 48, 1, 3, 0f, new Vector3(4f, 0f, -1f), AvatarAutoMapper.Side.None, AvatarAutoMapper.kRingFingerKeywords),
			new AvatarAutoMapper.BoneMappingItem(-1, 51, 1, 3, 0f, new Vector3(4f, 0f, -2f), AvatarAutoMapper.Side.None, AvatarAutoMapper.kLittleFingerKeywords),
			new AvatarAutoMapper.BoneMappingItem(39, 40, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(42, 43, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(45, 46, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(48, 49, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(51, 52, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(40, 41, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(43, 44, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(46, 47, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(49, 50, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0]),
			new AvatarAutoMapper.BoneMappingItem(52, 53, 1, 1, 0f, AvatarAutoMapper.Side.None, false, true, new string[0])
		};

		private static bool s_DidPerformInit = false;

		private Dictionary<Transform, bool> m_ValidBones;

		private bool m_TreatDummyBonesAsReal = false;

		private Quaternion m_Orientation;

		private int m_MappingIndexOffset = 0;

		private AvatarAutoMapper.BoneMappingItem[] m_MappingData;

		private Dictionary<string, int> m_BoneHasKeywordDict;

		private Dictionary<string, int> m_BoneHasBadKeywordDict;

		private Dictionary<int, AvatarAutoMapper.BoneMatch> m_BoneMatchDict;

		private const string kLeftMatch = "(^|.*[ \\.:_-])[lL]($|[ \\.:_-].*)";

		private const string kRightMatch = "(^|.*[ \\.:_-])[rR]($|[ \\.:_-].*)";

		public AvatarAutoMapper(Dictionary<Transform, bool> validBones)
		{
			this.m_BoneHasKeywordDict = new Dictionary<string, int>();
			this.m_BoneHasBadKeywordDict = new Dictionary<string, int>();
			this.m_BoneMatchDict = new Dictionary<int, AvatarAutoMapper.BoneMatch>();
			this.m_ValidBones = validBones;
		}

		private static int GetLeftBoneIndexFromRight(int rightIndex)
		{
			int result;
			if (rightIndex < 0)
			{
				result = rightIndex;
			}
			else if (rightIndex < 54)
			{
				string text = Enum.GetName(typeof(HumanBodyBones), rightIndex);
				text = text.Replace("Right", "Left");
				result = (int)((HumanBodyBones)Enum.Parse(typeof(HumanBodyBones), text));
			}
			else
			{
				result = rightIndex + 24 - 39;
			}
			return result;
		}

		public static void InitGlobalMappingData()
		{
			if (!AvatarAutoMapper.s_DidPerformInit)
			{
				List<AvatarAutoMapper.BoneMappingItem> list = new List<AvatarAutoMapper.BoneMappingItem>(AvatarAutoMapper.s_MappingDataBody);
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					AvatarAutoMapper.BoneMappingItem boneMappingItem = list[i];
					if (boneMappingItem.side == AvatarAutoMapper.Side.Right)
					{
						int leftBoneIndexFromRight = AvatarAutoMapper.GetLeftBoneIndexFromRight(boneMappingItem.bone);
						int leftBoneIndexFromRight2 = AvatarAutoMapper.GetLeftBoneIndexFromRight(boneMappingItem.parent);
						list.Add(new AvatarAutoMapper.BoneMappingItem(leftBoneIndexFromRight2, leftBoneIndexFromRight, boneMappingItem.minStep, boneMappingItem.maxStep, boneMappingItem.lengthRatio, new Vector3(-boneMappingItem.dir.x, boneMappingItem.dir.y, boneMappingItem.dir.z), AvatarAutoMapper.Side.Left, boneMappingItem.optional, boneMappingItem.alwaysInclude, boneMappingItem.keywords));
					}
				}
				AvatarAutoMapper.s_MappingDataBody = list.ToArray();
				for (int j = 0; j < AvatarAutoMapper.s_MappingDataBody.Length; j++)
				{
					AvatarAutoMapper.s_MappingDataBody[j].GetChildren(AvatarAutoMapper.s_MappingDataBody);
				}
				for (int k = 0; k < AvatarAutoMapper.s_LeftMappingDataHand.Length; k++)
				{
					AvatarAutoMapper.s_LeftMappingDataHand[k].GetChildren(AvatarAutoMapper.s_LeftMappingDataHand);
				}
				for (int l = 0; l < AvatarAutoMapper.s_RightMappingDataHand.Length; l++)
				{
					AvatarAutoMapper.s_RightMappingDataHand[l].GetChildren(AvatarAutoMapper.s_RightMappingDataHand);
				}
				AvatarAutoMapper.s_DidPerformInit = true;
			}
		}

		public static Dictionary<int, Transform> MapBones(Transform root, Dictionary<Transform, bool> validBones)
		{
			AvatarAutoMapper avatarAutoMapper = new AvatarAutoMapper(validBones);
			return avatarAutoMapper.MapBones(root);
		}

		public Dictionary<int, Transform> MapBones(Transform root)
		{
			AvatarAutoMapper.InitGlobalMappingData();
			Dictionary<int, Transform> dictionary = new Dictionary<int, Transform>();
			this.m_Orientation = Quaternion.identity;
			this.m_MappingData = AvatarAutoMapper.s_MappingDataBody;
			this.m_MappingIndexOffset = 0;
			this.m_BoneMatchDict.Clear();
			AvatarAutoMapper.BoneMatch boneMatch = new AvatarAutoMapper.BoneMatch(null, root, this.m_MappingData[0]);
			this.m_TreatDummyBonesAsReal = false;
			this.MapBonesFromRootDown(boneMatch, dictionary);
			if (dictionary.Count < 15)
			{
				this.m_TreatDummyBonesAsReal = true;
				this.MapBonesFromRootDown(boneMatch, dictionary);
			}
			if (dictionary.ContainsKey(1) && dictionary.ContainsKey(2) && dictionary.ContainsKey(13) && dictionary.ContainsKey(14))
			{
				this.m_Orientation = AvatarSetupTool.AvatarComputeOrientation(dictionary[1].position, dictionary[2].position, dictionary[13].position, dictionary[14].position);
				if (Vector3.Angle(this.m_Orientation * Vector3.up, Vector3.up) > 20f || Vector3.Angle(this.m_Orientation * Vector3.forward, Vector3.forward) > 20f)
				{
					if (AvatarAutoMapper.kDebug)
					{
						Debug.Log("*** Mapping with new computed orientation");
					}
					dictionary.Clear();
					this.m_BoneMatchDict.Clear();
					this.MapBonesFromRootDown(boneMatch, dictionary);
				}
			}
			bool flag = !this.m_ValidBones.ContainsKey(root) || !this.m_ValidBones[root];
			if (flag && dictionary.Count > 0 && dictionary.ContainsKey(0))
			{
				while (true)
				{
					Transform parent = dictionary[0].parent;
					if (!(parent != null) || !(parent != boneMatch.bone) || !this.m_ValidBones.ContainsKey(parent) || !this.m_ValidBones[parent])
					{
						break;
					}
					dictionary[0] = parent;
				}
			}
			int num = 3;
			Quaternion orientation = this.m_Orientation;
			if (dictionary.ContainsKey(17))
			{
				Transform transform = dictionary[15];
				Transform transform2 = dictionary[17];
				this.m_Orientation = Quaternion.FromToRotation(orientation * -Vector3.right, transform2.position - transform.position) * orientation;
				this.m_MappingData = AvatarAutoMapper.s_LeftMappingDataHand;
				this.m_MappingIndexOffset = 24;
				this.m_BoneMatchDict.Clear();
				AvatarAutoMapper.BoneMatch rootMatch = new AvatarAutoMapper.BoneMatch(null, transform, this.m_MappingData[0]);
				this.m_TreatDummyBonesAsReal = true;
				int count = dictionary.Count;
				this.MapBonesFromRootDown(rootMatch, dictionary);
				if (dictionary.Count < count + num)
				{
					for (int i = 24; i <= 38; i++)
					{
						dictionary.Remove(i);
					}
				}
			}
			if (dictionary.ContainsKey(18))
			{
				Transform transform3 = dictionary[16];
				Transform transform4 = dictionary[18];
				this.m_Orientation = Quaternion.FromToRotation(orientation * Vector3.right, transform4.position - transform3.position) * orientation;
				this.m_MappingData = AvatarAutoMapper.s_RightMappingDataHand;
				this.m_MappingIndexOffset = 39;
				this.m_BoneMatchDict.Clear();
				AvatarAutoMapper.BoneMatch rootMatch2 = new AvatarAutoMapper.BoneMatch(null, transform3, this.m_MappingData[0]);
				this.m_TreatDummyBonesAsReal = true;
				int count2 = dictionary.Count;
				this.MapBonesFromRootDown(rootMatch2, dictionary);
				if (dictionary.Count < count2 + num)
				{
					for (int j = 39; j <= 53; j++)
					{
						dictionary.Remove(j);
					}
				}
			}
			return dictionary;
		}

		private void MapBonesFromRootDown(AvatarAutoMapper.BoneMatch rootMatch, Dictionary<int, Transform> mapping)
		{
			List<AvatarAutoMapper.BoneMatch> list = this.RecursiveFindPotentialBoneMatches(rootMatch, this.m_MappingData[0], true);
			if (list != null && list.Count > 0)
			{
				if (AvatarAutoMapper.kDebug)
				{
					this.EvaluateBoneMatch(list[0], true);
				}
				this.ApplyMapping(list[0], mapping);
			}
		}

		private void ApplyMapping(AvatarAutoMapper.BoneMatch match, Dictionary<int, Transform> mapping)
		{
			if (match.doMap)
			{
				mapping[match.item.bone] = match.bone;
			}
			foreach (AvatarAutoMapper.BoneMatch current in match.children)
			{
				this.ApplyMapping(current, mapping);
			}
		}

		private string GetStrippedAndNiceBoneName(Transform bone)
		{
			string[] array = bone.name.Split(new char[]
			{
				':'
			});
			return ObjectNames.NicifyVariableName(array[array.Length - 1]);
		}

		private int BoneHasBadKeyword(Transform bone, params string[] keywords)
		{
			string key = bone.GetInstanceID() + ":" + string.Concat(keywords);
			int result;
			if (this.m_BoneHasBadKeywordDict.ContainsKey(key))
			{
				result = this.m_BoneHasBadKeywordDict[key];
			}
			else
			{
				int num = 0;
				Transform parent = bone.parent;
				while (parent.parent != null && this.m_ValidBones.ContainsKey(parent) && !this.m_ValidBones[parent])
				{
					parent = parent.parent;
				}
				string text = this.GetStrippedAndNiceBoneName(parent).ToLower();
				for (int i = 0; i < keywords.Length; i++)
				{
					string text2 = keywords[i];
					if (text2[0] != '!' && text.Contains(text2))
					{
						num = -20;
						this.m_BoneHasBadKeywordDict[key] = num;
						result = num;
						return result;
					}
				}
				text = this.GetStrippedAndNiceBoneName(bone).ToLower();
				for (int j = 0; j < keywords.Length; j++)
				{
					string text3 = keywords[j];
					if (text3[0] == '!' && text.Contains(text3.Substring(1)))
					{
						num = -1000;
						this.m_BoneHasBadKeywordDict[key] = num;
						result = num;
						return result;
					}
				}
				this.m_BoneHasBadKeywordDict[key] = num;
				result = num;
			}
			return result;
		}

		private int BoneHasKeyword(Transform bone, params string[] keywords)
		{
			string key = bone.GetInstanceID() + ":" + string.Concat(keywords);
			int result;
			if (this.m_BoneHasKeywordDict.ContainsKey(key))
			{
				result = this.m_BoneHasKeywordDict[key];
			}
			else
			{
				int num = 0;
				string text = this.GetStrippedAndNiceBoneName(bone).ToLower();
				for (int i = 0; i < keywords.Length; i++)
				{
					string text2 = keywords[i];
					if (text2[0] != '!' && text.Contains(text2))
					{
						num = 20;
						this.m_BoneHasKeywordDict[key] = num;
						result = num;
						return result;
					}
				}
				this.m_BoneHasKeywordDict[key] = num;
				result = num;
			}
			return result;
		}

		private bool MatchesSideKeywords(string boneName, bool left)
		{
			return boneName.ToLower().Contains((!left) ? "right" : "left") || Regex.Match(boneName, (!left) ? "(^|.*[ \\.:_-])[rR]($|[ \\.:_-].*)" : "(^|.*[ \\.:_-])[lL]($|[ \\.:_-].*)").Length > 0;
		}

		private int GetBoneSideMatchPoints(AvatarAutoMapper.BoneMatch match)
		{
			string name = match.bone.name;
			int result;
			if (match.item.side == AvatarAutoMapper.Side.None && (this.MatchesSideKeywords(name, false) || this.MatchesSideKeywords(name, true)))
			{
				result = -1000;
			}
			else
			{
				bool flag = match.item.side == AvatarAutoMapper.Side.Left;
				if (this.MatchesSideKeywords(name, flag))
				{
					result = 15;
				}
				else if (this.MatchesSideKeywords(name, !flag))
				{
					result = -1000;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		private int GetMatchKey(AvatarAutoMapper.BoneMatch parentMatch, Transform t, AvatarAutoMapper.BoneMappingItem goalItem)
		{
			int num = goalItem.bone;
			num += t.GetInstanceID() * 1000;
			if (parentMatch != null)
			{
				num += parentMatch.bone.GetInstanceID() * 1000000;
				if (parentMatch.parent != null)
				{
					num += parentMatch.parent.bone.GetInstanceID() * 1000000000;
				}
			}
			return num;
		}

		private List<AvatarAutoMapper.BoneMatch> RecursiveFindPotentialBoneMatches(AvatarAutoMapper.BoneMatch parentMatch, AvatarAutoMapper.BoneMappingItem goalItem, bool confirmedChoice)
		{
			List<AvatarAutoMapper.BoneMatch> list = new List<AvatarAutoMapper.BoneMatch>();
			Queue<AvatarAutoMapper.QueuedBone> queue = new Queue<AvatarAutoMapper.QueuedBone>();
			queue.Enqueue(new AvatarAutoMapper.QueuedBone(parentMatch.bone, 0));
			while (queue.Count > 0)
			{
				AvatarAutoMapper.QueuedBone queuedBone = queue.Dequeue();
				Transform bone = queuedBone.bone;
				if (queuedBone.level >= goalItem.minStep && (this.m_TreatDummyBonesAsReal || this.m_ValidBones == null || (this.m_ValidBones.ContainsKey(bone) && this.m_ValidBones[bone])))
				{
					int matchKey = this.GetMatchKey(parentMatch, bone, goalItem);
					AvatarAutoMapper.BoneMatch boneMatch;
					if (this.m_BoneMatchDict.ContainsKey(matchKey))
					{
						boneMatch = this.m_BoneMatchDict[matchKey];
					}
					else
					{
						boneMatch = new AvatarAutoMapper.BoneMatch(parentMatch, bone, goalItem);
						this.EvaluateBoneMatch(boneMatch, false);
						this.m_BoneMatchDict[matchKey] = boneMatch;
					}
					if (boneMatch.score > 0f || AvatarAutoMapper.kDebug)
					{
						list.Add(boneMatch);
					}
				}
				if (queuedBone.level < goalItem.maxStep)
				{
					IEnumerator enumerator = bone.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							Transform transform = (Transform)enumerator.Current;
							if (this.m_ValidBones == null || this.m_ValidBones.ContainsKey(transform))
							{
								if (!this.m_TreatDummyBonesAsReal && this.m_ValidBones != null && !this.m_ValidBones[transform])
								{
									queue.Enqueue(new AvatarAutoMapper.QueuedBone(transform, queuedBone.level));
								}
								else
								{
									queue.Enqueue(new AvatarAutoMapper.QueuedBone(transform, queuedBone.level + 1));
								}
							}
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
			List<AvatarAutoMapper.BoneMatch> result;
			if (list.Count == 0)
			{
				result = null;
			}
			else
			{
				list.Sort();
				if (list[0].score <= 0f)
				{
					result = null;
				}
				else
				{
					if (AvatarAutoMapper.kDebug && confirmedChoice)
					{
						this.DebugMatchChoice(list);
					}
					while (list.Count > 3)
					{
						list.RemoveAt(list.Count - 1);
					}
					list.TrimExcess();
					result = list;
				}
			}
			return result;
		}

		private string GetNameOfBone(int boneIndex)
		{
			string result;
			if (boneIndex < 0)
			{
				result = "" + boneIndex;
			}
			else
			{
				result = "" + (HumanBodyBones)boneIndex;
			}
			return result;
		}

		private string GetMatchString(AvatarAutoMapper.BoneMatch match)
		{
			return this.GetNameOfBone(match.item.bone) + ":" + ((!(match.bone == null)) ? match.bone.name : "null");
		}

		private void DebugMatchChoice(List<AvatarAutoMapper.BoneMatch> matches)
		{
			string text = this.GetNameOfBone(matches[0].item.bone) + " preferred order: ";
			for (int i = 0; i < matches.Count; i++)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					matches[i].bone.name,
					" (",
					matches[i].score.ToString("0.0"),
					" / ",
					matches[i].totalSiblingScore.ToString("0.0"),
					"), "
				});
			}
			foreach (AvatarAutoMapper.BoneMatch current in matches)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"\n   Match ",
					current.bone.name,
					" (",
					current.score.ToString("0.0"),
					" / ",
					current.totalSiblingScore.ToString("0.0"),
					"):"
				});
				foreach (string current2 in current.debugTracker)
				{
					text = text + "\n    - " + current2;
				}
			}
			Debug.Log(text);
		}

		private AvatarAutoMapper.BoneMappingItem GetBoneMappingItem(int bone)
		{
			AvatarAutoMapper.BoneMappingItem[] mappingData = this.m_MappingData;
			AvatarAutoMapper.BoneMappingItem result;
			for (int i = 0; i < mappingData.Length; i++)
			{
				AvatarAutoMapper.BoneMappingItem boneMappingItem = mappingData[i];
				if (boneMappingItem.bone == bone)
				{
					result = boneMappingItem;
					return result;
				}
			}
			result = default(AvatarAutoMapper.BoneMappingItem);
			return result;
		}

		private bool IsParentOfOther(Transform knownCommonParent, Transform potentialParent, Transform potentialChild)
		{
			Transform transform = potentialChild;
			bool result;
			while (transform != knownCommonParent)
			{
				if (transform == potentialParent)
				{
					result = true;
				}
				else
				{
					if (!(transform == knownCommonParent))
					{
						transform = transform.parent;
						continue;
					}
					result = false;
				}
				return result;
			}
			result = false;
			return result;
		}

		private bool ShareTransformPath(Transform commonParent, Transform childA, Transform childB)
		{
			return this.IsParentOfOther(commonParent, childA, childB) || this.IsParentOfOther(commonParent, childB, childA);
		}

		private List<AvatarAutoMapper.BoneMatch> GetBestChildMatches(AvatarAutoMapper.BoneMatch parentMatch, List<List<AvatarAutoMapper.BoneMatch>> childMatchesLists)
		{
			List<AvatarAutoMapper.BoneMatch> list = new List<AvatarAutoMapper.BoneMatch>();
			List<AvatarAutoMapper.BoneMatch> result;
			if (childMatchesLists.Count == 1)
			{
				list.Add(childMatchesLists[0][0]);
				result = list;
			}
			else
			{
				int[] array = new int[childMatchesLists.Count];
				float num;
				array = this.GetBestChildMatchChoices(parentMatch, childMatchesLists, array, out num);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] >= 0)
					{
						list.Add(childMatchesLists[i][array[i]]);
					}
				}
				result = list;
			}
			return result;
		}

		private int[] GetBestChildMatchChoices(AvatarAutoMapper.BoneMatch parentMatch, List<List<AvatarAutoMapper.BoneMatch>> childMatchesLists, int[] choices, out float score)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < choices.Length; i++)
			{
				if (choices[i] >= 0)
				{
					list.Clear();
					list.Add(i);
					for (int j = i + 1; j < choices.Length; j++)
					{
						if (choices[j] >= 0)
						{
							if (this.ShareTransformPath(parentMatch.bone, childMatchesLists[i][choices[i]].bone, childMatchesLists[j][choices[j]].bone))
							{
								list.Add(j);
							}
						}
					}
					if (list.Count > 1)
					{
						break;
					}
				}
			}
			int[] result;
			if (list.Count <= 1)
			{
				score = 0f;
				for (int k = 0; k < choices.Length; k++)
				{
					if (choices[k] >= 0)
					{
						score += childMatchesLists[k][choices[k]].totalSiblingScore;
					}
				}
				result = choices;
			}
			else
			{
				float num = 0f;
				int[] array = choices;
				for (int l = 0; l < list.Count; l++)
				{
					int[] array2 = new int[choices.Length];
					Array.Copy(choices, array2, choices.Length);
					for (int m = 0; m < list.Count; m++)
					{
						if (l != m)
						{
							if (list[m] >= array2.Length)
							{
								Debug.LogError(string.Concat(new object[]
								{
									"sharedIndices[j] (",
									list[m],
									") >= altChoices.Length (",
									array2.Length,
									")"
								}));
							}
							if (list[m] >= childMatchesLists.Count)
							{
								Debug.LogError(string.Concat(new object[]
								{
									"sharedIndices[j] (",
									list[m],
									") >= childMatchesLists.Count (",
									childMatchesLists.Count,
									")"
								}));
							}
							if (array2[list[m]] < childMatchesLists[list[m]].Count - 1)
							{
								array2[list[m]]++;
							}
							else
							{
								array2[list[m]] = -1;
							}
						}
					}
					float num2;
					array2 = this.GetBestChildMatchChoices(parentMatch, childMatchesLists, array2, out num2);
					if (num2 > num)
					{
						num = num2;
						array = array2;
					}
				}
				score = num;
				result = array;
			}
			return result;
		}

		private void EvaluateBoneMatch(AvatarAutoMapper.BoneMatch match, bool confirmedChoice)
		{
			match.score = 0f;
			match.siblingScore = 0f;
			List<List<AvatarAutoMapper.BoneMatch>> list = new List<List<AvatarAutoMapper.BoneMatch>>();
			int num = 0;
			int[] children = match.item.GetChildren(this.m_MappingData);
			int i = 0;
			while (i < children.Length)
			{
				int num2 = children[i];
				AvatarAutoMapper.BoneMappingItem goalItem = this.m_MappingData[num2];
				if (goalItem.parent == match.item.bone)
				{
					num++;
					List<AvatarAutoMapper.BoneMatch> list2 = this.RecursiveFindPotentialBoneMatches(match, goalItem, confirmedChoice);
					if (list2 != null && list2.Count != 0)
					{
						list.Add(list2);
					}
				}
				IL_9D:
				i++;
				continue;
				goto IL_9D;
			}
			bool flag = match.bone == match.humanBoneParent.bone;
			int num3 = 0;
			if (list.Count > 0)
			{
				match.children = this.GetBestChildMatches(match, list);
				foreach (AvatarAutoMapper.BoneMatch current in match.children)
				{
					if (AvatarAutoMapper.kDebug && confirmedChoice)
					{
						this.EvaluateBoneMatch(current, confirmedChoice);
					}
					num3++;
					match.score += current.score;
					if (AvatarAutoMapper.kDebug)
					{
						match.debugTracker.AddRange(current.debugTracker);
					}
					if (current.bone == match.bone && current.item.bone >= 0)
					{
						flag = true;
					}
				}
			}
			if (!match.item.optional || !flag)
			{
				this.ScoreBoneMatch(match);
			}
			if (match.item.dir != Vector3.zero)
			{
				Vector3 dir = match.item.dir;
				if (this.m_MappingIndexOffset >= 24 && this.m_MappingIndexOffset < 39)
				{
					dir.x *= -1f;
				}
				Vector3 vector = (match.bone.position - match.humanBoneParent.bone.position).normalized;
				vector = Quaternion.Inverse(this.m_Orientation) * vector;
				float num4 = Vector3.Dot(vector, dir) * (float)((!match.item.optional) ? 10 : 5);
				match.siblingScore += num4;
				if (AvatarAutoMapper.kDebug)
				{
					match.debugTracker.Add(string.Concat(new object[]
					{
						"* ",
						num4,
						": ",
						this.GetMatchString(match),
						" matched dir (",
						(match.bone.position - match.humanBoneParent.bone.position).normalized,
						" , ",
						dir,
						")"
					}));
				}
				if (num4 > 0f)
				{
					match.score += 10f;
					if (AvatarAutoMapper.kDebug)
					{
						match.debugTracker.Add(string.Concat(new object[]
						{
							10,
							": ",
							this.GetMatchString(match),
							" matched dir (",
							(match.bone.position - match.humanBoneParent.bone.position).normalized,
							" , ",
							dir,
							")"
						}));
					}
				}
			}
			if (this.m_MappingIndexOffset == 0)
			{
				int boneSideMatchPoints = this.GetBoneSideMatchPoints(match);
				if (match.parent.item.side == AvatarAutoMapper.Side.None || boneSideMatchPoints < 0)
				{
					match.siblingScore += (float)boneSideMatchPoints;
					if (AvatarAutoMapper.kDebug)
					{
						match.debugTracker.Add(string.Concat(new object[]
						{
							"* ",
							boneSideMatchPoints,
							": ",
							this.GetMatchString(match),
							" matched side"
						}));
					}
				}
			}
			if (match.score > 0f)
			{
				if (match.item.optional && !flag)
				{
					match.score += 5f;
					if (AvatarAutoMapper.kDebug)
					{
						match.debugTracker.Add(string.Concat(new object[]
						{
							5,
							": ",
							this.GetMatchString(match),
							" optional bone is included"
						}));
					}
				}
				if (num == 0 && match.bone.childCount > 0)
				{
					match.score += 1f;
					if (AvatarAutoMapper.kDebug)
					{
						match.debugTracker.Add(string.Concat(new object[]
						{
							1,
							": ",
							this.GetMatchString(match),
							" has dummy child bone"
						}));
					}
				}
				if (match.item.lengthRatio != 0f)
				{
					float num5 = Vector3.Distance(match.bone.position, match.humanBoneParent.bone.position);
					if (num5 == 0f && match.bone != match.humanBoneParent.bone)
					{
						match.score -= 1000f;
						if (AvatarAutoMapper.kDebug)
						{
							match.debugTracker.Add(string.Concat(new object[]
							{
								-1000,
								": ",
								this.GetMatchString(match.humanBoneParent),
								" has zero length"
							}));
						}
					}
					float num6 = Vector3.Distance(match.humanBoneParent.bone.position, match.humanBoneParent.humanBoneParent.bone.position);
					if (num6 > 0f)
					{
						float num7 = Mathf.Log(num5 / num6, 2f);
						float num8 = Mathf.Log(match.item.lengthRatio, 2f);
						float num9 = 10f * Mathf.Clamp(1f - 0.6f * Mathf.Abs(num7 - num8), 0f, 1f);
						match.score += num9;
						if (AvatarAutoMapper.kDebug)
						{
							match.debugTracker.Add(string.Concat(new object[]
							{
								num9,
								": parent ",
								this.GetMatchString(match.humanBoneParent),
								" matched lengthRatio - ",
								num5,
								" / ",
								num6,
								" = ",
								num5 / num6,
								" (",
								num7,
								") goal: ",
								match.item.lengthRatio,
								" (",
								num8,
								")"
							}));
						}
					}
				}
			}
			if (match.item.bone >= 0 && (!match.item.optional || !flag))
			{
				match.doMap = true;
			}
		}

		private void ScoreBoneMatch(AvatarAutoMapper.BoneMatch match)
		{
			int num = this.BoneHasBadKeyword(match.bone, match.item.keywords);
			match.score += (float)num;
			if (AvatarAutoMapper.kDebug && num != 0)
			{
				match.debugTracker.Add(string.Concat(new object[]
				{
					num,
					": ",
					this.GetMatchString(match),
					" matched bad keywords"
				}));
			}
			if (num >= 0)
			{
				int num2 = this.BoneHasKeyword(match.bone, match.item.keywords);
				match.score += (float)num2;
				if (AvatarAutoMapper.kDebug && num2 != 0)
				{
					match.debugTracker.Add(string.Concat(new object[]
					{
						num2,
						": ",
						this.GetMatchString(match),
						" matched keywords"
					}));
				}
				if (match.item.keywords.Length == 0 && match.item.alwaysInclude)
				{
					match.score += 1f;
					if (AvatarAutoMapper.kDebug)
					{
						match.debugTracker.Add(string.Concat(new object[]
						{
							1,
							": ",
							this.GetMatchString(match),
							" always-include point"
						}));
					}
				}
			}
		}
	}
}
