using System;
using UnityEngine;

namespace UnityEditor
{
	internal interface ISpriteEditorModule
	{
		string moduleName
		{
			get;
		}

		void OnModuleActivate();

		void OnModuleDeactivate();

		void DoTextureGUI();

		void DrawToolbarGUI(Rect drawArea);

		void OnPostGUI();

		bool CanBeActivated();
	}
}
