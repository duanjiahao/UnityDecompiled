using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal class EnumInfo
	{
		public string[] names;

		public int[] values;

		public string[] annotations;

		public bool isFlags;

		public GUIContent[] guiNames;

		[UsedByNativeCode]
		internal static EnumInfo CreateEnumInfoFromNativeEnum(string[] names, int[] values, string[] annotations, bool isFlags)
		{
			EnumInfo enumInfo = new EnumInfo();
			enumInfo.names = names;
			enumInfo.values = values;
			enumInfo.annotations = annotations;
			enumInfo.isFlags = isFlags;
			enumInfo.guiNames = new GUIContent[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				enumInfo.guiNames[i] = new GUIContent(names[i], annotations[i]);
			}
			return enumInfo;
		}
	}
}
