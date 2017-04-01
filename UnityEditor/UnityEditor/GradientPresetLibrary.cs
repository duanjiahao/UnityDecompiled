using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class GradientPresetLibrary : PresetLibrary
	{
		[Serializable]
		private class GradientPreset
		{
			[SerializeField]
			private string m_Name;

			[SerializeField]
			private Gradient m_Gradient;

			public Gradient gradient
			{
				get
				{
					return this.m_Gradient;
				}
				set
				{
					this.m_Gradient = value;
				}
			}

			public string name
			{
				get
				{
					return this.m_Name;
				}
				set
				{
					this.m_Name = value;
				}
			}

			public GradientPreset(Gradient preset, string presetName)
			{
				this.gradient = preset;
				this.name = presetName;
			}
		}

		[SerializeField]
		private List<GradientPresetLibrary.GradientPreset> m_Presets = new List<GradientPresetLibrary.GradientPreset>();

		public override int Count()
		{
			return this.m_Presets.Count;
		}

		public override object GetPreset(int index)
		{
			return this.m_Presets[index].gradient;
		}

		public override void Add(object presetObject, string presetName)
		{
			Gradient gradient = presetObject as Gradient;
			if (gradient == null)
			{
				Debug.LogError("Wrong type used in GradientPresetLibrary");
			}
			else
			{
				Gradient gradient2 = new Gradient();
				gradient2.alphaKeys = gradient.alphaKeys;
				gradient2.colorKeys = gradient.colorKeys;
				gradient2.mode = gradient.mode;
				this.m_Presets.Add(new GradientPresetLibrary.GradientPreset(gradient2, presetName));
			}
		}

		public override void Replace(int index, object newPresetObject)
		{
			Gradient gradient = newPresetObject as Gradient;
			if (gradient == null)
			{
				Debug.LogError("Wrong type used in GradientPresetLibrary");
			}
			else
			{
				Gradient gradient2 = new Gradient();
				gradient2.alphaKeys = gradient.alphaKeys;
				gradient2.colorKeys = gradient.colorKeys;
				gradient2.mode = gradient.mode;
				this.m_Presets[index].gradient = gradient2;
			}
		}

		public override void Remove(int index)
		{
			this.m_Presets.RemoveAt(index);
		}

		public override void Move(int index, int destIndex, bool insertAfterDestIndex)
		{
			PresetLibraryHelpers.MoveListItem<GradientPresetLibrary.GradientPreset>(this.m_Presets, index, destIndex, insertAfterDestIndex);
		}

		public override void Draw(Rect rect, int index)
		{
			this.DrawInternal(rect, this.m_Presets[index].gradient);
		}

		public override void Draw(Rect rect, object presetObject)
		{
			this.DrawInternal(rect, presetObject as Gradient);
		}

		private void DrawInternal(Rect rect, Gradient gradient)
		{
			if (gradient != null)
			{
				GradientEditor.DrawGradientWithBackground(rect, gradient);
			}
		}

		public override string GetName(int index)
		{
			return this.m_Presets[index].name;
		}

		public override void SetName(int index, string presetName)
		{
			this.m_Presets[index].name = presetName;
		}

		public void DebugCreateTonsOfPresets()
		{
			int num = 150;
			string arg = "Preset_";
			for (int i = 0; i < num; i++)
			{
				List<GradientColorKey> list = new List<GradientColorKey>();
				int num2 = UnityEngine.Random.Range(3, 8);
				for (int j = 0; j < num2; j++)
				{
					list.Add(new GradientColorKey(new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value), UnityEngine.Random.value));
				}
				List<GradientAlphaKey> list2 = new List<GradientAlphaKey>();
				int num3 = UnityEngine.Random.Range(3, 8);
				for (int k = 0; k < num3; k++)
				{
					list2.Add(new GradientAlphaKey(UnityEngine.Random.value, UnityEngine.Random.value));
				}
				this.Add(new Gradient
				{
					colorKeys = list.ToArray(),
					alphaKeys = list2.ToArray()
				}, arg + (i + 1));
			}
		}
	}
}
