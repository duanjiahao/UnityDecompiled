using System;
using System.Collections.Generic;
using UnityEditor.Collaboration;
using UnityEditor.Connect;
using UnityEngine;

namespace UnityEditor
{
	internal class SearchUtility
	{
		private static void RemoveUnwantedWhitespaces(ref string searchString)
		{
			searchString = searchString.Replace(": ", ":");
		}

		internal static bool ParseSearchString(string searchText, SearchFilter filter)
		{
			if (string.IsNullOrEmpty(searchText))
			{
				return false;
			}
			filter.ClearSearch();
			string text = string.Copy(searchText);
			SearchUtility.RemoveUnwantedWhitespaces(ref text);
			bool result = false;
			int i = SearchUtility.FindFirstPositionNotOf(text, " \t,");
			if (i == -1)
			{
				i = 0;
			}
			while (i < text.Length)
			{
				int num = text.IndexOfAny(" \t,".ToCharArray(), i);
				int num2 = text.IndexOf('"', i);
				int num3 = -1;
				if (num2 != -1)
				{
					num3 = text.IndexOf('"', num2 + 1);
					if (num3 != -1)
					{
						num = text.IndexOfAny(" \t,".ToCharArray(), num3);
					}
					else
					{
						num = -1;
					}
				}
				if (num == -1)
				{
					num = text.Length;
				}
				if (num > i)
				{
					string text2 = text.Substring(i, num - i);
					if (SearchUtility.CheckForKeyWords(text2, filter, num2, num3))
					{
						result = true;
					}
					else
					{
						filter.nameFilter = filter.nameFilter + ((!string.IsNullOrEmpty(filter.nameFilter)) ? " " : string.Empty) + text2;
					}
				}
				i = num + 1;
			}
			return result;
		}

		internal static bool CheckForKeyWords(string searchString, SearchFilter filter, int quote1, int quote2)
		{
			bool result = false;
			int num = searchString.IndexOf("t:");
			if (num == 0)
			{
				string item = searchString.Substring(num + 2);
				filter.classNames = new List<string>(filter.classNames)
				{
					item
				}.ToArray();
				result = true;
			}
			num = searchString.IndexOf("l:");
			if (num == 0)
			{
				string item2 = searchString.Substring(num + 2);
				filter.assetLabels = new List<string>(filter.assetLabels)
				{
					item2
				}.ToArray();
				result = true;
			}
			if (UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted)
			{
				num = searchString.IndexOf("v:");
				if (num >= 0)
				{
					string item3 = searchString.Substring(num + 2);
					filter.versionControlStates = new List<string>(filter.versionControlStates)
					{
						item3
					}.ToArray();
					result = true;
				}
			}
			num = searchString.IndexOf("b:");
			if (num == 0)
			{
				string item4 = searchString.Substring(num + 2);
				filter.assetBundleNames = new List<string>(filter.assetBundleNames)
				{
					item4
				}.ToArray();
				result = true;
			}
			num = searchString.IndexOf("ref:");
			if (num == 0)
			{
				int num2 = 0;
				int num3 = num + 3;
				int num4 = searchString.IndexOf(':', num3 + 1);
				if (num4 >= 0)
				{
					string s = searchString.Substring(num3 + 1, num4 - num3 - 1);
					int num5;
					if (int.TryParse(s, out num5))
					{
						num2 = num5;
					}
				}
				else
				{
					string assetPath;
					if (quote1 != -1 && quote2 != -1)
					{
						int num6 = quote1 + 1;
						int num7 = quote2 - quote1 - 1;
						if (num7 < 0 || quote2 == -1)
						{
							num7 = searchString.Length - num6;
						}
						assetPath = "Assets/" + searchString.Substring(num6, num7);
					}
					else
					{
						assetPath = "Assets/" + searchString.Substring(num3 + 1);
					}
					UnityEngine.Object @object = AssetDatabase.LoadMainAssetAtPath(assetPath);
					if (@object != null)
					{
						num2 = @object.GetInstanceID();
					}
				}
				filter.referencingInstanceIDs = new int[]
				{
					num2
				};
				result = true;
			}
			return result;
		}

		private static int FindFirstPositionNotOf(string source, string chars)
		{
			if (source == null)
			{
				return -1;
			}
			if (chars == null)
			{
				return 0;
			}
			if (source.Length == 0)
			{
				return -1;
			}
			if (chars.Length == 0)
			{
				return 0;
			}
			for (int i = 0; i < source.Length; i++)
			{
				if (chars.IndexOf(source[i]) == -1)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
