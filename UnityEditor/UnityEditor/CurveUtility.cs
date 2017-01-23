using System;
using UnityEngine;

namespace UnityEditor
{
	internal static class CurveUtility
	{
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
			bool result;
			for (int i = curve.length - 1; i >= 0; i--)
			{
				if (curve[i].time >= beginTime && curve[i].time < endTime)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
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

		public static void SetKeyModeFromContext(AnimationCurve curve, int keyIndex)
		{
			Keyframe key = curve[keyIndex];
			bool flag = false;
			bool flag2 = false;
			if (keyIndex > 0)
			{
				if (AnimationUtility.GetKeyBroken(curve[keyIndex - 1]))
				{
					flag = true;
				}
				if (AnimationUtility.GetKeyRightTangentMode(curve[keyIndex - 1]) == AnimationUtility.TangentMode.ClampedAuto)
				{
					flag2 = true;
				}
			}
			if (keyIndex < curve.length - 1)
			{
				if (AnimationUtility.GetKeyBroken(curve[keyIndex + 1]))
				{
					flag = true;
				}
				if (AnimationUtility.GetKeyLeftTangentMode(curve[keyIndex + 1]) == AnimationUtility.TangentMode.ClampedAuto)
				{
					flag2 = true;
				}
			}
			AnimationUtility.SetKeyBroken(ref key, flag);
			if (flag && !flag2)
			{
				if (keyIndex > 0)
				{
					AnimationUtility.SetKeyLeftTangentMode(ref key, AnimationUtility.GetKeyRightTangentMode(curve[keyIndex - 1]));
				}
				if (keyIndex < curve.length - 1)
				{
					AnimationUtility.SetKeyRightTangentMode(ref key, AnimationUtility.GetKeyLeftTangentMode(curve[keyIndex + 1]));
				}
			}
			else
			{
				AnimationUtility.TangentMode tangentMode = AnimationUtility.TangentMode.Free;
				if ((keyIndex == 0 || AnimationUtility.GetKeyRightTangentMode(curve[keyIndex - 1]) == AnimationUtility.TangentMode.ClampedAuto) && (keyIndex == curve.length - 1 || AnimationUtility.GetKeyLeftTangentMode(curve[keyIndex + 1]) == AnimationUtility.TangentMode.ClampedAuto))
				{
					tangentMode = AnimationUtility.TangentMode.ClampedAuto;
				}
				else if ((keyIndex == 0 || AnimationUtility.GetKeyRightTangentMode(curve[keyIndex - 1]) == AnimationUtility.TangentMode.Auto) && (keyIndex == curve.length - 1 || AnimationUtility.GetKeyLeftTangentMode(curve[keyIndex + 1]) == AnimationUtility.TangentMode.Auto))
				{
					tangentMode = AnimationUtility.TangentMode.Auto;
				}
				AnimationUtility.SetKeyLeftTangentMode(ref key, tangentMode);
				AnimationUtility.SetKeyRightTangentMode(ref key, tangentMode);
			}
			curve.MoveKey(keyIndex, key);
		}

		public static string GetClipName(AnimationClip clip)
		{
			string result;
			if (clip == null)
			{
				result = "[No Clip]";
			}
			else
			{
				string text = clip.name;
				if ((clip.hideFlags & HideFlags.NotEditable) != HideFlags.None)
				{
					text += " (Read-Only)";
				}
				result = text;
			}
			return result;
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
