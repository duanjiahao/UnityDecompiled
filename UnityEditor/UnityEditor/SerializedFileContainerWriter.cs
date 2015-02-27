using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace UnityEditor
{
	internal static class SerializedFileContainerWriter
	{
		internal class SerializedFile
		{
			public string Name;
			public string FileName;
			public long FileSize;
		}
		public static void Write(string filepath, IEnumerable<SerializedFileContainerWriter.SerializedFile> files, string outputdir)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(SerializedFileContainerWriter.GetHead());
			filepath = Environment.CurrentDirectory + "/" + filepath;
			filepath = filepath.Replace("\\", "/");
			foreach (SerializedFileContainerWriter.SerializedFile current in files)
			{
				stringBuilder.Append(SerializedFileContainerWriter.GetFieldDeclarationForFile(filepath, current.FileName));
			}
			stringBuilder.Append(SerializedFileContainerWriter.GetConstructor(files));
			stringBuilder.Append(SerializedFileContainerWriter.GetTail());
			File.WriteAllText(Path.Combine(outputdir, "ProjectSerializedFileContainer.as"), stringBuilder.ToString());
		}
		private static string GetHead()
		{
			return "\npackage\n{\n        import flash.utils.ByteArray;\n        import flash.utils.Dictionary;\n        import flash.utils.Endian;\n        import UnityEngine.*;\n\t\timport com.unity.SerializedFileContainer;\n\n        public class ProjectSerializedFileContainer extends SerializedFileContainer\n        {\n                \n";
		}
		private static string GetFieldDeclarationForFile(string filepath, string file)
		{
			return string.Concat(new string[]
			{
				"        [Embed(\"",
				filepath,
				"/",
				file,
				"_txt\", mimeType=\"application/octet-stream\")]\n        private var ",
				SerializedFileContainerWriter.GetTypeNameForFile(file),
				":Class;\n\n"
			});
		}
		private static string GetConstructor(IEnumerable<SerializedFileContainerWriter.SerializedFile> files)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("     \n       public function ProjectSerializedFileContainer()\n       {\n              files = new Dictionary();\n");
			foreach (SerializedFileContainerWriter.SerializedFile current in files)
			{
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"files[\"",
					current.Name,
					"\"] = ",
					SerializedFileContainerWriter.GetArrayOfByteArrays(current)
				}));
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"fileSizes[\"",
					current.Name,
					"\"] = ",
					current.FileSize
				}));
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}
		private static StringBuilder GetArrayOfByteArrays(SerializedFileContainerWriter.SerializedFile file)
		{
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			stringBuilder.Append("new " + SerializedFileContainerWriter.GetTypeNameForFile(file.Name) + "() as ByteArray");
			return stringBuilder;
		}
		private static string GetTypeNameForFile(string file)
		{
			file = file.Replace("/", "_");
			file = file.Replace(" ", "_");
			return file.Replace(".", "_");
		}
		private static string GetTail()
		{
			return "\t}\n}\n";
		}
	}
}
