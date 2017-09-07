using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal class RetainedMode : AssetPostprocessor
	{
		[CompilerGenerated]
		private static Action<IMGUIContainer> <>f__mg$cache0;

		[CompilerGenerated]
		private static Action<IMGUIContainer> <>f__mg$cache1;

		static RetainedMode()
		{
			if (RetainedMode.<>f__mg$cache0 == null)
			{
				RetainedMode.<>f__mg$cache0 = new Action<IMGUIContainer>(RetainedMode.OnBeginContainer);
			}
			UIElementsUtility.s_BeginContainerCallback = RetainedMode.<>f__mg$cache0;
			if (RetainedMode.<>f__mg$cache1 == null)
			{
				RetainedMode.<>f__mg$cache1 = new Action<IMGUIContainer>(RetainedMode.OnEndContainer);
			}
			UIElementsUtility.s_EndContainerCallback = RetainedMode.<>f__mg$cache1;
		}

		private static void OnBeginContainer(IMGUIContainer c)
		{
			HandleUtility.BeginHandles();
		}

		private static void OnEndContainer(IMGUIContainer c)
		{
			HandleUtility.EndHandles();
		}

		[RequiredByNativeCode]
		private static void UpdateSchedulers()
		{
			Dictionary<int, Panel>.Enumerator panelsIterator = UIElementsUtility.GetPanelsIterator();
			while (panelsIterator.MoveNext())
			{
				KeyValuePair<int, Panel> current = panelsIterator.Current;
				Panel value = current.Value;
				if (value.contextType == ContextType.Editor)
				{
					IScheduler scheduler = value.scheduler;
					value.timerEventScheduler.UpdateScheduledEvents();
					DataWatchService dataWatchService = value.dataWatch as DataWatchService;
					dataWatchService.ProcessNotificationQueue();
					if (value.visualTree.IsDirty(ChangeType.Repaint))
					{
						GUIView gUIView = EditorUtility.InstanceIDToObject(value.instanceID) as GUIView;
						if (gUIView != null)
						{
							gUIView.Repaint();
						}
					}
				}
			}
		}

		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			for (int i = 0; i < importedAssets.Length; i++)
			{
				string text = importedAssets[i];
				if (text.EndsWith("uss"))
				{
					RetainedMode.FlagStyleSheetChange();
					break;
				}
			}
		}

		public static void FlagStyleSheetChange()
		{
			StyleSheetCache.ClearCaches();
			Dictionary<int, Panel>.Enumerator panelsIterator = UIElementsUtility.GetPanelsIterator();
			while (panelsIterator.MoveNext())
			{
				KeyValuePair<int, Panel> current = panelsIterator.Current;
				Panel value = current.Value;
				if (value.contextType == ContextType.Editor)
				{
					value.styleContext.DirtyStyleSheets();
					value.visualTree.Dirty(ChangeType.Styles);
					GUIView gUIView = EditorUtility.InstanceIDToObject(value.instanceID) as GUIView;
					if (gUIView != null)
					{
						gUIView.Repaint();
					}
				}
			}
		}
	}
}
