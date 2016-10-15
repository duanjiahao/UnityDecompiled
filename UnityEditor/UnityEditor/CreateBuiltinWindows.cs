using System;
using UnityEditor.Sprites;
using UnityEditor.VersionControl;

namespace UnityEditor
{
	internal class CreateBuiltinWindows
	{
		[MenuItem("Window/Scene %1", false, 2000)]
		private static void ShowSceneView()
		{
			EditorWindow.GetWindow<SceneView>();
		}

		[MenuItem("Window/Game %2", false, 2001)]
		private static void ShowGameView()
		{
			EditorWindow.GetWindow<GameView>();
		}

		[MenuItem("Window/Inspector %3", false, 2002)]
		private static void ShowInspector()
		{
			EditorWindow.GetWindow<InspectorWindow>();
		}

		[MenuItem("Window/Hierarchy %4", false, 2003)]
		private static void ShowNewHierarchy()
		{
			EditorWindow.GetWindow<SceneHierarchyWindow>();
		}

		[MenuItem("Window/Project %5", false, 2004)]
		private static void ShowProject()
		{
			EditorWindow.GetWindow<ProjectBrowser>();
		}

		[MenuItem("Window/Animation %6", false, 2006)]
		private static void ShowAnimationWindow()
		{
			EditorWindow.GetWindow<AnimationWindow>();
		}

		private static void ShowProfilerWindow()
		{
			EditorWindow.GetWindow<ProfilerWindow>();
		}

		[MenuItem("Window/Audio Mixer %8", false, 2008)]
		private static void ShowAudioMixer()
		{
			AudioMixerWindow.Create();
		}

		private static void ShowVersionControl()
		{
			if (EditorSettings.externalVersionControl == ExternalVersionControl.AssetServer)
			{
				ASEditorBackend.DoAS();
			}
			else
			{
				EditorWindow.GetWindow<WindowPending>();
			}
		}

		[MenuItem("Window/Sprite Packer", false, 2014)]
		private static void ShowSpritePackerWindow()
		{
			EditorWindow.GetWindow<PackerWindow>();
		}

		[MenuItem("Window/Console %#c", false, 2200)]
		private static void ShowConsole()
		{
			EditorWindow.GetWindow<ConsoleWindow>();
		}
	}
}
