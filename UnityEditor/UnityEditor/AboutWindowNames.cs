using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnityEditor
{
	internal static class AboutWindowNames
	{
		public class CreditEntry
		{
			public string name;

			public string normalizedName;

			public string country_code;

			public string office;

			public string region;

			public string twitter;

			public string nationality;

			public string gravatar_hash;

			public bool alumni;

			public string FormattedName
			{
				get
				{
					string text = this.name;
					if (!string.IsNullOrEmpty(this.twitter))
					{
						text = text + " ( @" + this.twitter + " )";
					}
					return text;
				}
			}
		}

		private const int kChunkSize = 100;

		private static string s_Country = null;

		private static string[] s_CachedNames = new string[0];

		public static List<AboutWindowNames.CreditEntry> s_Credits = new List<AboutWindowNames.CreditEntry>();

		private static string CreditsFilePath
		{
			get
			{
				return Path.Combine(EditorApplication.applicationContentsPath, "Resources/credits.csv");
			}
		}

		public static string RemoveDiacritics(string text)
		{
			string text2 = text.Normalize(NormalizationForm.FormD);
			StringBuilder stringBuilder = new StringBuilder();
			string text3 = text2;
			for (int i = 0; i < text3.Length; i++)
			{
				char c = text3[i];
				UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		}

		public static void ParseCredits()
		{
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("credits.csv"))
			{
				using (StreamReader streamReader = new StreamReader(manifestResourceStream))
				{
					string text;
					do
					{
						text = streamReader.ReadLine();
						if (text != null)
						{
							if (text.Length > 0)
							{
								string[] array = text.Split(new char[]
								{
									','
								});
								AboutWindowNames.CreditEntry item = new AboutWindowNames.CreditEntry
								{
									name = array[0],
									normalizedName = AboutWindowNames.RemoveDiacritics(array[0]),
									alumni = (array[1] == "1"),
									country_code = array[2],
									region = array[3],
									twitter = array[4]
								};
								AboutWindowNames.s_Credits.Add(item);
							}
						}
					}
					while (text != null);
				}
			}
		}

		public static string[] Names(string country_filter = null, bool chunked = false)
		{
			string[] result;
			if (AboutWindowNames.s_Country == country_filter && AboutWindowNames.s_CachedNames.Length > 0)
			{
				result = AboutWindowNames.s_CachedNames;
			}
			else
			{
				AboutWindowNames.s_Country = country_filter;
				List<string> list = new List<string>();
				foreach (AboutWindowNames.CreditEntry current in AboutWindowNames.s_Credits)
				{
					if (string.IsNullOrEmpty(country_filter) || current.country_code == country_filter)
					{
						list.Add(current.FormattedName);
					}
				}
				if (!chunked)
				{
					AboutWindowNames.s_CachedNames = list.ToArray();
				}
				else
				{
					string[] array = new string[list.Count / 100 + 1];
					int num = 0;
					while (num * 100 < list.Count)
					{
						array[num] = string.Join(", ", list.Skip(num * 100).Take(100).ToArray<string>());
						num++;
					}
					AboutWindowNames.s_CachedNames = array.ToArray<string>();
				}
				result = AboutWindowNames.s_CachedNames;
			}
			return result;
		}
	}
}
