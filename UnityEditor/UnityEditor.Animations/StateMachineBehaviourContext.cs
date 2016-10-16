using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEditor.Animations
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class StateMachineBehaviourContext
	{
		public AnimatorController animatorController;

		public UnityEngine.Object animatorObject;

		public int layerIndex;
	}
}
