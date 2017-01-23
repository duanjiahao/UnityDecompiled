using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UnityEditorInternal
{
	internal static class FileMirroring
	{
		private enum FileEntryType
		{
			File,
			Directory,
			NotExisting
		}

		[CompilerGenerated]
		private static Func<string, string, bool> <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<string, string, bool> <>f__mg$cache1;

		public static void MirrorFile(string from, string to)
		{
			if (FileMirroring.<>f__mg$cache0 == null)
			{
				FileMirroring.<>f__mg$cache0 = new Func<string, string, bool>(FileMirroring.CanSkipCopy);
			}
			FileMirroring.MirrorFile(from, to, FileMirroring.<>f__mg$cache0);
		}

		public static void MirrorFile(string from, string to, Func<string, string, bool> comparer)
		{
			if (!comparer(from, to))
			{
				if (!File.Exists(from))
				{
					FileMirroring.DeleteFileOrDirectory(to);
				}
				else
				{
					string directoryName = Path.GetDirectoryName(to);
					if (!Directory.Exists(directoryName))
					{
						Directory.CreateDirectory(directoryName);
					}
					File.Copy(from, to, true);
				}
			}
		}

		public static void MirrorFolder(string from, string to)
		{
			if (FileMirroring.<>f__mg$cache1 == null)
			{
				FileMirroring.<>f__mg$cache1 = new Func<string, string, bool>(FileMirroring.CanSkipCopy);
			}
			FileMirroring.MirrorFolder(from, to, FileMirroring.<>f__mg$cache1);
		}

		public static void MirrorFolder(string from, string to, Func<string, string, bool> comparer)
		{
			from = Path.GetFullPath(from);
			to = Path.GetFullPath(to);
			if (!Directory.Exists(from))
			{
				if (Directory.Exists(to))
				{
					Directory.Delete(to, true);
				}
			}
			else
			{
				if (!Directory.Exists(to))
				{
					Directory.CreateDirectory(to);
				}
				IEnumerable<string> first = from s in Directory.GetFileSystemEntries(to)
				select FileMirroring.StripPrefix(s, to);
				IEnumerable<string> enumerable = from s in Directory.GetFileSystemEntries(@from)
				select FileMirroring.StripPrefix(s, @from);
				IEnumerable<string> enumerable2 = first.Except(enumerable);
				foreach (string current in enumerable2)
				{
					FileMirroring.DeleteFileOrDirectory(Path.Combine(to, current));
				}
				foreach (string current2 in enumerable)
				{
					string text = Path.Combine(from, current2);
					string text2 = Path.Combine(to, current2);
					FileMirroring.FileEntryType fileEntryType = FileMirroring.FileEntryTypeFor(text);
					FileMirroring.FileEntryType fileEntryType2 = FileMirroring.FileEntryTypeFor(text2);
					if (fileEntryType == FileMirroring.FileEntryType.File && fileEntryType2 == FileMirroring.FileEntryType.Directory)
					{
						FileMirroring.DeleteFileOrDirectory(text2);
					}
					if (fileEntryType == FileMirroring.FileEntryType.Directory)
					{
						if (fileEntryType2 == FileMirroring.FileEntryType.File)
						{
							FileMirroring.DeleteFileOrDirectory(text2);
						}
						if (fileEntryType2 != FileMirroring.FileEntryType.Directory)
						{
							Directory.CreateDirectory(text2);
						}
						FileMirroring.MirrorFolder(text, text2);
					}
					if (fileEntryType == FileMirroring.FileEntryType.File)
					{
						FileMirroring.MirrorFile(text, text2, comparer);
					}
				}
			}
		}

		private static void DeleteFileOrDirectory(string path)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			else if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
		}

		private static string StripPrefix(string s, string prefix)
		{
			return s.Substring(prefix.Length + 1);
		}

		private static FileMirroring.FileEntryType FileEntryTypeFor(string fileEntry)
		{
			FileMirroring.FileEntryType result;
			if (File.Exists(fileEntry))
			{
				result = FileMirroring.FileEntryType.File;
			}
			else if (Directory.Exists(fileEntry))
			{
				result = FileMirroring.FileEntryType.Directory;
			}
			else
			{
				result = FileMirroring.FileEntryType.NotExisting;
			}
			return result;
		}

		public static bool CanSkipCopy(string from, string to)
		{
			return File.Exists(to) && FileMirroring.AreFilesIdentical(from, to);
		}

		private static bool AreFilesIdentical(string filePath1, string filePath2)
		{
			bool result;
			using (FileStream fileStream = File.OpenRead(filePath1))
			{
				using (FileStream fileStream2 = File.OpenRead(filePath2))
				{
					if (fileStream.Length != fileStream2.Length)
					{
						result = false;
						return result;
					}
					byte[] array = new byte[65536];
					byte[] array2 = new byte[65536];
					int num;
					while ((num = fileStream.Read(array, 0, array.Length)) > 0)
					{
						fileStream2.Read(array2, 0, array2.Length);
						for (int i = 0; i < num; i++)
						{
							if (array[i] != array2[i])
							{
								result = false;
								return result;
							}
						}
					}
				}
			}
			result = true;
			return result;
		}
	}
}
