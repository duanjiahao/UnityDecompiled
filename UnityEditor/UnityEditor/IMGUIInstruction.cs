using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[RequiredByNativeCode]
	internal struct IMGUIInstruction
	{
		public InstructionType type;

		public int level;

		public Rect unclippedRect;

		public StackFrame[] stack;

		public int typeInstructionIndex;
	}
}
