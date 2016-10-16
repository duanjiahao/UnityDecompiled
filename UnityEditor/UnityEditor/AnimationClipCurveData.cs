using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEditor
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class AnimationClipCurveData
	{
		public string path;

		public Type type;

		public string propertyName;

		public AnimationCurve curve;

		internal int classID;

		internal int scriptInstanceID;

		public AnimationClipCurveData()
		{
		}

		public AnimationClipCurveData(EditorCurveBinding binding)
		{
			this.path = binding.path;
			this.type = binding.type;
			this.propertyName = binding.propertyName;
			this.curve = null;
			this.classID = binding.m_ClassID;
			this.scriptInstanceID = binding.m_ScriptInstanceID;
		}
	}
}
