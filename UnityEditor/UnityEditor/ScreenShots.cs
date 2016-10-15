using System;
using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class ScreenShots
	{
		public static Color kToolbarBorderColor = new Color(0.54f, 0.54f, 0.54f, 1f);

		public static Color kWindowBorderColor = new Color(0.51f, 0.51f, 0.51f, 1f);

		public static bool s_TakeComponentScreenshot = false;

		[MenuItem("Window/Screenshot/Set Window Size %&l", false, 1000, true)]
		public static void SetMainWindowSize()
		{
			MainWindow mainWindow = Resources.FindObjectsOfTypeAll(typeof(MainWindow))[0] as MainWindow;
			mainWindow.window.position = new Rect(0f, 0f, 1024f, 768f);
		}

		[MenuItem("Window/Screenshot/Set Window Size Small", false, 1000, true)]
		public static void SetMainWindowSizeSmall()
		{
			MainWindow mainWindow = Resources.FindObjectsOfTypeAll(typeof(MainWindow))[0] as MainWindow;
			mainWindow.window.position = new Rect(0f, 0f, 762f, 600f);
		}

		[MenuItem("Window/Screenshot/Snap View %&j", false, 1000, true)]
		public static void Screenshot()
		{
			GUIView mouseOverView = ScreenShots.GetMouseOverView();
			if (mouseOverView != null)
			{
				string gUIViewName = ScreenShots.GetGUIViewName(mouseOverView);
				Rect screenPosition = mouseOverView.screenPosition;
				screenPosition.y -= 1f;
				screenPosition.height += 2f;
				ScreenShots.SaveScreenShot(screenPosition, gUIViewName);
			}
		}

		[MenuItem("Window/Screenshot/Snap View Toolbar", false, 1000, true)]
		public static void ScreenshotToolbar()
		{
			GUIView mouseOverView = ScreenShots.GetMouseOverView();
			if (mouseOverView != null)
			{
				string name = ScreenShots.GetGUIViewName(mouseOverView) + "Toolbar";
				Rect screenPosition = mouseOverView.screenPosition;
				screenPosition.y += 19f;
				screenPosition.height = 16f;
				screenPosition.width -= 2f;
				ScreenShots.SaveScreenShotWithBorder(screenPosition, ScreenShots.kToolbarBorderColor, name);
			}
		}

		[MenuItem("Window/Screenshot/Snap View Extended Right %&k", false, 1000, true)]
		public static void ScreenshotExtendedRight()
		{
			GUIView mouseOverView = ScreenShots.GetMouseOverView();
			if (mouseOverView != null)
			{
				string name = ScreenShots.GetGUIViewName(mouseOverView) + "Extended";
				MainWindow mainWindow = Resources.FindObjectsOfTypeAll(typeof(MainWindow))[0] as MainWindow;
				Rect screenPosition = mouseOverView.screenPosition;
				screenPosition.xMax = mainWindow.window.position.xMax;
				screenPosition.y -= 1f;
				screenPosition.height += 2f;
				ScreenShots.SaveScreenShot(screenPosition, name);
			}
		}

		[MenuItem("Window/Screenshot/Snap Component", false, 1000, true)]
		public static void ScreenShotComponent()
		{
			ScreenShots.s_TakeComponentScreenshot = true;
		}

		public static void ScreenShotComponent(Rect contentRect, UnityEngine.Object target)
		{
			ScreenShots.s_TakeComponentScreenshot = false;
			contentRect.yMax += 2f;
			contentRect.xMin += 1f;
			ScreenShots.SaveScreenShotWithBorder(contentRect, ScreenShots.kWindowBorderColor, target.GetType().Name + "Inspector");
		}

		[MenuItem("Window/Screenshot/Snap Game View Content", false, 1000, true)]
		public static void ScreenGameViewContent()
		{
			string uniquePathForName = ScreenShots.GetUniquePathForName("ContentExample");
			Application.CaptureScreenshot(uniquePathForName);
			Debug.Log(string.Format("Saved screenshot at {0}", uniquePathForName));
		}

		[MenuItem("Window/Screenshot/Toggle DeveloperBuild", false, 1000, true)]
		public static void ToggleFakeNonDeveloperBuild()
		{
			Unsupported.fakeNonDeveloperBuild = !Unsupported.fakeNonDeveloperBuild;
			InternalEditorUtility.RequestScriptReload();
			InternalEditorUtility.RepaintAllViews();
		}

		private static GUIView GetMouseOverView()
		{
			GUIView mouseOverView = GUIView.mouseOverView;
			if (mouseOverView == null)
			{
				EditorApplication.Beep();
				Debug.LogWarning("Could not take screenshot.");
			}
			return mouseOverView;
		}

		private static string GetGUIViewName(GUIView view)
		{
			HostView hostView = view as HostView;
			if (hostView != null)
			{
				return hostView.actualView.GetType().Name;
			}
			return "Window";
		}

		public static void SaveScreenShot(Rect r, string name)
		{
			ScreenShots.SaveScreenShot((int)r.width, (int)r.height, InternalEditorUtility.ReadScreenPixel(new Vector2(r.x, r.y), (int)r.width, (int)r.height), name);
		}

		public static string SaveScreenShotWithBorder(Rect r, Color borderColor, string name)
		{
			int num = (int)r.width;
			int num2 = (int)r.height;
			Color[] array = InternalEditorUtility.ReadScreenPixel(new Vector2(r.x, r.y), num, num2);
			Color[] array2 = new Color[(num + 2) * (num2 + 2)];
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					array2[i + 1 + (num + 2) * (j + 1)] = array[i + num * j];
				}
			}
			for (int k = 0; k < num + 2; k++)
			{
				array2[k] = borderColor;
				array2[k + (num + 2) * (num2 + 1)] = borderColor;
			}
			for (int l = 0; l < num2 + 2; l++)
			{
				array2[l * (num + 2)] = borderColor;
				array2[l * (num + 2) + (num + 1)] = borderColor;
			}
			return ScreenShots.SaveScreenShot((int)(r.width + 2f), (int)(r.height + 2f), array2, name);
		}

		private static string SaveScreenShot(int width, int height, Color[] pixels, string name)
		{
			Texture2D texture2D = new Texture2D(width, height);
			texture2D.SetPixels(pixels, 0);
			texture2D.Apply(true);
			byte[] bytes = texture2D.EncodeToPNG();
			UnityEngine.Object.DestroyImmediate(texture2D, true);
			string uniquePathForName = ScreenShots.GetUniquePathForName(name);
			File.WriteAllBytes(uniquePathForName, bytes);
			Debug.Log(string.Format("Saved screenshot at {0}", uniquePathForName));
			return uniquePathForName;
		}

		private static string GetUniquePathForName(string name)
		{
			string text = string.Format("{0}/../../{1}.png", Application.dataPath, name);
			int num = 0;
			while (File.Exists(text))
			{
				text = string.Format("{0}/../../{1}{2:000}.png", Application.dataPath, name, num);
				num++;
			}
			return text;
		}
	}
}
