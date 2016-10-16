using System;
using UnityEngine;

namespace UnityEditor
{
	internal interface IDropArea
	{
		DropInfo DragOver(EditorWindow w, Vector2 screenPos);

		bool PerformDrop(EditorWindow w, DropInfo dropInfo, Vector2 screenPos);
	}
}
