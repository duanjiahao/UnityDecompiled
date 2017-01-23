using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	internal struct DiagnosticSwitch
	{
		public string name;

		public string description;

		public DiagnosticSwitchFlags flags;

		public object value;

		public object minValue;

		public object maxValue;

		public object persistentValue;

		public EnumInfo enumInfo;

		[UsedByNativeCode]
		private static void AppendDiagnosticSwitchToList(List<DiagnosticSwitch> list, string name, string description, DiagnosticSwitchFlags flags, object value, object minValue, object maxValue, object persistentValue, EnumInfo enumInfo)
		{
			list.Add(new DiagnosticSwitch
			{
				name = name,
				description = description,
				flags = flags,
				value = value,
				minValue = minValue,
				maxValue = maxValue,
				persistentValue = persistentValue,
				enumInfo = enumInfo
			});
		}
	}
}
