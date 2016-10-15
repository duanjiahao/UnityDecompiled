using System;
using UnityEngine;

namespace UnityEditor
{
	internal static class CurveUtility
	{
		private const int kBrokenMask = 1;

		private const int kLeftTangentMask = 6;

		private const int kRightTangentMask = 24;

		private static Texture2D iconKey;

		private static Texture2D iconCurve;

		private static Texture2D iconNone;

		public static int GetPathAndTypeID(string path, Type type)
		{
			return path.GetHashCode() * 27 ^ type.GetHashCode();
		}

		public static Texture2D GetIconCurve()
		{
			if (CurveUtility.iconCurve == null)
			{
				CurveUtility.iconCurve = EditorGUIUtility.LoadIcon("animationanimated");
			}
			return CurveUtility.iconCurve;
		}

		public static Texture2D GetIconKey()
		{
			if (CurveUtility.iconKey == null)
			{
				CurveUtility.iconKey = EditorGUIUtility.LoadIcon("animationkeyframe");
			}
			return CurveUtility.iconKey;
		}

		public static bool HaveKeysInRange(AnimationCurve curve, float beginTime, float endTime)
		{
			for (int i = curve.length - 1; i >= 0; i--)
			{
				if (curve[i].time >= beginTime && curve[i].time < endTime)
				{
					return true;
				}
			}
			return false;
		}

		public static void RemoveKeysInRange(AnimationCurve curve, float beginTime, float endTime)
		{
			for (int i = curve.length - 1; i >= 0; i--)
			{
				if (curve[i].time >= beginTime && curve[i].time < endTime)
				{
					curve.RemoveKey(i);
				}
			}
		}

		public static void UpdateTangentsFromMode(AnimationCurve curve)
		{
			for (int i = 0; i < curve.length; i++)
			{
				CurveUtility.UpdateTangentsFromMode(curve, i);
			}
		}

		private static float CalculateLinearTangent(AnimationCurve curve, int index, int toIndex)
		{
			return (curve[index].value - curve[toIndex].value) / (curve[index].time - curve[toIndex].time);
		}

		private static void UpdateTangentsFromMode(AnimationCurve curve, int index)
		{
			if (index < 0 || index >= curve.length)
			{
				return;
			}
			Keyframe key = curve[index];
			if (CurveUtility.GetKeyTangentMode(key, 0) == TangentMode.Linear && index >= 1)
			{
				key.inTangent = CurveUtility.CalculateLinearTangent(curve, index, index - 1);
				curve.MoveKey(index, key);
			}
			if (CurveUtility.GetKeyTangentMode(key, 1) == TangentMode.Linear && index + 1 < curve.length)
			{
				key.outTangent = CurveUtility.CalculateLinearTangent(curve, index, index + 1);
				curve.MoveKey(index, key);
			}
			if (CurveUtility.GetKeyTangentMode(key, 0) == TangentMode.Smooth || CurveUtility.GetKeyTangentMode(key, 1) == TangentMode.Smooth)
			{
				curve.SmoothTangents(index, 0f);
			}
		}

		public static void UpdateTangentsFromModeSurrounding(AnimationCurve curve, int index)
		{
			CurveUtility.UpdateTangentsFromMode(curve, index - 2);
			CurveUtility.UpdateTangentsFromMode(curve, index - 1);
			CurveUtility.UpdateTangentsFromMode(curve, index);
			CurveUtility.UpdateTangentsFromMode(curve, index + 1);
			CurveUtility.UpdateTangentsFromMode(curve, index + 2);
		}

		public static float CalculateSmoothTangent(Keyframe key)
		{
			if (key.inTangent == float.PositiveInfinity)
			{
				key.inTangent = 0f;
			}
			if (key.outTangent == float.PositiveInfinity)
			{
				key.outTangent = 0f;
			}
			return (key.outTangent + key.inTangent) * 0.5f;
		}

		public static void SetKeyBroken(ref Keyframe key, bool broken)
		{
			if (broken)
			{
				key.tangentMode |= 1;
			}
			else
			{
				key.tangentMode &= -2;
			}
		}

		public static bool GetKeyBroken(Keyframe key)
		{
			return (key.tangentMode & 1) != 0;
		}

		public static void SetKeyTangentMode(ref Keyframe key, int leftRight, TangentMode mode)
		{
			if (leftRight == 0)
			{
				key.tangentMode &= -7;
				key.tangentMode |= (int)((int)mode << 1);
			}
			else
			{
				key.tangentMode &= -25;
				key.tangentMode |= (int)((int)mode << 3);
			}
			if (CurveUtility.GetKeyTangentMode(key, leftRight) != mode)
			{
				Debug.Log("bug");
			}
		}

		public static TangentMode GetKeyTangentMode(Keyframe key, int leftRight)
		{
			if (leftRight == 0)
			{
				return (TangentMode)((key.tangentMode & 6) >> 1);
			}
			return (TangentMode)((key.tangentMode & 24) >> 3);
		}

		public static void SetKeyModeFromContext(AnimationCurve curve, int keyIndex)
		{
			Keyframe key = curve[keyIndex];
			bool flag = false;
			bool flag2 = false;
			if (keyIndex > 0)
			{
				if (CurveUtility.GetKeyBroken(curve[keyIndex - 1]))
				{
					flag = true;
				}
				if (CurveUtility.GetKeyTangentMode(curve[keyIndex - 1], 1) == TangentMode.Smooth)
				{
					flag2 = true;
				}
			}
			if (keyIndex < curve.length - 1)
			{
				if (CurveUtility.GetKeyBroken(curve[keyIndex + 1]))
				{
					flag = true;
				}
				if (CurveUtility.GetKeyTangentMode(curve[keyIndex + 1], 0) == TangentMode.Smooth)
				{
					flag2 = true;
				}
			}
			CurveUtility.SetKeyBroken(ref key, flag);
			if (flag && !flag2)
			{
				if (keyIndex > 0)
				{
					CurveUtility.SetKeyTangentMode(ref key, 0, CurveUtility.GetKeyTangentMode(curve[keyIndex - 1], 1));
				}
				if (keyIndex < curve.length - 1)
				{
					CurveUtility.SetKeyTangentMode(ref key, 1, CurveUtility.GetKeyTangentMode(curve[keyIndex + 1], 0));
				}
			}
			else
			{
				TangentMode mode = TangentMode.Smooth;
				if (keyIndex > 0 && CurveUtility.GetKeyTangentMode(curve[keyIndex - 1], 1) != TangentMode.Smooth)
				{
					mode = TangentMode.Editable;
				}
				if (keyIndex < curve.length - 1 && CurveUtility.GetKeyTangentMode(curve[keyIndex + 1], 0) != TangentMode.Smooth)
				{
					mode = TangentMode.Editable;
				}
				CurveUtility.SetKeyTangentMode(ref key, 0, mode);
				CurveUtility.SetKeyTangentMode(ref key, 1, mode);
			}
			curve.MoveKey(keyIndex, key);
		}

		public static string GetClipName(AnimationClip clip)
		{
			if (clip == null)
			{
				return "[No Clip]";
			}
			string text = clip.name;
			if ((clip.hideFlags & HideFlags.NotEditable) != HideFlags.None)
			{
				text += " (Read-Only)";
			}
			return text;
		}

		public static Color GetBalancedColor(Color c)
		{
			return new Color(0.15f + 0.75f * c.r, 0.2f + 0.6f * c.g, 0.1f + 0.9f * c.b);
		}

		public static Color GetPropertyColor(string name)
		{
			Color result = Color.white;
			int num = 0;
			if (name.StartsWith("m_LocalPosition"))
			{
				num = 1;
			}
			if (name.StartsWith("localEulerAngles"))
			{
				num = 2;
			}
			if (name.StartsWith("m_LocalScale"))
			{
				num = 3;
			}
			if (num == 1)
			{
				if (name.EndsWith(".x"))
				{
					result = Handles.xAxisColor;
				}
				else if (name.EndsWith(".y"))
				{
					result = Handles.yAxisColor;
				}
				else if (name.EndsWith(".z"))
				{
					result = Handles.zAxisColor;
				}
			}
			else if (num == 2)
			{
				if (name.EndsWith(".x"))
				{
					result = AnimEditor.kEulerXColor;
				}
				else if (name.EndsWith(".y"))
				{
					result = AnimEditor.kEulerYColor;
				}
				else if (name.EndsWith(".z"))
				{
					result = AnimEditor.kEulerZColor;
				}
			}
			else if (num == 3)
			{
				if (name.EndsWith(".x"))
				{
					result = CurveUtility.GetBalancedColor(new Color(0.7f, 0.4f, 0.4f));
				}
				else if (name.EndsWith(".y"))
				{
					result = CurveUtility.GetBalancedColor(new Color(0.4f, 0.7f, 0.4f));
				}
				else if (name.EndsWith(".z"))
				{
					result = CurveUtility.GetBalancedColor(new Color(0.4f, 0.4f, 0.7f));
				}
			}
			else if (name.EndsWith(".x"))
			{
				result = Handles.xAxisColor;
			}
			else if (name.EndsWith(".y"))
			{
				result = Handles.yAxisColor;
			}
			else if (name.EndsWith(".z"))
			{
				result = Handles.zAxisColor;
			}
			else if (name.EndsWith(".w"))
			{
				result = new Color(1f, 0.5f, 0f);
			}
			else if (name.EndsWith(".r"))
			{
				result = CurveUtility.GetBalancedColor(Color.red);
			}
			else if (name.EndsWith(".g"))
			{
				result = CurveUtility.GetBalancedColor(Color.green);
			}
			else if (name.EndsWith(".b"))
			{
				result = CurveUtility.GetBalancedColor(Color.blue);
			}
			else if (name.EndsWith(".a"))
			{
				result = CurveUtility.GetBalancedColor(Color.yellow);
			}
			else if (name.EndsWith(".width"))
			{
				result = CurveUtility.GetBalancedColor(Color.blue);
			}
			else if (name.EndsWith(".height"))
			{
				result = CurveUtility.GetBalancedColor(Color.yellow);
			}
			else
			{
				float num2 = 6.28318548f * (float)(name.GetHashCode() % 1000);
				num2 -= Mathf.Floor(num2);
				result = CurveUtility.GetBalancedColor(Color.HSVToRGB(num2, 1f, 1f));
			}
			result.a = 1f;
			return result;
		}
	}
}
