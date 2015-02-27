using System;
using UnityEngine;
namespace TreeEditor
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class TreeAttribute : Attribute
	{
		public string uiLabel;
		public string uiGadget;
		public string uiCurve;
		public string uiRequirement;
		public GUIContent[] uiOptions;
		public float uiCurveMin;
		public float uiCurveMax;
		public float uiMin;
		public float uiMax;
		public TreeAttribute(string uiLabel, string uiGadget, float uiMin, float uiMax)
		{
			this.uiLabel = uiLabel;
			this.uiGadget = uiGadget;
			this.uiMin = uiMin;
			this.uiMax = uiMax;
			this.uiCurve = string.Empty;
			this.uiRequirement = string.Empty;
		}
		public TreeAttribute(string uiLabel, string uiGadget, float uiMin, float uiMax, string uiRequirement)
		{
			this.uiLabel = uiLabel;
			this.uiGadget = uiGadget;
			this.uiMin = uiMin;
			this.uiMax = uiMax;
			this.uiCurve = string.Empty;
			this.uiRequirement = uiRequirement;
		}
		public TreeAttribute(string uiLabel, string uiGadget, float uiMin, float uiMax, string uiCurve, float uiCurveMin, float uiCurveMax)
		{
			this.uiLabel = uiLabel;
			this.uiGadget = uiGadget;
			this.uiMin = uiMin;
			this.uiMax = uiMax;
			this.uiCurve = uiCurve;
			this.uiCurveMin = uiCurveMin;
			this.uiCurveMax = uiCurveMax;
			this.uiRequirement = string.Empty;
		}
		public TreeAttribute(string uiLabel, string uiGadget, float uiMin, float uiMax, string uiCurve, float uiCurveMin, float uiCurveMax, string uiRequirement)
		{
			this.uiLabel = uiLabel;
			this.uiGadget = uiGadget;
			this.uiMin = uiMin;
			this.uiMax = uiMax;
			this.uiCurve = uiCurve;
			this.uiCurveMin = uiCurveMin;
			this.uiCurveMax = uiCurveMax;
			this.uiRequirement = uiRequirement;
		}
		public TreeAttribute(string uiLabel, string uiGadget, string uiOptions)
		{
			char[] separator = new char[]
			{
				','
			};
			this.uiLabel = uiLabel;
			this.uiGadget = uiGadget;
			this.uiRequirement = uiOptions;
			string[] array = uiOptions.Split(separator);
			this.uiOptions = new GUIContent[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				this.uiOptions[i] = new GUIContent(array[i]);
			}
		}
		public TreeAttribute(string uiLabel, string uiGadget, string uiOptions, string uiCurve, float uiCurveMin, float uiCurveMax, string uiRequirement)
		{
			char[] separator = new char[]
			{
				','
			};
			this.uiLabel = uiLabel;
			this.uiGadget = uiGadget;
			this.uiRequirement = uiRequirement;
			this.uiCurve = uiCurve;
			this.uiCurveMin = uiCurveMin;
			this.uiCurveMax = uiCurveMax;
			string[] array = uiOptions.Split(separator);
			this.uiOptions = new GUIContent[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				this.uiOptions[i] = new GUIContent(array[i]);
			}
		}
		public override string ToString()
		{
			string text = string.Concat(new object[]
			{
				"uiLabel: ",
				this.uiLabel,
				", uiGadget: ",
				this.uiGadget,
				", uiMin: ",
				this.uiMin,
				", uiMax: ",
				this.uiMax
			});
			if (this.uiCurve != string.Empty)
			{
				text = text + ", uiCurve: " + this.uiCurve;
			}
			return text;
		}
	}
}
