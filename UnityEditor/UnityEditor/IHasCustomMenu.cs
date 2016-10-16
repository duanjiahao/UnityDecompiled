using System;

namespace UnityEditor
{
	public interface IHasCustomMenu
	{
		void AddItemsToMenu(GenericMenu menu);
	}
}
