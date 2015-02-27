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
			bool flag2 = flag;
			bool flag3 = flag;
			bool flag4 = flag;
			bool flag5 = flag;
			bool flag6 = flag;
			bool flag7 = flag;
			foreach (KeyIdentifier current in keyList)
			{
				Keyframe keyframe = current.keyframe;
				TangentMode keyTangentMode = CurveUtility.GetKeyTangentMode(keyframe, 0);
				TangentMode keyTangentMode2 = CurveUtility.GetKeyTangentMode(keyframe, 1);
				bool keyBroken = CurveUtility.GetKeyBroken(keyframe);
				if (keyTangentMode != TangentMode.Smooth || keyTangentMode2 != TangentMode.Smooth)
				{
					on = false;
				}
				if (keyBroken || keyTangentMode != TangentMode.Editable || keyTangentMode2 != TangentMode.Editable)
				{
					on2 = false;
				}
				if (keyBroken || keyTangentMode != TangentMode.Editable || keyframe.inTangent != 0f || keyTangentMode2 != TangentMode.Editable || keyframe.outTangent != 0f)
				{
					on3 = false;
				}
				if (!keyBroken)
				{
					on4 = false;
				}
				if (!keyBroken || keyTangentMode != TangentMode.Editable)
				{
					flag2 = false;
				}
				if (!keyBroken || keyTangentMode != TangentMode.Linear)
				{
					flag3 = false;
				}
				if (!keyBroken || keyTangentMode != TangentMode.Stepped)
				{
					flag4 = false;
				}
				if (!keyBroken || keyTangentMode2 != TangentMode.Editable)
				{
					flag5 = false;
				}
				if (!keyBroken || keyTangentMode2 != TangentMode.Linear)
				{
					flag6 = false;
				}
				if (!keyBroken || keyTangentMode2 != TangentMode.Stepped)
				{
					flag7 = false;
				}
			}
			if (flag)
			{
				menu.AddItem(EditorGUIUtility.TextContent("CurveKeyPopupAuto"), on, new GenericMenu.MenuFunction2(this.SetSmooth), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("CurveKeyPopupFreeSmooth"), on2, new GenericMenu.MenuFunction2(this.SetEditable), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("CurveKeyPopupFlat"), on3, new GenericMenu.MenuFunction2(this.SetFlat), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("CurveKeyPopupBroken"), on4, new GenericMenu.MenuFunction2(this.SetBroken), keyList);
				menu.AddSeparator(string.Empty);
				menu.AddItem(EditorGUIUtility.TextContent("CurveKeyPopupLeftTangent/Free"), flag2, new GenericMenu.MenuFunction2(this.SetLeftEditable), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("CurveKeyPopupLeftTangent/Linear"), flag3, new GenericMenu.MenuFunction2(this.SetLeftLinear), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("CurveKeyPopupLeftTangent/Constant"), flag4, new GenericMenu.MenuFunction2(this.SetLeftConstant), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("CurveKeyPopupRightTangent/Free"), flag5, new GenericMenu.MenuFunction2(this.SetRightEditable), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("CurveKeyPopupRightTangent/Linear"), flag6, new GenericMenu.MenuFunction2(this.SetRightLinear), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("CurveKeyPopupRightTangent/Constant"), flag7, new GenericMenu.MenuFunction2(this.SetRightConstant), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("CurveKeyPopupBothTangents/Free"), flag5 && flag2, new GenericMenu.MenuFunction2(this.SetBothEditable), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("CurveKeyPopupBothTangents/Linear"), flag6 && flag3, new GenericMenu.MenuFunction2(this.SetBothLinear), keyList);
				menu.AddItem(EditorGUIUtility.TextContent("CurveKeyPopupBothTangents/Constant"), flag7 && flag4, new GenericMenu.MenuFunction2(this.SetBothConstant), keyList);
			}
			else
			{
				menu.AddDisabledItem(EditorGUIUtility.TextContent("CurveKeyPopupAuto"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("CurveKeyPopupFreeSmooth"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("CurveKeyPopupFlat"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("CurveKeyPopupBroken"));
				menu.AddSeparator(string.Empty);
				menu.AddDisabledItem(EditorGUIUtility.TextContent("CurveKeyPopupLeftTangent/Free"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("CurveKeyPopupLeftTangent/Linear"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("CurveKeyPopupLeftTangent/Constant"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("CurveKeyPopupRightTangent/Free"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("CurveKeyPopupRightTangent/Linear"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("CurveKeyPopupRightTangent/Constant"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("CurveKeyPopupBothTangents/Free"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("CurveKeyPopupBothTangents/Linear"));
				menu.AddDisabledItem(EditorGUIUtility.TextContent("CurveKeyPopupBothTangents/Constant"));
			}
		}
		public void SetSmooth(object keysToSet)
		{
			this.SetBoth(TangentMode.Smooth, (List<KeyIdentifier>)keysToSet);
		}
		public void SetEditable(object keysToSet)
		{
			this.SetBoth(TangentMode.Editable, (List<KeyIdentifier>)keysToSet);
		}
		public void SetFlat(object keysToSet)
		{
			this.SetBoth(TangentMode.Editable, (List<KeyIdentifier>)keysToSet);
			this.Flatten((List<KeyIdentifier>)keysToSet);
		}
		public void SetBoth(TangentMode mode, List<KeyIdentifier> keysToSet)
		{
			List<int> list = new List<int>();
			foreach (KeyIdentifier current in keysToSet)
			{
				AnimationCurve curve = current.curve;
				Keyframe keyframe = current.keyframe;
				CurveUtility.SetKeyBroken(ref keyframe, false);
				CurveUtility.SetKeyTangentMode(ref keyframe, 1, mode);
				CurveUtility.SetKeyTangentMode(ref keyframe, 0, mode);
				if (mode == TangentMode.Editable)
				{
					float num = CurveUtility.CalculateSmoothTangent(keyframe);
					keyframe.inTangent = num;
					keyframe.outTangent = num;
				}
				curve.MoveKey(current.key, keyframe);
				CurveUtility.UpdateTangentsFromModeSurrounding(curve, current.key);
				list.Add(current.curveId);
			}
			this.updater.UpdateCurves(list, "Set Tangents");
		}
		public void Flatten(List<KeyIdentifier> keysToSet)
		{
			List<int> list = new List<int>();
			foreach (KeyIdentifier current in keysToSet)
			{
				AnimationCurve curve = current.curve;
				Keyframe keyframe = current.keyframe;
				keyframe.inTangent = 0f;
				keyframe.outTangent = 0f;
				curve.MoveKey(current.key, keyframe);
				CurveUtility.UpdateTangentsFromModeSurrounding(curve, current.key);
				list.Add(current.curveId);
			}
			this.updater.UpdateCurves(list, "Set Tangents");
		}
		public void SetBroken(object _keysToSet)
		{
			List<KeyIdentifier> list = (List<KeyIdentifier>)_keysToSet;
			List<int> list2 = new List<int>();
			foreach (KeyIdentifier current in list)
			{
				AnimationCurve curve = current.curve;
				Keyframe keyframe = current.keyframe;
				CurveUtility.SetKeyBroken(ref keyframe, true);
				if (CurveUtility.GetKeyTangentMode(keyframe, 1) == TangentMode.Smooth)
				{
					CurveUtility.SetKeyTangentMode(ref keyframe, 1, TangentMode.Editable);
				}
				if (CurveUtility.GetKeyTangentMode(keyframe, 0) == TangentMode.Smooth)
				{
					CurveUtility.SetKeyTangentMode(ref keyframe, 0, TangentMode.Editable);
				}
				curve.MoveKey(current.key, keyframe);
				CurveUtility.UpdateTangentsFromModeSurrounding(curve, current.key);
				list2.Add(current.curveId);
			}
			this.updater.UpdateCurves(list2, "Set Tangents");
		}
		public void SetLeftEditable(object keysToSet)
		{
			this.SetTangent(0, TangentMode.Editable, (List<KeyIdentifier>)keysToSet);
		}
		public void SetLeftLinear(object keysToSet)
		{
			this.SetTangent(0, TangentMode.Linear, (List<KeyIdentifier>)keysToSet);
		}
		public void SetLeftConstant(object keysToSet)
		{
			this.SetTangent(0, TangentMode.Stepped, (List<KeyIdentifier>)keysToSet);
		}
		public void SetRightEditable(object keysToSet)
		{
			this.SetTangent(1, TangentMode.Editable, (List<KeyIdentifier>)keysToSet);
		}
		public void SetRightLinear(object keysToSet)
		{
			this.SetTangent(1, TangentMode.Linear, (List<KeyIdentifier>)keysToSet);
		}
		public void SetRightConstant(object keysToSet)
		{
			this.SetTangent(1, TangentMode.Stepped, (List<KeyIdentifier>)keysToSet);
		}
		public void SetBothEditable(object keysToSet)
		{
			this.SetTangent(2, TangentMode.Editable, (List<KeyIdentifier>)keysToSet);
		}
		public void SetBothLinear(object keysToSet)
		{
			this.SetTangent(2, TangentMode.Linear, (List<KeyIdentifier>)keysToSet);
		}
		public void SetBothConstant(object keysToSet)
		{
			this.SetTangent(2, TangentMode.Stepped, (List<KeyIdentifier>)keysToSet);
		}
		public void SetTangent(int leftRight, TangentMode mode, List<KeyIdentifier> keysToSet)
		{
			List<int> list = new List<int>();
			foreach (KeyIdentifier current in keysToSet)
			{
				AnimationCurve curve = current.curve;
				Keyframe keyframe = current.keyframe;
				CurveUtility.SetKeyBroken(ref keyframe, true);
				if (leftRight == 2)
				{
					CurveUtility.SetKeyTangentMode(ref keyframe, 0, mode);
					CurveUtility.SetKeyTangentMode(ref keyframe, 1, mode);
				}
				else
				{
					CurveUtility.SetKeyTangentMode(ref keyframe, leftRight, mode);
					if (CurveUtility.GetKeyTangentMode(keyframe, 1 - leftRight) == TangentMode.Smooth)
					{
						CurveUtility.SetKeyTangentMode(ref keyframe, 1 - leftRight, TangentMode.Editable);
					}
				}
				if (mode == TangentMode.Stepped && (leftRight == 0 || leftRight == 2))
				{
					keyframe.inTangent = float.PositiveInfinity;
				}
				if (mode == TangentMode.Stepped && (leftRight == 1 || leftRight == 2))
				{
					keyframe.outTangent = float.PositiveInfinity;
				}
				curve.MoveKey(current.key, keyframe);
				CurveUtility.UpdateTangentsFromModeSurrounding(curve, current.key);
				list.Add(current.curveId);
			}
			this.updater.UpdateCurves(list, "Set Tangents");
		}
	}
}
