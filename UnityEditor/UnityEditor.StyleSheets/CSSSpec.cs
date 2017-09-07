using System;
using System.Text.RegularExpressions;
using UnityEngine.StyleSheets;

namespace UnityEditor.StyleSheets
{
	internal static class CSSSpec
	{
		private static readonly Regex rgx = new Regex("(?<id>#\\w+)|(?<class>\\.\\w+)|(?<pseudoclass>:[\\w-]+(\\((?<param>.+)\\))?)|(?<type>\\w+)|(?<wildcard>\\*)|\\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private const int typeSelectorWeight = 1;

		private const int classSelectorWeight = 10;

		private const int idSelectorWeight = 100;

		public static int GetSelectorSpecificity(string selector)
		{
			int result = 0;
			StyleSelectorPart[] parts;
			if (CSSSpec.ParseSelector(selector, out parts))
			{
				result = CSSSpec.GetSelectorSpecificity(parts);
			}
			return result;
		}

		public static int GetSelectorSpecificity(StyleSelectorPart[] parts)
		{
			int num = 1;
			for (int i = 0; i < parts.Length; i++)
			{
				switch (parts[i].type)
				{
				case StyleSelectorType.Type:
					num++;
					break;
				case StyleSelectorType.Class:
				case StyleSelectorType.PseudoClass:
					num += 10;
					break;
				case StyleSelectorType.RecursivePseudoClass:
					throw new ArgumentException("Recursive pseudo classes are not supported");
				case StyleSelectorType.ID:
					num += 100;
					break;
				}
			}
			return num;
		}

		public static bool ParseSelector(string selector, out StyleSelectorPart[] parts)
		{
			MatchCollection matchCollection = CSSSpec.rgx.Matches(selector);
			int count = matchCollection.Count;
			bool result;
			if (count < 1)
			{
				parts = null;
				result = false;
			}
			else
			{
				parts = new StyleSelectorPart[count];
				for (int i = 0; i < count; i++)
				{
					Match match = matchCollection[i];
					StyleSelectorType type = StyleSelectorType.Unknown;
					string value = string.Empty;
					if (!string.IsNullOrEmpty(match.Groups["wildcard"].Value))
					{
						value = "*";
						type = StyleSelectorType.Wildcard;
					}
					else if (!string.IsNullOrEmpty(match.Groups["id"].Value))
					{
						value = match.Groups["id"].Value.Substring(1);
						type = StyleSelectorType.ID;
					}
					else if (!string.IsNullOrEmpty(match.Groups["class"].Value))
					{
						value = match.Groups["class"].Value.Substring(1);
						type = StyleSelectorType.Class;
					}
					else if (!string.IsNullOrEmpty(match.Groups["pseudoclass"].Value))
					{
						string value2 = match.Groups["param"].Value;
						if (!string.IsNullOrEmpty(value2))
						{
							value = value2;
							type = StyleSelectorType.RecursivePseudoClass;
						}
						else
						{
							value = match.Groups["pseudoclass"].Value.Substring(1);
							type = StyleSelectorType.PseudoClass;
						}
					}
					else if (!string.IsNullOrEmpty(match.Groups["type"].Value))
					{
						value = match.Groups["type"].Value;
						type = StyleSelectorType.Type;
					}
					parts[i] = new StyleSelectorPart
					{
						type = type,
						value = value
					};
				}
				result = true;
			}
			return result;
		}
	}
}
