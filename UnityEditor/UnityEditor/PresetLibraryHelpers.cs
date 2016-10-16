using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal static class PresetLibraryHelpers
	{
		public static void MoveListItem<T>(List<T> list, int index, int destIndex, bool insertAfterDestIndex)
		{
			if (index < 0 || destIndex < 0)
			{
				Debug.LogError("Invalid preset move");
				return;
			}
			if (index == destIndex)
			{
				return;
			}
			if (destIndex > index)
			{
				destIndex--;
			}
			if (insertAfterDestIndex && destIndex < list.Count - 1)
			{
				destIndex++;
			}
			T item = list[index];
			list.RemoveAt(index);
			list.Insert(destIndex, item);
		}
	}
}
