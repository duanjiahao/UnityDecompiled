using System;
using System.Security.Cryptography;
using System.Text;

namespace UnityEditor.VisualStudioIntegration
{
	public static class SolutionGuidGenerator
	{
		public static string GuidForProject(string projectName)
		{
			return SolutionGuidGenerator.ComputeGuidHashFor(projectName + "salt");
		}

		public static string GuidForSolution(string projectName)
		{
			return SolutionGuidGenerator.ComputeGuidHashFor(projectName);
		}

		private static string ComputeGuidHashFor(string input)
		{
			byte[] bs = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
			return SolutionGuidGenerator.HashAsGuid(SolutionGuidGenerator.HashToString(bs));
		}

		private static string HashAsGuid(string hash)
		{
			string text = string.Concat(new string[]
			{
				hash.Substring(0, 8),
				"-",
				hash.Substring(8, 4),
				"-",
				hash.Substring(12, 4),
				"-",
				hash.Substring(16, 4),
				"-",
				hash.Substring(20, 12)
			});
			return text.ToUpper();
		}

		private static string HashToString(byte[] bs)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < bs.Length; i++)
			{
				byte b = bs[i];
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}
	}
}
