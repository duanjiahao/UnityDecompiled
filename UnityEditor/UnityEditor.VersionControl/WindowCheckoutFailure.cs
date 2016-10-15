using System;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace UnityEditor.VersionControl
{
	internal class WindowCheckoutFailure : EditorWindow
	{
		private AssetList assetList = new AssetList();

		private ListControl checkoutSuccessList = new ListControl();

		private ListControl checkoutFailureList = new ListControl();

		public void OnEnable()
		{
			base.position = new Rect(100f, 100f, 700f, 230f);
			base.minSize = new Vector2(700f, 230f);
			this.checkoutSuccessList.ReadOnly = true;
			this.checkoutFailureList.ReadOnly = true;
		}

		public static void OpenIfCheckoutFailed(AssetList assets)
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(WindowCheckoutFailure));
			WindowCheckoutFailure x = (array.Length <= 0) ? null : (array[0] as WindowCheckoutFailure);
			bool flag = x != null;
			bool flag2 = flag;
			if (!flag2)
			{
				foreach (Asset current in assets)
				{
					if (!current.IsState(Asset.States.CheckedOutLocal))
					{
						flag2 = true;
						break;
					}
				}
			}
			if (flag2)
			{
				WindowCheckoutFailure.GetWindow().DoOpen(assets, flag);
			}
		}

		private static WindowCheckoutFailure GetWindow()
		{
			return EditorWindow.GetWindow<WindowCheckoutFailure>(true, "Version Control Check Out Failed");
		}

		private void DoOpen(AssetList assets, bool alreadyOpen)
		{
			if (alreadyOpen)
			{
				foreach (Asset current in assets)
				{
					bool flag = false;
					int count = this.assetList.Count;
					for (int i = 0; i < count; i++)
					{
						if (this.assetList[i].path == current.path)
						{
							flag = true;
							this.assetList[i] = current;
							break;
						}
					}
					if (!flag)
					{
						this.assetList.Add(current);
					}
				}
			}
			else
			{
				this.assetList.AddRange(assets);
			}
			this.RefreshList();
		}

		private void RefreshList()
		{
			this.checkoutSuccessList.Clear();
			this.checkoutFailureList.Clear();
			foreach (Asset current in this.assetList)
			{
				if (current.IsState(Asset.States.CheckedOutLocal))
				{
					this.checkoutSuccessList.Add(null, current.prettyPath, current);
				}
				else
				{
					this.checkoutFailureList.Add(null, current.prettyPath, current);
				}
			}
			this.checkoutSuccessList.Refresh();
			this.checkoutFailureList.Refresh();
			base.Repaint();
		}

		public void OnGUI()
		{
			float num = (base.position.height - 122f) / 2f;
			GUILayout.Label("Some files could not be checked out:", EditorStyles.boldLabel, new GUILayoutOption[0]);
			Rect screenRect = new Rect(6f, 40f, base.position.width - 12f, num);
			GUILayout.BeginArea(screenRect);
			GUILayout.Box(string.Empty, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true)
			});
			GUILayout.EndArea();
			this.checkoutFailureList.OnGUI(new Rect(screenRect.x + 2f, screenRect.y + 2f, screenRect.width - 4f, screenRect.height - 4f), true);
			GUILayout.Space(20f + num);
			GUILayout.Label("The following files were successfully checked out:", EditorStyles.boldLabel, new GUILayoutOption[0]);
			Rect screenRect2 = new Rect(6f, 40f + num + 40f, base.position.width - 12f, num);
			GUILayout.BeginArea(screenRect2);
			GUILayout.Box(string.Empty, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true)
			});
			GUILayout.EndArea();
			this.checkoutSuccessList.OnGUI(new Rect(screenRect2.x + 2f, screenRect2.y + 2f, screenRect2.width - 4f, screenRect2.height - 4f), true);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorUserSettings.showFailedCheckout = !GUILayout.Toggle(!EditorUserSettings.showFailedCheckout, "Don't show this window again.", new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			bool enabled = GUI.enabled;
			GUI.enabled = (this.checkoutFailureList.Size > 0);
			if (GUILayout.Button("Retry Check Out", new GUILayoutOption[0]))
			{
				Provider.Checkout(this.assetList, CheckoutMode.Exact);
			}
			GUI.enabled = (this.checkoutSuccessList.Size > 0);
			if (GUILayout.Button("Revert Unchanged", new GUILayoutOption[0]))
			{
				Provider.Revert(this.assetList, RevertMode.Unchanged).SetCompletionAction(CompletionAction.UpdatePendingWindow);
				Provider.Status(this.assetList);
				base.Close();
			}
			GUI.enabled = enabled;
			if (GUILayout.Button("OK", new GUILayoutOption[0]))
			{
				base.Close();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(12f);
		}
	}
}
