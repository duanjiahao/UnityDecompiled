using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class CurveMenuManager
	{
		private CurveUpdater updater;

		public CurveMenuManager(CurveUpdater updater)
		{
			this.updater = updater;
		}

		public void AddTangentMenuItems(GenericMenu menu, List<KeyIdentifier> keyList)
		{
			bool flag = keyList.Count > 0;
			bool on = flag;
			bool on2 = flag;
			bool on3 = flag;
			bool on4 = flag;
			bool on5 = flag;
			bool flag2 = flag;
			bool flag3 = flag;
			bool flag4 = flag;
			bool flag5 = flag;
			bool flag6 = flag;
			bool flag7 = flag;
			foreach (KeyIdentifier current in keyList)
			{
				Keyframe keyframe = current.keyframe;
				AnimationUtility.TangentMode keyLeftTangentMode = AnimationUtility.GetKeyLeftTangentMode(keyframe);
				AnimationUtility.TangentMode keyRightTangentMode = AnimationUtility.GetKeyRightTangentMode(keyframe);
				bool keyBroken = AnimationUtility.GetKeyBroken(keyframe);
				if (keyLeftTangentMode != AnimationUtility.TangentMode.ClampedAuto || keyRightTangentMode != AnimationUtility.TangentMode.ClampedAuto)
				{
					on = false;
				}
				if (keyLeftTangentMode != AnimationUtility.TangentMode.Auto || keyRightTangentMode != AnimationUtility.TangentMode.Auto)
				{
					on2 = false;
				}
				if (keyBroken || keyLeftTangentMode != AnimationUtility.TangentMode.Free || keyRightTangentMode != AnimationUtility.TangentMode.Free)
				{
					on3 = false;
				}
				if (keyBroken || keyLeftTangentMode != AnimationUtility.TangentMode.Free || keyframe.inTangent != 0f || keyRightTangentMode != AnimationUtility.TangentMode.Free || keyframe.outTangent != 0f)
				{
					on4 = false;
				}
				if (!keyBroken)
				{
					on5 = false;
				}
				if (!keyBroken || keyLeftTangentMode != AnimationUtility.TangentMode.Free)
				{
					flag2 = false;
				}
				if (!keyBroken || keyLeftTangentMode != AnimationUtility.TangentMode.Linear)
				{
					flag3 = false;
				}
				if (!keyBroken || keyLeftTangentMode != AnimationUtility.TangentMode.Constant)
				{
					flag4 = false;
				}
				if (!keyBroken || keyRightTangentMode != AnimationUtility.TangentMode.Free)
				{
					flag5 = false;
				}
				if (!keyBroken || keyRightTangentMode != AnimationUtility.TangentMode.Linear)
				{
					flag6 = false;
				}
				if (!keyBroken || keyRightTangentMode != AnimationUtility.TangentMode.Constant)
				{
					flag7 = false;
				}
			}
			if (flag)
			{
				menu.AddItem(EditorGUIUtility.TextContent("Clamped Auto"), on, new GenericMenu.MenuFunction2(this.SetClampedAuto), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("Auto"), on2, new GenericMenu.MenuFunction2(this.SetAuto), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("Free Smooth"), on3, new GenericMenu.MenuFunction2(this.SetEditable), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("Flat"), on4, new GenericMenu.MenuFunction2(this.SetFlat), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("Broken"), on5, new GenericMenu.MenuFunction2(this.SetBroken), keyList);
				menu.AddSeparator("");
				menu.AddItem(EditorGUIUtility.TextContent("Left Tangent/Free"), flag2, new GenericMenu.MenuFunction2(this.SetLeftEditable), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("Left Tangent/Linear"), flag3, new GenericMenu.MenuFunction2(this.SetLeftLinear), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("Left Tangent/Constant"), flag4, new GenericMenu.MenuFunction2(this.SetLeftConstant), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("Right Tangent/Free"), flag5, new GenericMenu.MenuFunction2(this.SetRightEditable), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("Right Tangent/Linear"), flag6, new GenericMenu.MenuFunction2(this.SetRightLinear), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("Right Tangent/Constant"), flag7, new GenericMenu.MenuFunction2(this.SetRightConstant), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("Both Tangents/Free"), flag5 && flag2, new GenericMenu.MenuFunction2(this.SetBothEditable), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("Both Tangents/Linear"), flag6 && flag3, new GenericMenu.MenuFunction2(this.SetBothLinear), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("Both Tangents/Constant"), flag7 && flag4, new GenericMenu.MenuFunction2(this.SetBothConstant), keyList);
			}
			else
			{
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Clamped Auto"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Auto"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Free Smooth"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Flat"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Broken"));
				menu.AddSeparator("");
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Left Tangent/Free"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Left Tangent/Linear"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Left Tangent/Constant"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Right Tangent/Free"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Right Tangent/Linear"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Right Tangent/Constant"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Both Tangents/Free"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Both Tangents/Linear"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Both Tangents/Constant"));
			}
		}

		public void SetClampedAuto(object keysToSet)
		{
			this.SetBoth(AnimationUtility.TangentMode.ClampedAuto, (List<KeyIdentifier>)keysToSet);
		}

		public void SetAuto(object keysToSet)
		{
			this.SetBoth(AnimationUtility.TangentMode.Auto, (List<KeyIdentifier>)keysToSet);
		}

		public void SetEditable(object keysToSet)
		{
			this.SetBoth(AnimationUtility.TangentMode.Free, (List<KeyIdentifier>)keysToSet);
		}

		public void SetFlat(object keysToSet)
		{
			this.SetBoth(AnimationUtility.TangentMode.Free, (List<KeyIdentifier>)keysToSet);
			this.Flatten((List<KeyIdentifier>)keysToSet);
		}

		public void SetBoth(AnimationUtility.TangentMode mode, List<KeyIdentifier> keysToSet)
		{
			List<ChangedCurve> list = new List<ChangedCurve>();
			foreach (KeyIdentifier current in keysToSet)
			{
				AnimationCurve curve = current.curve;
				Keyframe keyframe = current.keyframe;
				AnimationUtility.SetKeyBroken(ref keyframe, false);
				AnimationUtility.SetKeyRightTangentMode(ref keyframe, mode);
				AnimationUtility.SetKeyLeftTangentMode(ref keyframe, mode);
				if (mode == AnimationUtility.TangentMode.Free)
				{
					float num = CurveUtility.CalculateSmoothTangent(keyframe);
					keyframe.inTangent = num;
					keyframe.outTangent = num;
				}
				curve.MoveKey(current.key, keyframe);
				AnimationUtility.UpdateTangentsFromModeSurrounding(curve, current.key);
				ChangedCurve item = new ChangedCurve(curve, current.curveId, current.binding);
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			this.updater.UpdateCurves(list, "Set Tangents");
		}

		public void Flatten(List<KeyIdentifier> keysToSet)
		{
			List<ChangedCurve> list = new List<ChangedCurve>();
			foreach (KeyIdentifier current in keysToSet)
			{
				AnimationCurve curve = current.curve;
				Keyframe keyframe = current.keyframe;
				keyframe.inTangent = 0f;
				keyframe.outTangent = 0f;
				curve.MoveKey(current.key, keyframe);
				AnimationUtility.UpdateTangentsFromModeSurrounding(curve, current.key);
				ChangedCurve item = new ChangedCurve(curve, current.curveId, current.binding);
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			this.updater.UpdateCurves(list, "Set Tangents");
		}

		public void SetBroken(object _keysToSet)
		{
			List<ChangedCurve> list = new List<ChangedCurve>();
			List<KeyIdentifier> list2 = (List<KeyIdentifier>)_keysToSet;
			foreach (KeyIdentifier current in list2)
			{
				AnimationCurve curve = current.curve;
				Keyframe keyframe = current.keyframe;
				AnimationUtility.SetKeyBroken(ref keyframe, true);
				if (AnimationUtility.GetKeyRightTangentMode(keyframe) == AnimationUtility.TangentMode.ClampedAuto || AnimationUtility.GetKeyRightTangentMode(keyframe) == AnimationUtility.TangentMode.Auto)
				{
					AnimationUtility.SetKeyRightTangentMode(ref keyframe, AnimationUtility.TangentMode.Free);
				}
				if (AnimationUtility.GetKeyLeftTangentMode(keyframe) == AnimationUtility.TangentMode.ClampedAuto || AnimationUtility.GetKeyLeftTangentMode(keyframe) == AnimationUtility.TangentMode.Auto)
				{
					AnimationUtility.SetKeyLeftTangentMode(ref keyframe, AnimationUtility.TangentMode.Free);
				}
				curve.MoveKey(current.key, keyframe);
				AnimationUtility.UpdateTangentsFromModeSurrounding(curve, current.key);
				ChangedCurve item = new ChangedCurve(curve, current.curveId, current.binding);
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			this.updater.UpdateCurves(list, "Set Tangents");
		}

		public void SetLeftEditable(object keysToSet)
		{
			this.SetTangent(0, AnimationUtility.TangentMode.Free, (List<KeyIdentifier>)keysToSet);
		}

		public void SetLeftLinear(object keysToSet)
		{
			this.SetTangent(0, AnimationUtility.TangentMode.Linear, (List<KeyIdentifier>)keysToSet);
		}

		public void SetLeftConstant(object keysToSet)
		{
			this.SetTangent(0, AnimationUtility.TangentMode.Constant, (List<KeyIdentifier>)keysToSet);
		}

		public void SetRightEditable(object keysToSet)
		{
			this.SetTangent(1, AnimationUtility.TangentMode.Free, (List<KeyIdentifier>)keysToSet);
		}

		public void SetRightLinear(object keysToSet)
		{
			this.SetTangent(1, AnimationUtility.TangentMode.Linear, (List<KeyIdentifier>)keysToSet);
		}

		public void SetRightConstant(object keysToSet)
		{
			this.SetTangent(1, AnimationUtility.TangentMode.Constant, (List<KeyIdentifier>)keysToSet);
		}

		public void SetBothEditable(object keysToSet)
		{
			this.SetTangent(2, AnimationUtility.TangentMode.Free, (List<KeyIdentifier>)keysToSet);
		}

		public void SetBothLinear(object keysToSet)
		{
			this.SetTangent(2, AnimationUtility.TangentMode.Linear, (List<KeyIdentifier>)keysToSet);
		}

		public void SetBothConstant(object keysToSet)
		{
			this.SetTangent(2, AnimationUtility.TangentMode.Constant, (List<KeyIdentifier>)keysToSet);
		}

		public void SetTangent(int leftRight, AnimationUtility.TangentMode mode, List<KeyIdentifier> keysToSet)
		{
			List<ChangedCurve> list = new List<ChangedCurve>();
			foreach (KeyIdentifier current in keysToSet)
			{
				AnimationCurve curve = current.curve;
				Keyframe keyframe = current.keyframe;
				AnimationUtility.SetKeyBroken(ref keyframe, true);
				if (leftRight == 2)
				{
					AnimationUtility.SetKeyLeftTangentMode(ref keyframe, mode);
					AnimationUtility.SetKeyRightTangentMode(ref keyframe, mode);
				}
				else if (leftRight == 0)
				{
					AnimationUtility.SetKeyLeftTangentMode(ref keyframe, mode);
					if (AnimationUtility.GetKeyRightTangentMode(keyframe) == AnimationUtility.TangentMode.ClampedAuto || AnimationUtility.GetKeyRightTangentMode(keyframe) == AnimationUtility.TangentMode.Auto)
					{
						AnimationUtility.SetKeyRightTangentMode(ref keyframe, AnimationUtility.TangentMode.Free);
					}
				}
				else
				{
					AnimationUtility.SetKeyRightTangentMode(ref keyframe, mode);
					if (AnimationUtility.GetKeyLeftTangentMode(keyframe) == AnimationUtility.TangentMode.ClampedAuto || AnimationUtility.GetKeyLeftTangentMode(keyframe) == AnimationUtility.TangentMode.Auto)
					{
						AnimationUtility.SetKeyLeftTangentMode(ref keyframe, AnimationUtility.TangentMode.Free);
					}
				}
				if (mode == AnimationUtility.TangentMode.Constant && (leftRight == 0 || leftRight == 2))
				{
					keyframe.inTangent = float.PositiveInfinity;
				}
				if (mode == AnimationUtility.TangentMode.Constant && (leftRight == 1 || leftRight == 2))
				{
					keyframe.outTangent = float.PositiveInfinity;
				}
				curve.MoveKey(current.key, keyframe);
				AnimationUtility.UpdateTangentsFromModeSurrounding(curve, current.key);
				ChangedCurve item = new ChangedCurve(curve, current.curveId, current.binding);
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			this.updater.UpdateCurves(list, "Set Tangents");
		}
	}
}
