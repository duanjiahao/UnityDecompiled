using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace UnityEditor.Utils
{
	internal static class Paths
	{
		public static string Combine(params string[] components)
		{
			if (components.Length < 1)
			{
				throw new ArgumentException("At least one component must be provided!");
			}
			string text = components[0];
			for (int i = 1; i < components.Length; i++)
			{
				text = Path.Combine(text, components[i]);
			}
			return text;
		}

		public static string[] Split(string path)
		{
			List<string> list = new List<string>(path.Split(new char[]
			{
				Path.DirectorySeparatorChar
			}));
			int i = 0;
			while (i < list.Count)
			{
				list[i] = list[i].Trim();
				if (list[i].Equals(""))
				{
					list.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
			return list.ToArray();
		}

		public static string GetFileOrFolderName(string path)
		{
			string result;
			if (File.Exists(path))
			{
				result = Path.GetFileName(path);
			}
			else
			{
				if (!Directory.Exists(path))
				{
					throw new ArgumentException("Target '" + path + "' does not exist.");
				}
				string[] array = Paths.Split(path);
				result = array[array.Length - 1];
			}
			return result;
		}

		public static string CreateTempDirectory()
		{
			string tempFileName = Path.GetTempFileName();
			File.Delete(tempFileName);
			Directory.CreateDirectory(tempFileName);
			return tempFileName;
		}

		public static string NormalizePath(this string path)
		{
			string result;
			if (Path.DirectorySeparatorChar == '\\')
			{
				result = path.Replace('/', Path.DirectorySeparatorChar);
			}
			else
			{
				result = path.Replace('\\', Path.DirectorySeparatorChar);
			}
			return result;
		}

		public static string ConvertSeparatorsToUnity(this string path)
		{
			return path.Replace('\\', '/');
		}

		public static string UnifyDirectorySeparator(string path)
		{
			return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
		}

		public static bool AreEqual(string pathA, string pathB, bool ignoreCase)
		{
			return (pathA == "" && pathB == "") || (!string.IsNullOrEmpty(pathA) && !string.IsNullOrEmpty(pathB) && string.Compare(Path.GetFullPath(pathA), Path.GetFullPath(pathB), (!ignoreCase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) == 0);
		}

		public static bool IsValidAssetPathWithErrorLogging(string assetPath, string requiredExtensionWithDot)
		{
			string message;
			bool result;
			if (!Paths.IsValidAssetPath(assetPath, requiredExtensionWithDot, out message))
			{
				Debug.LogError(message);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public static bool IsValidAssetPath(string assetPath)
		{
			return Paths.IsValidAssetPath(assetPath, null);
		}

		public static bool IsValidAssetPath(string assetPath, string requiredExtensionWithDot)
		{
			string text = null;
			return Paths.CheckIfAssetPathIsValid(assetPath, requiredExtensionWithDot, ref text);
		}

		public static bool IsValidAssetPath(string assetPath, string requiredExtensionWithDot, out string errorMsg)
		{
			errorMsg = string.Empty;
			return Paths.CheckIfAssetPathIsValid(assetPath, requiredExtensionWithDot, ref errorMsg);
		}

		private static bool CheckIfAssetPathIsValid(string assetPath, string requiredExtensionWithDot, ref string errorMsg)
		{
			bool result;
			try
			{
				if (string.IsNullOrEmpty(assetPath))
				{
					if (errorMsg != null)
					{
						Paths.SetFullErrorMessage("Asset path is empty", assetPath, ref errorMsg);
					}
					result = false;
					return result;
				}
				string fileName = Path.GetFileName(assetPath);
				if (fileName.StartsWith("."))
				{
					if (errorMsg != null)
					{
						Paths.SetFullErrorMessage("Do not prefix asset name with '.'", assetPath, ref errorMsg);
					}
					result = false;
					return result;
				}
				if (fileName.StartsWith(" "))
				{
					if (errorMsg != null)
					{
						Paths.SetFullErrorMessage("Do not prefix asset name with white space", assetPath, ref errorMsg);
					}
					result = false;
					return result;
				}
				if (!string.IsNullOrEmpty(requiredExtensionWithDot))
				{
					string extension = Path.GetExtension(assetPath);
					if (!string.Equals(extension, requiredExtensionWithDot, StringComparison.OrdinalIgnoreCase))
					{
						if (errorMsg != null)
						{
							Paths.SetFullErrorMessage(string.Format("Incorrect extension. Required extension is: '{0}'", requiredExtensionWithDot), assetPath, ref errorMsg);
						}
						result = false;
						return result;
					}
				}
			}
			catch (Exception ex)
			{
				if (errorMsg != null)
				{
					Paths.SetFullErrorMessage(ex.Message, assetPath, ref errorMsg);
				}
				result = false;
				return result;
			}
			result = true;
			return result;
		}

		private static void SetFullErrorMessage(string error, string assetPath, ref string errorMsg)
		{
			errorMsg = string.Format("Asset path error: '{0}' is not valid: {1}", Paths.ToLiteral(assetPath), error);
		}

		private static string ToLiteral(string input)
		{
			string result;
			if (string.IsNullOrEmpty(input))
			{
				result = string.Empty;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(input.Length + 2);
				for (int i = 0; i < input.Length; i++)
				{
					char c = input[i];
					switch (c)
					{
					case '\a':
						stringBuilder.Append("\\a");
						break;
					case '\b':
						stringBuilder.Append("\\b");
						break;
					case '\t':
						stringBuilder.Append("\\t");
						break;
					case '\n':
						stringBuilder.Append("\\n");
						break;
					case '\v':
						stringBuilder.Append("\\v");
						break;
					case '\f':
						stringBuilder.Append("\\f");
						break;
					case '\r':
						stringBuilder.Append("\\r");
						break;
					default:
						if (c != '\0')
						{
							if (c != '"')
							{
								if (c != '\'')
								{
									if (c != '\\')
									{
										if (c >= ' ' && c <= '~')
										{
											stringBuilder.Append(c);
										}
										else
										{
											stringBuilder.Append("\\u");
											StringBuilder arg_178_0 = stringBuilder;
											int num = (int)c;
											arg_178_0.Append(num.ToString("x4"));
										}
									}
									else
									{
										stringBuilder.Append("\\\\");
									}
								}
								else
								{
									stringBuilder.Append("\\'");
								}
							}
							else
							{
								stringBuilder.Append("\\\"");
							}
						}
						else
						{
							stringBuilder.Append("\\0");
						}
						break;
					}
				}
				result = stringBuilder.ToString();
			}
			return result;
		}
	}
}
