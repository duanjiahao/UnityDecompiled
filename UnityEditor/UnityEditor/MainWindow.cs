using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class MainWindow : View, ICleanuppable
	{
		private const float kStatusbarHeight = 20f;

		private static readonly Vector2 kMinSize = new Vector2(950f, 300f);

		private static readonly Vector2 kMaxSize = new Vector2(10000f, 10000f);

		private void OnEnable()
		{
			base.SetMinMaxSizes(MainWindow.kMinSize, MainWindow.kMaxSize);
		}

		protected override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);
			if (base.children.Length == 0)
			{
				return;
			}
			Toolbar toolbar = (Toolbar)base.children[0];
			base.children[0].position = new Rect(0f, 0f, newPos.width, toolbar.CalcHeight());
			if (base.children.Length > 2)
			{
				base.children[1].position = new Rect(0f, toolbar.CalcHeight(), newPos.width, newPos.height - toolbar.CalcHeight() - base.children[2].position.height);
				base.children[2].position = new Rect(0f, newPos.height - base.children[2].position.height, newPos.width, base.children[2].position.height);
			}
		}

		protected override void ChildrenMinMaxChanged()
		{
			if (base.children.Length == 3)
			{
				Toolbar toolbar = (Toolbar)base.children[0];
				Vector2 min = new Vector2(MainWindow.kMinSize.x, Mathf.Max(MainWindow.kMinSize.y, toolbar.CalcHeight() + 20f + base.children[1].minSize.y));
				base.SetMinMaxSizes(min, MainWindow.kMaxSize);
			}
			base.ChildrenMinMaxChanged();
		}

		public static void MakeMain()
		{
			ContainerWindow containerWindow = ScriptableObject.CreateInstance<ContainerWindow>();
			MainWindow mainWindow = ScriptableObject.CreateInstance<MainWindow>();
			mainWindow.SetMinMaxSizes(MainWindow.kMinSize, MainWindow.kMaxSize);
			containerWindow.mainView = mainWindow;
			Resolution desktopResolution = InternalEditorUtility.GetDesktopResolution();
			int num = Mathf.Clamp(desktopResolution.width * 3 / 4, 800, 1400);
			int num2 = Mathf.Clamp(desktopResolution.height * 3 / 4, 600, 950);
			containerWindow.position = new Rect(60f, 20f, (float)num, (float)num2);
			containerWindow.Show(ShowMode.MainWindow, true, true);
			containerWindow.DisplayAllViews();
		}

		public void Cleanup()
		{
			if (base.children[1].children.Length == 0)
			{
				Rect position = base.window.position;
				Toolbar toolbar = (Toolbar)base.children[0];
				position.height = toolbar.CalcHeight() + 20f;
			}
		}
	}
}
