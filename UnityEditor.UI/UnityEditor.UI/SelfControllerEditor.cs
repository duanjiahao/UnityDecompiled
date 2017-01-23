using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	public class SelfControllerEditor : Editor
	{
		private static string s_Warning = "Parent has a type of layout group component. A child of a layout group should not have a {0} component, since it should be driven by the layout group.";

		public override void OnInspectorGUI()
		{
			bool flag = false;
			for (int i = 0; i < base.targets.Length; i++)
			{
				Component component = base.targets[i] as Component;
				ILayoutIgnorer layoutIgnorer = component.GetComponent(typeof(ILayoutIgnorer)) as ILayoutIgnorer;
				if (layoutIgnorer == null || !layoutIgnorer.ignoreLayout)
				{
					RectTransform rectTransform = component.transform.parent as RectTransform;
					if (rectTransform != null)
					{
						Behaviour behaviour = rectTransform.GetComponent(typeof(ILayoutGroup)) as Behaviour;
						if (behaviour != null && behaviour.enabled)
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag)
			{
				EditorGUILayout.HelpBox(string.Format(SelfControllerEditor.s_Warning, ObjectNames.NicifyVariableName(base.target.GetType().Name)), MessageType.Warning);
			}
		}
	}
}
