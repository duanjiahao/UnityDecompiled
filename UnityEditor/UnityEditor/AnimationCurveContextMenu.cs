using System;
using UnityEngine;

namespace UnityEditor
{
	internal class AnimationCurveContextMenu
	{
		private SerializedProperty m_Prop1;

		private SerializedProperty m_Prop2;

		private SerializedProperty m_Scalar;

		private ParticleSystemCurveEditor m_ParticleSystemCurveEditor;

		private Rect m_CurveRanges;

		private AnimationCurveContextMenu(SerializedProperty prop1, SerializedProperty prop2, SerializedProperty scalar, Rect curveRanges, ParticleSystemCurveEditor owner)
		{
			this.m_Prop1 = prop1;
			this.m_Prop2 = prop2;
			this.m_Scalar = scalar;
			this.m_ParticleSystemCurveEditor = owner;
			this.m_CurveRanges = curveRanges;
		}

		internal static void Show(Rect position, SerializedProperty property, SerializedProperty property2, SerializedProperty scalar, Rect curveRanges, ParticleSystemCurveEditor curveEditor)
		{
			GUIContent content = new GUIContent("Copy");
			GUIContent content2 = new GUIContent("Paste");
			GenericMenu genericMenu = new GenericMenu();
			bool flag = property != null && property2 != null;
			bool flag2 = (flag && ParticleSystemClipboard.HasDoubleAnimationCurve()) || (!flag && ParticleSystemClipboard.HasSingleAnimationCurve());
			AnimationCurveContextMenu @object = new AnimationCurveContextMenu(property, property2, scalar, curveRanges, curveEditor);
			genericMenu.AddItem(content, false, new GenericMenu.MenuFunction(@object.Copy));
			if (flag2)
			{
				genericMenu.AddItem(content2, false, new GenericMenu.MenuFunction(@object.Paste));
			}
			else
			{
				genericMenu.AddDisabledItem(content2);
			}
			genericMenu.DropDown(position);
		}

		private void Copy()
		{
			AnimationCurve animCurve = (this.m_Prop1 == null) ? null : this.m_Prop1.animationCurveValue;
			AnimationCurve animCurve2 = (this.m_Prop2 == null) ? null : this.m_Prop2.animationCurveValue;
			float scalar = (this.m_Scalar == null) ? 1f : this.m_Scalar.floatValue;
			ParticleSystemClipboard.CopyAnimationCurves(animCurve, animCurve2, scalar);
		}

		private void Paste()
		{
			ParticleSystemClipboard.PasteAnimationCurves(this.m_Prop1, this.m_Prop2, this.m_Scalar, this.m_CurveRanges, this.m_ParticleSystemCurveEditor);
		}
	}
}
