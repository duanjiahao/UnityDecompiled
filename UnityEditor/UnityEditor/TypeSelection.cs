using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TypeSelection : IComparable
	{
		public GUIContent label;

		public UnityEngine.Object[] objects;

		public TypeSelection(string typeName, UnityEngine.Object[] objects)
		{
			this.objects = objects;
			this.label = new GUIContent(string.Concat(new object[]
			{
				objects.Length,
				" ",
				ObjectNames.NicifyVariableName(typeName),
				(objects.Length <= 1) ? "" : "s"
			}));
			this.label.image = AssetPreview.GetMiniTypeThumbnail(objects[0]);
		}

		public int CompareTo(object o)
		{
			TypeSelection typeSelection = (TypeSelection)o;
			int result;
			if (typeSelection.objects.Length != this.objects.Length)
			{
				result = typeSelection.objects.Length.CompareTo(this.objects.Length);
			}
			else
			{
				result = this.label.text.CompareTo(typeSelection.label.text);
			}
			return result;
		}
	}
}
