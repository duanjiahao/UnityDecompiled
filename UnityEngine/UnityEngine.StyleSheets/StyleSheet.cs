using System;

namespace UnityEngine.StyleSheets
{
	[Serializable]
	internal class StyleSheet : ScriptableObject
	{
		[SerializeField]
		private StyleRule[] m_Rules;

		[SerializeField]
		private StyleComplexSelector[] m_ComplexSelectors;

		[SerializeField]
		internal float[] floats;

		[SerializeField]
		internal Color[] colors;

		[SerializeField]
		internal string[] strings;

		public StyleRule[] rules
		{
			get
			{
				return this.m_Rules;
			}
			internal set
			{
				this.m_Rules = value;
				this.SetupReferences();
			}
		}

		public StyleComplexSelector[] complexSelectors
		{
			get
			{
				return this.m_ComplexSelectors;
			}
			internal set
			{
				this.m_ComplexSelectors = value;
				this.SetupReferences();
			}
		}

		private static T CheckAccess<T>(T[] list, StyleValueType type, StyleValueHandle handle)
		{
			T result = default(T);
			if (handle.valueType != type)
			{
				Debug.LogErrorFormat("Trying to read value of type {0} while reading a value of type {1}", new object[]
				{
					type,
					handle.valueType
				});
			}
			else if (handle.valueIndex < 0 && handle.valueIndex >= list.Length)
			{
				Debug.LogError("Accessing invalid property");
			}
			else
			{
				result = list[handle.valueIndex];
			}
			return result;
		}

		private void OnEnable()
		{
			this.SetupReferences();
		}

		private void SetupReferences()
		{
			if (this.complexSelectors != null && this.rules != null)
			{
				for (int i = 0; i < this.complexSelectors.Length; i++)
				{
					StyleComplexSelector styleComplexSelector = this.complexSelectors[i];
					if (styleComplexSelector.ruleIndex < this.rules.Length)
					{
						styleComplexSelector.rule = this.rules[styleComplexSelector.ruleIndex];
					}
				}
			}
		}

		public StyleValueKeyword ReadKeyword(StyleValueHandle handle)
		{
			return (StyleValueKeyword)handle.valueIndex;
		}

		public float ReadFloat(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<float>(this.floats, StyleValueType.Float, handle);
		}

		public Color ReadColor(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<Color>(this.colors, StyleValueType.Color, handle);
		}

		public string ReadString(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<string>(this.strings, StyleValueType.String, handle);
		}

		public string ReadEnum(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<string>(this.strings, StyleValueType.Enum, handle);
		}

		public string ReadResourcePath(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<string>(this.strings, StyleValueType.ResourcePath, handle);
		}
	}
}
