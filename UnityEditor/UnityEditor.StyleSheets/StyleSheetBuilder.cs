using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.StyleSheets;

namespace UnityEditor.StyleSheets
{
	internal class StyleSheetBuilder
	{
		public struct ComplexSelectorScope : IDisposable
		{
			private StyleSheetBuilder m_Builder;

			public ComplexSelectorScope(StyleSheetBuilder builder)
			{
				this.m_Builder = builder;
			}

			public void Dispose()
			{
				this.m_Builder.EndComplexSelector();
			}
		}

		private enum BuilderState
		{
			Init,
			Rule,
			ComplexSelector,
			Property
		}

		private StyleSheetBuilder.BuilderState m_BuilderState;

		private List<float> m_Floats = new List<float>();

		private List<Color> m_Colors = new List<Color>();

		private List<string> m_Strings = new List<string>();

		private List<StyleRule> m_Rules = new List<StyleRule>();

		private List<StyleComplexSelector> m_ComplexSelectors = new List<StyleComplexSelector>();

		private List<StyleProperty> m_CurrentProperties = new List<StyleProperty>();

		private List<StyleValueHandle> m_CurrentValues = new List<StyleValueHandle>();

		private StyleComplexSelector m_CurrentComplexSelector;

		private List<StyleSelector> m_CurrentSelectors = new List<StyleSelector>();

		private string m_CurrentPropertyName;

		private StyleRule m_CurrentRule;

		public void BeginRule(int ruleLine)
		{
			StyleSheetBuilder.Log("Beginning rule");
			this.m_BuilderState = StyleSheetBuilder.BuilderState.Rule;
			this.m_CurrentRule = new StyleRule
			{
				line = ruleLine
			};
		}

		public StyleSheetBuilder.ComplexSelectorScope BeginComplexSelector(int specificity)
		{
			StyleSheetBuilder.Log("Begin complex selector with specificity " + specificity);
			this.m_BuilderState = StyleSheetBuilder.BuilderState.ComplexSelector;
			this.m_CurrentComplexSelector = new StyleComplexSelector();
			this.m_CurrentComplexSelector.specificity = specificity;
			this.m_CurrentComplexSelector.ruleIndex = this.m_Rules.Count;
			return new StyleSheetBuilder.ComplexSelectorScope(this);
		}

		public void AddSimpleSelector(StyleSelectorPart[] parts, StyleSelectorRelationship previousRelationsip)
		{
			StyleSelector styleSelector = new StyleSelector();
			styleSelector.parts = parts;
			styleSelector.previousRelationship = previousRelationsip;
			StyleSheetBuilder.Log("Add simple selector " + styleSelector);
			this.m_CurrentSelectors.Add(styleSelector);
		}

		public void EndComplexSelector()
		{
			StyleSheetBuilder.Log("Ending complex selector");
			this.m_BuilderState = StyleSheetBuilder.BuilderState.Rule;
			if (this.m_CurrentSelectors.Count > 0)
			{
				this.m_CurrentComplexSelector.selectors = this.m_CurrentSelectors.ToArray();
				this.m_ComplexSelectors.Add(this.m_CurrentComplexSelector);
				this.m_CurrentSelectors.Clear();
			}
			this.m_CurrentComplexSelector = null;
		}

		public void BeginProperty(string name)
		{
			StyleSheetBuilder.Log("Begin property named " + name);
			this.m_BuilderState = StyleSheetBuilder.BuilderState.Property;
			this.m_CurrentPropertyName = name;
		}

		public void AddValue(float value)
		{
			this.RegisterValue<float>(this.m_Floats, StyleValueType.Float, value);
		}

		public void AddValue(StyleValueKeyword keyword)
		{
			this.m_CurrentValues.Add(new StyleValueHandle((int)keyword, StyleValueType.Keyword));
		}

		public void AddValue(string value, StyleValueType type)
		{
			this.RegisterValue<string>(this.m_Strings, type, value);
		}

		public void AddValue(Color value)
		{
			this.RegisterValue<Color>(this.m_Colors, StyleValueType.Color, value);
		}

		public void EndProperty()
		{
			StyleSheetBuilder.Log("Ending property");
			this.m_BuilderState = StyleSheetBuilder.BuilderState.Rule;
			StyleProperty styleProperty = new StyleProperty();
			styleProperty.name = this.m_CurrentPropertyName;
			styleProperty.values = this.m_CurrentValues.ToArray();
			this.m_CurrentProperties.Add(styleProperty);
			this.m_CurrentValues.Clear();
		}

		public void EndRule()
		{
			StyleSheetBuilder.Log("Ending rule");
			this.m_BuilderState = StyleSheetBuilder.BuilderState.Init;
			this.m_CurrentRule.properties = this.m_CurrentProperties.ToArray();
			this.m_Rules.Add(this.m_CurrentRule);
			this.m_CurrentRule = null;
			this.m_CurrentProperties.Clear();
		}

		public void BuildTo(StyleSheet writeTo)
		{
			writeTo.floats = this.m_Floats.ToArray();
			writeTo.colors = this.m_Colors.ToArray();
			writeTo.strings = this.m_Strings.ToArray();
			writeTo.rules = this.m_Rules.ToArray();
			writeTo.complexSelectors = this.m_ComplexSelectors.ToArray();
		}

		private void RegisterValue<T>(List<T> list, StyleValueType type, T value)
		{
			StyleSheetBuilder.Log(string.Concat(new object[]
			{
				"Add value of type ",
				type,
				" : ",
				value
			}));
			list.Add(value);
			this.m_CurrentValues.Add(new StyleValueHandle(list.Count - 1, type));
		}

		private static void Log(string msg)
		{
		}
	}
}
