using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class TypeSelectionList
	{
		private List<TypeSelection> m_TypeSelections;

		public List<TypeSelection> typeSelections
		{
			get
			{
				return this.m_TypeSelections;
			}
		}

		public TypeSelectionList(UnityEngine.Object[] objects)
		{
			Dictionary<string, List<UnityEngine.Object>> dictionary = new Dictionary<string, List<UnityEngine.Object>>();
			for (int i = 0; i < objects.Length; i++)
			{
				UnityEngine.Object @object = objects[i];
				string typeName = ObjectNames.GetTypeName(@object);
				if (!dictionary.ContainsKey(typeName))
				{
					dictionary[typeName] = new List<UnityEngine.Object>();
				}
				dictionary[typeName].Add(@object);
			}
			this.m_TypeSelections = new List<TypeSelection>();
			foreach (KeyValuePair<string, List<UnityEngine.Object>> current in dictionary)
			{
				this.m_TypeSelections.Add(new TypeSelection(current.Key, current.Value.ToArray()));
			}
			this.m_TypeSelections.Sort();
		}
	}
}
