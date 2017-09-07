using System;
using System.Collections.Generic;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal static class StyleComplexSelectorExtensions
	{
		private struct PseudoStateData
		{
			public readonly PseudoStates state;

			public readonly bool negate;

			public PseudoStateData(PseudoStates state, bool negate)
			{
				this.state = state;
				this.negate = negate;
			}
		}

		private static Dictionary<string, StyleComplexSelectorExtensions.PseudoStateData> s_PseudoStates;

		public static void CachePseudoStateMasks(this StyleComplexSelector complexSelector)
		{
			if (complexSelector.selectors[0].pseudoStateMask == -1)
			{
				if (StyleComplexSelectorExtensions.s_PseudoStates == null)
				{
					StyleComplexSelectorExtensions.s_PseudoStates = new Dictionary<string, StyleComplexSelectorExtensions.PseudoStateData>();
					StyleComplexSelectorExtensions.s_PseudoStates["active"] = new StyleComplexSelectorExtensions.PseudoStateData(PseudoStates.Active, false);
					StyleComplexSelectorExtensions.s_PseudoStates["hover"] = new StyleComplexSelectorExtensions.PseudoStateData(PseudoStates.Hover, false);
					StyleComplexSelectorExtensions.s_PseudoStates["checked"] = new StyleComplexSelectorExtensions.PseudoStateData(PseudoStates.Checked, false);
					StyleComplexSelectorExtensions.s_PseudoStates["selected"] = new StyleComplexSelectorExtensions.PseudoStateData(PseudoStates.Selected, false);
					StyleComplexSelectorExtensions.s_PseudoStates["disabled"] = new StyleComplexSelectorExtensions.PseudoStateData(PseudoStates.Disabled, false);
					StyleComplexSelectorExtensions.s_PseudoStates["focus"] = new StyleComplexSelectorExtensions.PseudoStateData(PseudoStates.Focus, false);
					StyleComplexSelectorExtensions.s_PseudoStates["inactive"] = new StyleComplexSelectorExtensions.PseudoStateData(PseudoStates.Active, true);
					StyleComplexSelectorExtensions.s_PseudoStates["enabled"] = new StyleComplexSelectorExtensions.PseudoStateData(PseudoStates.Disabled, true);
				}
				int i = 0;
				int num = complexSelector.selectors.Length;
				while (i < num)
				{
					StyleSelector styleSelector = complexSelector.selectors[i];
					StyleSelectorPart[] parts = styleSelector.parts;
					PseudoStates pseudoStates = (PseudoStates)0;
					PseudoStates pseudoStates2 = (PseudoStates)0;
					for (int j = 0; j < styleSelector.parts.Length; j++)
					{
						if (styleSelector.parts[j].type == StyleSelectorType.PseudoClass)
						{
							StyleComplexSelectorExtensions.PseudoStateData pseudoStateData;
							if (StyleComplexSelectorExtensions.s_PseudoStates.TryGetValue(parts[j].value, out pseudoStateData))
							{
								if (!pseudoStateData.negate)
								{
									pseudoStates |= pseudoStateData.state;
								}
								else
								{
									pseudoStates2 |= pseudoStateData.state;
								}
							}
							else
							{
								Debug.LogWarningFormat("Unknown pseudo class \"{0}\"", new object[]
								{
									parts[j].value
								});
							}
						}
					}
					styleSelector.pseudoStateMask = (int)pseudoStates;
					styleSelector.negatedPseudoStateMask = (int)pseudoStates2;
					i++;
				}
			}
		}
	}
}
