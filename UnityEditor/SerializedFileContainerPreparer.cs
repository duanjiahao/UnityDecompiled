using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
internal class SerializedFileContainerPreparer
{
	private static string _stagingAreaData;
	public static void CreateSerializedFileContainer(FlashFileHelper flashFileHelper)
	{
		SerializedFileContainerPreparer._stagingAreaData = flashFileHelper.StagingArea;
		IEnumerable<string> source = SerializedFileContainerPreparer.FindFilesThatShouldBeEmbedded();
		SerializedFileContainerWriter.SerializedFile[] files = (
			from f in source
			select SerializedFileContainerPreparer.GetSerializedFile(f)).ToArray<SerializedFileContainerWriter.SerializedFile>();
		SerializedFileContainerWriter.Write(SerializedFileContainerPreparer._stagingAreaData, files, flashFileHelper.PathForAs3Src());
	}
	private static IEnumerable<string> FindFilesThatShouldBeEmbedded()
	{
		List<string> list = new List<string>
		{
			"mainData",
			"Resources/unity default resources"
		};
		string[] patterns = new string[]
		{
			"\\.assets$",
			"level",
			"PlayerConnectionConfigFile"
		};
		list.AddRange(
			from ps in Directory.GetFiles(SerializedFileContainerPreparer._stagingAreaData)
			where SerializedFileContainerPreparer.MatchesFilter(ps, patterns)
			select ps into f
			select Path.GetFileName(f));
		string path = Path.Combine(SerializedFileContainerPreparer._stagingAreaData, "Resources/unity_builtin_extra");
		if (File.Exists(path))
		{
			list.Add("Resources/unity_builtin_extra");
		}
		return list;
	}
	private static SerializedFileContainerWriter.SerializedFile GetSerializedFile(string file)
	{
		long length = new FileInfo(SerializedFileContainerPreparer._stagingAreaData + "/" + file).Length;
		return new SerializedFileContainerWriter.SerializedFile
		{
			Name = file,
			FileName = SerializedFileContainerPreparer.GetTextPlaceHolderForFile(file),
			FileSize = length
		};
	}
	private static string GetTextPlaceHolderForFile(string fileName)
	{
		string str = Environment.CurrentDirectory + "/" + SerializedFileContainerPreparer._stagingAreaData + "/";
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		string str2 = fileName + "_txt";
		string text = "_UNITY_REPLACEFORFILE__" + str + fileName;
		using (FileStream fileStream = new FileStream(SerializedFileContainerPreparer._stagingAreaData + "/" + str2, FileMode.CreateNew))
		{
			fileStream.Write(uTF8Encoding.GetBytes(text), 0, uTF8Encoding.GetByteCount(text));
		}
		return fileName;
	}
	private static bool MatchesFilter(string input, IEnumerable<string> patterns)
	{
		return patterns.Any((string p) => Regex.Match(input, p).Success);
	}
}
