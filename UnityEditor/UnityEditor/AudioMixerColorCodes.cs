using System;
using UnityEditor.Audio;
using UnityEngine;

namespace UnityEditor
{
	internal static class AudioMixerColorCodes
	{
		private struct ItemData
		{
			public AudioMixerGroupController[] groups;

			public int index;
		}

		private static Color[] darkSkinColors = new Color[]
		{
			new Color(0.5f, 0.5f, 0.5f, 0.2f),
			new Color(1f, 0.8156863f, 0f),
			new Color(0.9607843f, 0.6117647f, 0.0156862754f),
			new Color(1f, 0.294117659f, 0.227450982f),
			new Color(1f, 0.380392164f, 0.6117647f),
			new Color(0.65882355f, 0.447058827f, 0.7176471f),
			new Color(0.0509803928f, 0.6117647f, 0.8235294f),
			new Color(0f, 0.745098054f, 0.784313738f),
			new Color(0.5411765f, 0.7529412f, 0.003921569f)
		};

		private static Color[] lightSkinColors = new Color[]
		{
			new Color(0.5f, 0.5f, 0.5f, 0.2f),
			new Color(1f, 0.8392157f, 0.08627451f),
			new Color(0.968627453f, 0.5764706f, 0f),
			new Color(1f, 0.294117659f, 0.227450982f),
			new Color(1f, 0.380392164f, 0.6117647f),
			new Color(0.65882355f, 0.447058827f, 0.7176471f),
			new Color(0.0509803928f, 0.6117647f, 0.8235294f),
			new Color(0f, 0.709803939f, 0.7254902f),
			new Color(0.447058827f, 0.6627451f, 0.09411765f)
		};

		private static string[] colorNames = new string[]
		{
			"No Color",
			"Yellow",
			"Orange",
			"Red",
			"Magenta",
			"Violet",
			"Blue",
			"Cyan",
			"Green"
		};

		private static string[] GetColorNames()
		{
			return AudioMixerColorCodes.colorNames;
		}

		private static Color[] GetColors()
		{
			if (EditorGUIUtility.isProSkin)
			{
				return AudioMixerColorCodes.darkSkinColors;
			}
			return AudioMixerColorCodes.lightSkinColors;
		}

		public static void AddColorItemsToGenericMenu(GenericMenu menu, AudioMixerGroupController[] groups)
		{
			Color[] colors = AudioMixerColorCodes.GetColors();
			string[] array = AudioMixerColorCodes.GetColorNames();
			for (int i = 0; i < colors.Length; i++)
			{
				bool on = groups.Length == 1 && i == groups[0].userColorIndex;
				menu.AddItem(new GUIContent(array[i]), on, new GenericMenu.MenuFunction2(AudioMixerColorCodes.ItemCallback), new AudioMixerColorCodes.ItemData
				{
					groups = groups,
					index = i
				});
			}
		}

		private static void ItemCallback(object data)
		{
			AudioMixerColorCodes.ItemData itemData = (AudioMixerColorCodes.ItemData)data;
			Undo.RecordObjects(itemData.groups, "Change Group(s) Color");
			AudioMixerGroupController[] groups = itemData.groups;
			for (int i = 0; i < groups.Length; i++)
			{
				AudioMixerGroupController audioMixerGroupController = groups[i];
				audioMixerGroupController.userColorIndex = itemData.index;
			}
		}

		public static Color GetColor(int index)
		{
			Color[] colors = AudioMixerColorCodes.GetColors();
			if (index >= 0 && index < colors.Length)
			{
				return colors[index];
			}
			Debug.LogError("Invalid color code index: " + index);
			return Color.white;
		}
	}
}
