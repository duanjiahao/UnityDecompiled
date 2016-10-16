using System;

namespace UnityEditor
{
	internal interface IFlexibleMenuItemProvider
	{
		int Count();

		object GetItem(int index);

		int Add(object obj);

		void Replace(int index, object newPresetObject);

		void Remove(int index);

		object Create();

		void Move(int index, int destIndex, bool insertAfterDestIndex);

		string GetName(int index);

		bool IsModificationAllowed(int index);

		int[] GetSeperatorIndices();
	}
}
