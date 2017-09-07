using System;
using System.Collections.Generic;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal class StyleContext
	{
		private struct RuleRef
		{
			public StyleComplexSelector selector;

			public StyleSheet sheet;
		}

		private List<RuleMatcher> m_Matchers;

		private List<StyleContext.RuleRef> m_MatchedRules;

		private VisualContainer m_VisualTree;

		private static Dictionary<long, VisualElementStyles> s_StyleCache = new Dictionary<long, VisualElementStyles>();

		public float currentPixelsPerPoint
		{
			get;
			set;
		}

		public StyleContext(VisualContainer tree)
		{
			this.m_VisualTree = tree;
			this.m_Matchers = new List<RuleMatcher>(0);
			this.m_MatchedRules = new List<StyleContext.RuleRef>(0);
		}

		private void AddMatchersFromSheet(IEnumerable<StyleSheet> styleSheets)
		{
			foreach (StyleSheet current in styleSheets)
			{
				this.PushStyleSheet(current);
			}
		}

		internal void GetMatchersFor(VisualElement element, List<RuleMatcher> ruleMatchers, List<StyleSheet> stylesheets)
		{
			List<VisualElement> list = new List<VisualElement>();
			list.Add(element);
			VisualContainer visualContainer = element as VisualContainer;
			if (visualContainer == null)
			{
				visualContainer = element.parent;
			}
			while (visualContainer != null)
			{
				if (visualContainer.styleSheets != null)
				{
					stylesheets.AddRange(visualContainer.styleSheets);
					this.AddMatchersFromSheet(visualContainer.styleSheets);
				}
				list.Add(visualContainer);
				visualContainer = visualContainer.parent;
			}
			int num = 0;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				this.GetMatchersFor(list, i, num++, ruleMatchers);
			}
			this.m_Matchers.Clear();
		}

		private void GetMatchersFor(List<VisualElement> elements, int idx, int depth, List<RuleMatcher> ruleMatchers)
		{
			VisualElement element = elements[idx];
			int count = this.m_Matchers.Count;
			for (int i = 0; i < count; i++)
			{
				RuleMatcher item = this.m_Matchers[i];
				if (item.depth >= depth && StyleContext.Match(element, ref item))
				{
					StyleSelector[] selectors = item.complexSelector.selectors;
					int num = item.simpleSelectorIndex + 1;
					int num2 = selectors.Length;
					if (num < num2)
					{
						RuleMatcher item2 = new RuleMatcher
						{
							complexSelector = item.complexSelector,
							depth = ((selectors[num].previousRelationship != StyleSelectorRelationship.Child) ? 2147483647 : (depth + 1)),
							simpleSelectorIndex = num,
							sheet = item.sheet
						};
						this.m_Matchers.Add(item2);
					}
					else if (idx == 0)
					{
						ruleMatchers.Add(item);
					}
				}
			}
		}

		private void PushStyleSheet(StyleSheet styleSheetData)
		{
			StyleComplexSelector[] complexSelectors = styleSheetData.complexSelectors;
			int val = this.m_Matchers.Count + complexSelectors.Length;
			this.m_Matchers.Capacity = Math.Max(this.m_Matchers.Capacity, val);
			for (int i = 0; i < complexSelectors.Length; i++)
			{
				StyleComplexSelector complexSelector = complexSelectors[i];
				this.m_Matchers.Add(new RuleMatcher
				{
					sheet = styleSheetData,
					complexSelector = complexSelector,
					simpleSelectorIndex = 0,
					depth = 2147483647
				});
			}
		}

		public void DirtyStyleSheets()
		{
			StyleContext.PropagateDirtyStyleSheets(this.m_VisualTree);
		}

		private static void PropagateDirtyStyleSheets(VisualElement e)
		{
			VisualContainer visualContainer = e as VisualContainer;
			if (visualContainer != null)
			{
				if (visualContainer.styleSheets != null)
				{
					visualContainer.LoadStyleSheetsFromPaths();
				}
				foreach (VisualElement current in visualContainer)
				{
					StyleContext.PropagateDirtyStyleSheets(current);
				}
			}
		}

		public void ApplyStyles(VisualContainer subTree)
		{
			Debug.Assert(subTree.panel != null);
			this.UpdateStyles(subTree, 0);
			this.m_Matchers.Clear();
		}

		public void ApplyStyles()
		{
			this.ApplyStyles(this.m_VisualTree);
		}

		private void UpdateStyles(VisualElement element, int depth)
		{
			if (element.IsDirty(ChangeType.Styles) || element.IsDirty(ChangeType.StylesPath))
			{
				VisualContainer visualContainer = element as VisualContainer;
				int count = this.m_Matchers.Count;
				if (visualContainer != null && visualContainer.styleSheets != null)
				{
					this.AddMatchersFromSheet(visualContainer.styleSheets);
				}
				string fullTypeName = element.fullTypeName;
				long num = (long)fullTypeName.GetHashCode();
				num = (num * 397L ^ (long)this.currentPixelsPerPoint.GetHashCode());
				this.m_MatchedRules.Clear();
				int count2 = this.m_Matchers.Count;
				for (int i = 0; i < count2; i++)
				{
					RuleMatcher ruleMatcher = this.m_Matchers[i];
					if (ruleMatcher.depth >= depth && StyleContext.Match(element, ref ruleMatcher))
					{
						StyleSelector[] selectors = ruleMatcher.complexSelector.selectors;
						int num2 = ruleMatcher.simpleSelectorIndex + 1;
						int num3 = selectors.Length;
						if (num2 < num3)
						{
							RuleMatcher item = new RuleMatcher
							{
								complexSelector = ruleMatcher.complexSelector,
								depth = ((selectors[num2].previousRelationship != StyleSelectorRelationship.Child) ? 2147483647 : (depth + 1)),
								simpleSelectorIndex = num2,
								sheet = ruleMatcher.sheet
							};
							this.m_Matchers.Add(item);
						}
						else
						{
							StyleRule rule = ruleMatcher.complexSelector.rule;
							int specificity = ruleMatcher.complexSelector.specificity;
							num = (num * 397L ^ (long)rule.GetHashCode());
							num = (num * 397L ^ (long)specificity);
							this.m_MatchedRules.Add(new StyleContext.RuleRef
							{
								selector = ruleMatcher.complexSelector,
								sheet = ruleMatcher.sheet
							});
						}
					}
				}
				VisualElementStyles visualElementStyles;
				if (StyleContext.s_StyleCache.TryGetValue(num, out visualElementStyles))
				{
					element.SetSharedStyles(visualElementStyles);
				}
				else
				{
					visualElementStyles = new VisualElementStyles(true);
					int j = 0;
					int count3 = this.m_MatchedRules.Count;
					while (j < count3)
					{
						StyleContext.RuleRef ruleRef = this.m_MatchedRules[j];
						StylePropertyID[] propertyIDs = StyleSheetCache.GetPropertyIDs(ruleRef.sheet, ruleRef.selector.ruleIndex);
						visualElementStyles.ApplyRule(ruleRef.sheet, ruleRef.selector.specificity, ruleRef.selector.rule, propertyIDs, this.m_VisualTree.elementPanel.loadResourceFunc);
						j++;
					}
					StyleContext.s_StyleCache[num] = visualElementStyles;
					element.SetSharedStyles(visualElementStyles);
				}
				if (visualContainer != null)
				{
					for (int k = 0; k < visualContainer.childrenCount; k++)
					{
						VisualElement childAt = visualContainer.GetChildAt(k);
						this.UpdateStyles(childAt, depth + 1);
					}
				}
				if (this.m_Matchers.Count > count)
				{
					this.m_Matchers.RemoveRange(count, this.m_Matchers.Count - count);
				}
			}
		}

		private static bool Match(VisualElement element, ref RuleMatcher matcher)
		{
			bool flag = true;
			StyleSelector styleSelector = matcher.complexSelector.selectors[matcher.simpleSelectorIndex];
			int num = styleSelector.parts.Length;
			int num2 = 0;
			while (num2 < num && flag)
			{
				switch (styleSelector.parts[num2].type)
				{
				case StyleSelectorType.Wildcard:
					break;
				case StyleSelectorType.Type:
					flag = (element.typeName == styleSelector.parts[num2].value);
					break;
				case StyleSelectorType.Class:
					flag = element.ClassListContains(styleSelector.parts[num2].value);
					break;
				case StyleSelectorType.PseudoClass:
				{
					int pseudoStates = (int)element.pseudoStates;
					flag = ((styleSelector.pseudoStateMask & pseudoStates) == styleSelector.pseudoStateMask);
					flag &= ((styleSelector.negatedPseudoStateMask & ~pseudoStates) == styleSelector.negatedPseudoStateMask);
					break;
				}
				case StyleSelectorType.RecursivePseudoClass:
					goto IL_FA;
				case StyleSelectorType.ID:
					flag = (element.name == styleSelector.parts[num2].value);
					break;
				default:
					goto IL_FA;
				}
				IL_101:
				num2++;
				continue;
				IL_FA:
				flag = false;
				goto IL_101;
			}
			return flag;
		}
	}
}
