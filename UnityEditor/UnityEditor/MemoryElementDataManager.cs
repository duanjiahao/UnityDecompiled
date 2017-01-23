using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

		[CompilerGenerated]
		private static Comparison<MemoryElement> <>f__mg$cache0;

		[CompilerGenerated]
		private static Comparison<MemoryElement> <>f__mg$cache1;

		[CompilerGenerated]
		private static Comparison<ObjectInfo> <>f__mg$cache2;

		[CompilerGenerated]
		private static Comparison<MemoryElement> <>f__mg$cache3;

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
			MemoryElementDataManager.ObjectTypeFilter result;
			switch (info.reason)
			{
			case 1:
				result = MemoryElementDataManager.ObjectTypeFilter.BuiltinResource;
				return result;
			case 2:
				result = MemoryElementDataManager.ObjectTypeFilter.DontSave;
				return result;
			case 3:
			case 8:
			case 9:
				result = MemoryElementDataManager.ObjectTypeFilter.Asset;
				return result;
			case 10:
				result = MemoryElementDataManager.ObjectTypeFilter.Other;
				return result;
			}
			result = MemoryElementDataManager.ObjectTypeFilter.Scene;
			return result;
		}

		private static bool HasValidNames(List<MemoryElement> memory)
		{
			bool result;
			for (int i = 0; i < memory.Count; i++)
			{
				if (!string.IsNullOrEmpty(memory[i].name))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
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
			List<MemoryElement> arg_98_0 = list;
			if (MemoryElementDataManager.<>f__mg$cache0 == null)
			{
				MemoryElementDataManager.<>f__mg$cache0 = new Comparison<MemoryElement>(MemoryElementDataManager.SortByMemorySize);
			}
			arg_98_0.Sort(MemoryElementDataManager.<>f__mg$cache0);
			foreach (MemoryElement current in list)
			{
				List<MemoryElement> arg_D9_0 = current.children;
				if (MemoryElementDataManager.<>f__mg$cache1 == null)
				{
					MemoryElementDataManager.<>f__mg$cache1 = new Comparison<MemoryElement>(MemoryElementDataManager.SortByMemorySize);
				}
				arg_D9_0.Sort(MemoryElementDataManager.<>f__mg$cache1);
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
			ObjectInfo[] arg_113_0 = array;
			if (MemoryElementDataManager.<>f__mg$cache2 == null)
			{
				MemoryElementDataManager.<>f__mg$cache2 = new Comparison<ObjectInfo>(MemoryElementDataManager.SortByMemoryClassName);
			}
			Array.Sort<ObjectInfo>(arg_113_0, MemoryElementDataManager.<>f__mg$cache2);
			memoryElement.AddChild(new MemoryElement("Scene Memory", MemoryElementDataManager.GenerateObjectTypeGroups(array, MemoryElementDataManager.ObjectTypeFilter.Scene)));
			memoryElement.AddChild(new MemoryElement("Assets", MemoryElementDataManager.GenerateObjectTypeGroups(array, MemoryElementDataManager.ObjectTypeFilter.Asset)));
			memoryElement.AddChild(new MemoryElement("Builtin Resources", MemoryElementDataManager.GenerateObjectTypeGroups(array, MemoryElementDataManager.ObjectTypeFilter.BuiltinResource)));
			memoryElement.AddChild(new MemoryElement("Not Saved", MemoryElementDataManager.GenerateObjectTypeGroups(array, MemoryElementDataManager.ObjectTypeFilter.DontSave)));
			memoryElement.AddChild(new MemoryElement("Other", MemoryElementDataManager.GenerateObjectTypeGroups(array, MemoryElementDataManager.ObjectTypeFilter.Other)));
			List<MemoryElement> arg_1B4_0 = memoryElement.children;
			if (MemoryElementDataManager.<>f__mg$cache3 == null)
			{
				MemoryElementDataManager.<>f__mg$cache3 = new Comparison<MemoryElement>(MemoryElementDataManager.SortByMemorySize);
			}
			arg_1B4_0.Sort(MemoryElementDataManager.<>f__mg$cache3);
			return memoryElement;
		}
	}
}
