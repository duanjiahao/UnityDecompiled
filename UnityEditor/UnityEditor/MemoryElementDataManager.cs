using System;
using System.Collections.Generic;
using UnityEditorInternal;

namespace UnityEditor
{
	internal class MemoryElementDataManager
	{
		private enum ObjectTypeFilter
		{
			Scene,
			Asset,
			BuiltinResource,
			DontSave,
			Other
		}

		private static int SortByMemoryClassName(ObjectInfo x, ObjectInfo y)
		{
			return y.className.CompareTo(x.className);
		}

		private static int SortByMemorySize(MemoryElement x, MemoryElement y)
		{
			return y.totalMemory.CompareTo(x.totalMemory);
		}

		private static MemoryElementDataManager.ObjectTypeFilter GetObjectTypeFilter(ObjectInfo info)
		{
			switch (info.reason)
			{
			case 1:
				return MemoryElementDataManager.ObjectTypeFilter.BuiltinResource;
			case 2:
				return MemoryElementDataManager.ObjectTypeFilter.DontSave;
			case 3:
			case 8:
			case 9:
				return MemoryElementDataManager.ObjectTypeFilter.Asset;
			case 10:
				return MemoryElementDataManager.ObjectTypeFilter.Other;
			}
			return MemoryElementDataManager.ObjectTypeFilter.Scene;
		}

		private static bool HasValidNames(List<MemoryElement> memory)
		{
			for (int i = 0; i < memory.Count; i++)
			{
				if (!string.IsNullOrEmpty(memory[i].name))
				{
					return true;
				}
			}
			return false;
		}

		private static List<MemoryElement> GenerateObjectTypeGroups(ObjectInfo[] memory, MemoryElementDataManager.ObjectTypeFilter filter)
		{
			List<MemoryElement> list = new List<MemoryElement>();
			MemoryElement memoryElement = null;
			for (int i = 0; i < memory.Length; i++)
			{
				ObjectInfo objectInfo = memory[i];
				if (MemoryElementDataManager.GetObjectTypeFilter(objectInfo) == filter)
				{
					if (memoryElement == null || objectInfo.className != memoryElement.name)
					{
						memoryElement = new MemoryElement(objectInfo.className);
						list.Add(memoryElement);
					}
					memoryElement.AddChild(new MemoryElement(objectInfo, true));
				}
			}
			list.Sort(new Comparison<MemoryElement>(MemoryElementDataManager.SortByMemorySize));
			foreach (MemoryElement current in list)
			{
				current.children.Sort(new Comparison<MemoryElement>(MemoryElementDataManager.SortByMemorySize));
				if (filter == MemoryElementDataManager.ObjectTypeFilter.Other && !MemoryElementDataManager.HasValidNames(current.children))
				{
					current.children.Clear();
				}
			}
			return list;
		}

		public static MemoryElement GetTreeRoot(ObjectMemoryInfo[] memoryObjectList, int[] referencesIndices)
		{
			ObjectInfo[] array = new ObjectInfo[memoryObjectList.Length];
			for (int i = 0; i < memoryObjectList.Length; i++)
			{
				array[i] = new ObjectInfo
				{
					instanceId = memoryObjectList[i].instanceId,
					memorySize = memoryObjectList[i].memorySize,
					reason = memoryObjectList[i].reason,
					name = memoryObjectList[i].name,
					className = memoryObjectList[i].className
				};
			}
			int num = 0;
			for (int j = 0; j < memoryObjectList.Length; j++)
			{
				for (int k = 0; k < memoryObjectList[j].count; k++)
				{
					int num2 = referencesIndices[k + num];
					if (array[num2].referencedBy == null)
					{
						array[num2].referencedBy = new List<ObjectInfo>();
					}
					array[num2].referencedBy.Add(array[j]);
				}
				num += memoryObjectList[j].count;
			}
			MemoryElement memoryElement = new MemoryElement();
			Array.Sort<ObjectInfo>(array, new Comparison<ObjectInfo>(MemoryElementDataManager.SortByMemoryClassName));
			memoryElement.AddChild(new MemoryElement("Scene Memory", MemoryElementDataManager.GenerateObjectTypeGroups(array, MemoryElementDataManager.ObjectTypeFilter.Scene)));
			memoryElement.AddChild(new MemoryElement("Assets", MemoryElementDataManager.GenerateObjectTypeGroups(array, MemoryElementDataManager.ObjectTypeFilter.Asset)));
			memoryElement.AddChild(new MemoryElement("Builtin Resources", MemoryElementDataManager.GenerateObjectTypeGroups(array, MemoryElementDataManager.ObjectTypeFilter.BuiltinResource)));
			memoryElement.AddChild(new MemoryElement("Not Saved", MemoryElementDataManager.GenerateObjectTypeGroups(array, MemoryElementDataManager.ObjectTypeFilter.DontSave)));
			memoryElement.AddChild(new MemoryElement("Other", MemoryElementDataManager.GenerateObjectTypeGroups(array, MemoryElementDataManager.ObjectTypeFilter.Other)));
			memoryElement.children.Sort(new Comparison<MemoryElement>(MemoryElementDataManager.SortByMemorySize));
			return memoryElement;
		}
	}
}
