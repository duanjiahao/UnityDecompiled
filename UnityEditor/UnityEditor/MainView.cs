using System;
using UnityEngine;

namespace UnityEditor
{
	internal class MainView : View, ICleanuppable
	{
		private const float kStatusbarHeight = 20f;

		private static readonly Vector2 kMinSize = new Vector2(950f, 300f);

		private static readonly Vector2 kMaxSize = new Vector2(10000f, 10000f);

		private void OnEnable()
		{
			base.SetMinMaxSizes(MainView.kMinSize, MainView.kMaxSize);
		}

		protected override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);
			if (base.children.Length != 0)
			{
				Toolbar toolbar = (Toolbar)base.children[0];
				base.children[0].position = new Rect(0f, 0f, newPos.width, toolbar.CalcHeight());
				if (base.children.Length > 2)
				{
					base.children[1].position = new Rect(0f, toolbar.CalcHeight(), newPos.width, newPos.height - toolbar.CalcHeight() - base.children[2].position.height);
					base.children[2].position = new Rect(0f, newPos.height - base.children[2].position.height, newPos.width, base.children[2].position.height);
				}
			}
		}

		protected override void ChildrenMinMaxChanged()
		{
			if (base.children.Length == 3)
			{
				Toolbar toolbar = (Toolbar)base.children[0];
				Vector2 min = new Vector2(MainView.kMinSize.x, Mathf.Max(MainView.kMinSize.y, toolbar.CalcHeight() + 20f + base.children[1].minSize.y));
				base.SetMinMaxSizes(min, MainView.kMaxSize);
			}
			base.ChildrenMinMaxChanged();
		}

		public static void MakeMain()
		{
			ContainerWindow containerWindow = ScriptableObject.CreateInstance<ContainerWindow>();
			MainView mainView = ScriptableObject.CreateInstance<MainView>();
			mainView.SetMinMaxSizes(MainView.kMinSize, MainView.kMaxSize);
			containerWindow.rootView = mainView;
			Resolution currentResolution = Screen.currentResolution;
			int num = Mathf.Clamp(currentResolution.width * 3 / 4, 800, 1400);
			int num2 = Mathf.Clamp(currentResolution.height * 3 / 4, 600, 950);
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
