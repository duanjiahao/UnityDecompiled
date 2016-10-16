using System;
using UnityEngine;

namespace UnityEditor
{
	internal class PropertyAndTargetHandler
	{
		public SerializedProperty property;

		public UnityEngine.Object target;

		public TargetChoiceHandler.TargetChoiceMenuFunction function;

		public PropertyAndTargetHandler(SerializedProperty property, UnityEngine.Object target, TargetChoiceHandler.TargetChoiceMenuFunction function)
		{
			this.property = property;
			this.target = target;
			this.function = function;
		}
	}
}
