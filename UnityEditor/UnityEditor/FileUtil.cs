using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
namespace UnityEditor
{
	public sealed class FileUtil
	{
		internal static ArrayList ReadAllText(string path)
		{
			ArrayList arrayList = new ArrayList();
			using (StreamReader streamReader = File.OpenText(FileUtil.NiceWinPath(path)))
			{
				string value = string.Empty;
				while ((value = streamReader.ReadLine()) != null)
				{
					arrayList.Add(value);
				}
			}
			return arrayList;
		}
		internal static void WriteAllText(string path, ArrayList strings)
		{
			StreamWriter streamWriter = new StreamWriter(FileUtil.NiceWinPath(path));
			foreach (string value in strings)
			{
				streamWriter.WriteLine(value);
			}
			streamWriter.Close();
		}
		internal static void ReplaceText(string path, params string[] input)
		{
			ArrayList arrayList = FileUtil.ReadAllText(path);
			for (int i = 0; i < input.Length; i += 2)
			{
				for (int j = 0; j < arrayList.Count; j++)
				{
					string text = (string)arrayList[j];
					arrayList[j] = text.Replace(input[i], input[i + 1]);
				}
			}
			FileUtil.WriteAllText(path, arrayList);
		}
		internal static bool ReplaceTextRegex(string path, params string[] input)
		{
			bool result = false;
			ArrayList arrayList = FileUtil.ReadAllText(path);
			for (int i = 0; i < input.Length; i += 2)
			{
				for (int j = 0; j < arrayList.Count; j++)
				{
					string text = (string)arrayList[j];
					arrayList[j] = Regex.Replace(text, input[i], input[i + 1]);
					if (text != (string)arrayList[j])
					{
						result = true;
					}
				}
			}
			FileUtil.WriteAllText(path, arrayList);
			return result;
		}
		internal static bool AppendTextAfter(string path, string find, string append)
		{
			bool result = false;
			ArrayList arrayList = FileUtil.ReadAllText(path);
			for (int i = 0; i < arrayList.Count; i++)
			{
				if (((string)arrayList[i]).IndexOf(find) > -1)
				{
					arrayList.Insert(i + 1, append);
					result = true;
					break;
				}
			}
			FileUtil.WriteAllText(path, arrayList);
			return result;
		}
		internal static void CopyDirectoryRecursive(string source, string target)
		{
			FileUtil.CopyDirectoryRecursive(source, target, false, false);
		}
		internal static void CopyDirectoryRecursiveIgnoreMeta(string source, string target)
		{
			FileUtil.CopyDirectoryRecursive(source, target, false, true);
		}
		internal static void CopyDirectoryRecursive(string source, string target, bool overwrite)
		{
			FileUtil.CopyDirectoryRecursive(source, target, overwrite, false);
		}
		internal static void CopyDirectory(string source, string target, bool overwrite)
		{
			FileUtil.CopyDirectoryFiltered(source, target, overwrite, (string f) => true, false);
		}
		internal static void CopyDirectoryRecursive(string source, string target, bool overwrite, bool ignoreMeta)
		{
			FileUtil.CopyDirectoryRecursiveFiltered(source, target, overwrite, (!ignoreMeta) ? null : "\\.meta$");
		}
		internal static void CopyDirectoryRecursiveForPostprocess(string source, string target, bool overwrite)
		{
			FileUtil.CopyDirectoryRecursiveFiltered(source, target, overwrite, ".*/\\.+|\\.meta$");
		}
		internal static void CopyDirectoryRecursiveFiltered(string source, string target, bool overwrite, string regExExcludeFilter)
		{
			FileUtil.CopyDirectoryFiltered(source, target, overwrite, regExExcludeFilter, true);
		}
		internal static void CopyDirectoryFiltered(string source, string target, bool overwrite, string regExExcludeFilter, bool recursive)
		{
			Regex exclude = null;
			try
			{
				if (regExExcludeFilter != null)
				{
					exclude = new Regex(regExExcludeFilter);
				}
			}
			catch (ArgumentException)
			{
				Debug.Log("CopyDirectoryRecursive: Pattern '" + regExExcludeFilter + "' is not a correct Regular Expression. Not excluding any files.");
				return;
			}
			Func<string, bool> includeCallback = (string file) => exclude == null || !exclude.IsMatch(file);
			FileUtil.CopyDirectoryFiltered(source, target, overwrite, includeCallback, recursive);
		}
		internal static void CopyDirectoryFiltered(string source, string target, bool overwrite, Func<string, bool> includeCallback, bool recursive)
		{
			if (!Directory.Exists(target))
			{
				Directory.CreateDirectory(target);
				overwrite = false;
			}
			string[] files = Directory.GetFiles(source);
			for (int i = 0; i < files.Length; i++)
			{
				string text = files[i];
				if (includeCallback(text))
				{
					string fileName = Path.GetFileName(text);
					string to = Path.Combine(target, fileName);
					FileUtil.UnityFileCopy(text, to, overwrite);
				}
			}
			if (!recursive)
			{
				return;
			}
			string[] directories = Directory.GetDirectories(source);
			for (int j = 0; j < directories.Length; j++)
			{
				string text2 = directories[j];
				if (includeCallback(text2))
				{
					string fileName2 = Path.GetFileName(text2);
					FileUtil.CopyDirectoryFiltered(Path.Combine(source, fileName2), Path.Combine(target, fileName2), overwrite, includeCallback, recursive);
				}
			}
		}
		internal static void UnityDirectoryDelete(string path)
		{
			FileUtil.UnityDirectoryDelete(path, false);
		}
		internal static void UnityDirectoryDelete(string path, bool recursive)
		{
			Directory.Delete(FileUtil.NiceWinPath(path), recursive);
		}
		internal static void MoveFileIfExists(string src, string dst)
		{
			if (File.Exists(src))
			{
				FileUtil.DeleteFileOrDirectory(dst);
				FileUtil.MoveFileOrDirectory(src, dst);
				File.SetLastWriteTime(dst, DateTime.Now);
			}
		}
		internal static void CopyFileIfExists(string src, string dst, bool overwrite)
		{
			if (File.Exists(src))
			{
				FileUtil.UnityFileCopy(src, dst, overwrite);
			}
		}
		internal static void UnityFileCopy(string from, string to, bool overwrite)
		{
			File.Copy(FileUtil.NiceWinPath(from), FileUtil.NiceWinPath(to), overwrite);
		}
		internal static string NiceWinPath(string unityPath)
		{
			return (Application.platform != RuntimePlatform.WindowsEditor) ? unityPath : unityPath.Replace("/", "\\");
		}
		internal static string UnityGetFileNameWithoutExtension(string path)
		{
			return Path.GetFileNameWithoutExtension(path.Replace("//", "\\\\")).Replace("\\\\", "//");
		}
		internal static string UnityGetFileName(string path)
		{
			return Path.GetFileName(path.Replace("//", "\\\\")).Replace("\\\\", "//");
		}
		internal static string UnityGetDirectoryName(string path)
		{
			return Path.GetDirectoryName(path.Replace("//", "\\\\")).Replace("\\\\", "//");
		}
		internal static void UnityFileCopy(string from, string to)
		{
			FileUtil.UnityFileCopy(from, to, false);
		}
		internal static void CreateOrCleanDirectory(string dir)
		{
			if (Directory.Exists(dir))
			{
				Directory.Delete(dir, true);
			}
			Directory.CreateDirectory(dir);
		}
		internal static string RemovePathPrefix(string fullPath, string prefix)
		{
			string[] array = fullPath.Split(new char[]
			{
				Path.DirectorySeparatorChar
			});
			string[] array2 = prefix.Split(new char[]
			{
				Path.DirectorySeparatorChar
			});
			int num = 0;
			if (array[0] == string.Empty)
			{
				num = 1;
			}
			while (num < array.Length && num < array2.Length && array[num] == array2[num])
			{
				num++;
			}
			if (num == array.Length)
			{
				return string.Empty;
			}
			return string.Join(Path.DirectorySeparatorChar.ToString(), array, num, array.Length - num);
		}
		internal static string CombinePaths(params string[] paths)
		{
			if (paths == null)
			{
				return string.Empty;
			}
			return string.Join(Path.DirectorySeparatorChar.ToString(), paths);
		}
		internal static List<string> GetAllFilesRecursive(string path)
		{
			List<string> files = new List<string>();
			FileUtil.WalkFilesystemRecursively(path, delegate(string p)
			{
				files.Add(p);
			}, (string p) => true);
			return files;
		}
		internal static void WalkFilesystemRecursively(string path, Action<string> fileCallback, Func<string, bool> directoryCallback)
		{
			string[] files = Directory.GetFiles(path);
			for (int i = 0; i < files.Length; i++)
			{
				string obj = files[i];
				fileCallback(obj);
			}
			string[] directories = Directory.GetDirectories(path);
			for (int j = 0; j < directories.Length; j++)
			{
				string text = directories[j];
				if (directoryCallback(text))
				{
					FileUtil.WalkFilesystemRecursively(text, fileCallback, directoryCallback);
				}
			}
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool DeleteFileOrDirectory(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyFileOrDirectory(string from, string to);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyFileOrDirectoryFollowSymlinks(string from, string to);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MoveFileOrDirectory(string from, string to);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetUniqueTempPathInProject();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetActualPathName(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetProjectRelativePath(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetLastPathNameComponent(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string DeleteLastPathNameComponent(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetPathExtension(string path);
		public static void ReplaceFile(string src, string dst)
		{
			if (File.Exists(dst))
			{
				FileUtil.DeleteFileOrDirectory(dst);
			}
			FileUtil.CopyFileOrDirectory(src, dst);
		}
		public static void ReplaceDirectory(string src, string dst)
		{
			if (Directory.Exists(dst))
			{
				FileUtil.DeleteFileOrDirectory(dst);
			}
			FileUtil.CopyFileOrDirectory(src, dst);
		}
	}
}
