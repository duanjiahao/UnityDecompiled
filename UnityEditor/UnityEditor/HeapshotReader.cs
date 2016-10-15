using System;
using System.Collections.Generic;
using System.IO;

namespace UnityEditor
{
	internal class HeapshotReader
	{
		public enum Tag
		{
			Type = 1,
			Object,
			UnityObjects,
			EndOfFile = 255
		}

		public enum ObjectType
		{
			None,
			Root,
			Managed,
			UnityRoot
		}

		public class FieldInfo
		{
			public string name = string.Empty;

			public FieldInfo()
			{
			}

			public FieldInfo(string name)
			{
				this.name = name;
			}
		}

		public class TypeInfo
		{
			public string name = string.Empty;

			public Dictionary<uint, HeapshotReader.FieldInfo> fields = new Dictionary<uint, HeapshotReader.FieldInfo>();

			public TypeInfo()
			{
			}

			public TypeInfo(string name)
			{
				this.name = name;
			}
		}

		public class ReferenceInfo
		{
			public uint code;

			public HeapshotReader.ObjectInfo referencedObject;

			public HeapshotReader.FieldInfo fieldInfo;

			public ReferenceInfo()
			{
			}

			public ReferenceInfo(HeapshotReader.ObjectInfo refObj, HeapshotReader.FieldInfo field)
			{
				this.referencedObject = refObj;
				this.fieldInfo = field;
			}
		}

		public class BackReferenceInfo
		{
			public HeapshotReader.ObjectInfo parentObject;

			public HeapshotReader.FieldInfo fieldInfo;
		}

		public class ObjectInfo
		{
			public uint code;

			public HeapshotReader.TypeInfo typeInfo;

			public uint size;

			public List<HeapshotReader.ReferenceInfo> references = new List<HeapshotReader.ReferenceInfo>();

			public List<HeapshotReader.BackReferenceInfo> inverseReferences = new List<HeapshotReader.BackReferenceInfo>();

			public HeapshotReader.ObjectType type;

			public ObjectInfo()
			{
			}

			public ObjectInfo(HeapshotReader.TypeInfo typeInfo, uint size)
			{
				this.typeInfo = typeInfo;
				this.size = size;
			}

			public ObjectInfo(HeapshotReader.TypeInfo typeInfo, uint size, HeapshotReader.ObjectType type)
			{
				this.typeInfo = typeInfo;
				this.size = size;
				this.type = type;
			}
		}

		private const uint kMagicNumber = 1319894481u;

		private const int kLogVersion = 6;

		private const string kLogFileLabel = "heap-shot logfile";

		private Dictionary<uint, HeapshotReader.TypeInfo> types = new Dictionary<uint, HeapshotReader.TypeInfo>();

		private Dictionary<uint, HeapshotReader.ObjectInfo> objects = new Dictionary<uint, HeapshotReader.ObjectInfo>();

		private List<HeapshotReader.ReferenceInfo> roots = new List<HeapshotReader.ReferenceInfo>();

		private List<HeapshotReader.ObjectInfo> allObjects = new List<HeapshotReader.ObjectInfo>();

		private List<HeapshotReader.TypeInfo> allTypes = new List<HeapshotReader.TypeInfo>();

		private HeapshotReader.ObjectInfo kUnmanagedObject = new HeapshotReader.ObjectInfo(new HeapshotReader.TypeInfo("Unmanaged"), 0u);

		private HeapshotReader.ObjectInfo kUnity = new HeapshotReader.ObjectInfo(new HeapshotReader.TypeInfo("<Unity>"), 0u, HeapshotReader.ObjectType.UnityRoot);

		public List<HeapshotReader.ReferenceInfo> Roots
		{
			get
			{
				return this.roots;
			}
		}

		public List<HeapshotReader.ObjectInfo> Objects
		{
			get
			{
				return this.allObjects;
			}
		}

		public List<HeapshotReader.TypeInfo> Types
		{
			get
			{
				return this.allTypes;
			}
		}

		public List<HeapshotReader.ObjectInfo> GetObjectsOfType(string name)
		{
			List<HeapshotReader.ObjectInfo> list = new List<HeapshotReader.ObjectInfo>();
			foreach (HeapshotReader.ObjectInfo current in this.allObjects)
			{
				if (current.typeInfo.name == name)
				{
					list.Add(current);
				}
			}
			return list;
		}

		public bool Open(string fileName)
		{
			this.types.Clear();
			this.objects.Clear();
			this.roots.Clear();
			this.allObjects.Clear();
			this.allTypes.Clear();
			Stream stream;
			try
			{
				stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				bool result = false;
				return result;
			}
			BinaryReader binaryReader;
			try
			{
				binaryReader = new BinaryReader(stream);
			}
			catch (Exception ex2)
			{
				Console.WriteLine(ex2.Message);
				stream.Close();
				bool result = false;
				return result;
			}
			this.ReadHeader(binaryReader);
			while (this.ReadData(binaryReader))
			{
			}
			this.ResolveReferences();
			this.ResolveInverseReferences();
			this.ResolveRoots();
			binaryReader.Close();
			stream.Close();
			return true;
		}

		private void ReadHeader(BinaryReader reader)
		{
			uint num = reader.ReadUInt32();
			if (num != 1319894481u)
			{
				throw new Exception(string.Format("Bad magic number: expected {0}, found {1}", 1319894481u, num));
			}
			int num2 = reader.ReadInt32();
			string text = reader.ReadString();
			if (!(text == "heap-shot logfile"))
			{
				throw new Exception("Unknown file label in heap-shot outfile");
			}
			int num3 = 6;
			if (num2 != num3)
			{
				throw new Exception(string.Format("Version error in {0}: expected {1}, found {2}", text, num3, num2));
			}
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
		}

		private bool ReadData(BinaryReader reader)
		{
			HeapshotReader.Tag tag = (HeapshotReader.Tag)reader.ReadByte();
			HeapshotReader.Tag tag2 = tag;
			switch (tag2)
			{
			case HeapshotReader.Tag.Type:
				this.ReadType(reader);
				break;
			case HeapshotReader.Tag.Object:
				this.ReadObject(reader);
				break;
			case HeapshotReader.Tag.UnityObjects:
				this.ReadUnityObjects(reader);
				break;
			default:
				if (tag2 != HeapshotReader.Tag.EndOfFile)
				{
					throw new Exception("Unknown tag! " + tag);
				}
				return false;
			}
			return true;
		}

		private void ReadType(BinaryReader reader)
		{
			uint num = reader.ReadUInt32();
			HeapshotReader.TypeInfo typeInfo = new HeapshotReader.TypeInfo();
			typeInfo.name = reader.ReadString();
			uint key;
			while ((key = reader.ReadUInt32()) != 0u)
			{
				HeapshotReader.FieldInfo fieldInfo = new HeapshotReader.FieldInfo();
				fieldInfo.name = reader.ReadString();
				typeInfo.fields[key] = fieldInfo;
			}
			if (this.types.ContainsKey(num))
			{
				throw new Exception(string.Format("Type info for object {0} was already loaded!!!", num));
			}
			this.types[num] = typeInfo;
			this.allTypes.Add(typeInfo);
		}

		private void ReadObject(BinaryReader reader)
		{
			uint num = reader.ReadUInt32();
			uint num2 = reader.ReadUInt32();
			HeapshotReader.ObjectInfo objectInfo = new HeapshotReader.ObjectInfo();
			objectInfo.code = num;
			objectInfo.size = reader.ReadUInt32();
			if (!this.types.ContainsKey(num2))
			{
				throw new Exception(string.Format("Failed to find type info {0} for object {1}!!!", num2, num));
			}
			objectInfo.typeInfo = this.types[num2];
			uint code;
			while ((code = reader.ReadUInt32()) != 0u)
			{
				HeapshotReader.ReferenceInfo referenceInfo = new HeapshotReader.ReferenceInfo();
				referenceInfo.code = code;
				uint num3 = reader.ReadUInt32();
				if (num3 == 0u)
				{
					referenceInfo.fieldInfo = null;
				}
				else if (objectInfo.typeInfo.fields.ContainsKey(num3))
				{
					referenceInfo.fieldInfo = objectInfo.typeInfo.fields[num3];
				}
				else
				{
					referenceInfo.fieldInfo = null;
				}
				objectInfo.references.Add(referenceInfo);
			}
			if (this.objects.ContainsKey(num))
			{
				throw new Exception(string.Format("Object {0} was already loaded?!", num));
			}
			objectInfo.type = ((num != num2) ? HeapshotReader.ObjectType.Managed : HeapshotReader.ObjectType.Root);
			this.objects[num] = objectInfo;
			this.allObjects.Add(objectInfo);
		}

		private void ReadUnityObjects(BinaryReader reader)
		{
			uint key;
			while ((key = reader.ReadUInt32()) != 0u)
			{
				if (this.objects.ContainsKey(key))
				{
					HeapshotReader.BackReferenceInfo backReferenceInfo = new HeapshotReader.BackReferenceInfo();
					backReferenceInfo.parentObject = this.kUnity;
					backReferenceInfo.fieldInfo = new HeapshotReader.FieldInfo(this.objects[key].typeInfo.name);
					this.objects[key].inverseReferences.Add(backReferenceInfo);
				}
			}
		}

		private void ResolveReferences()
		{
			foreach (KeyValuePair<uint, HeapshotReader.ObjectInfo> current in this.objects)
			{
				foreach (HeapshotReader.ReferenceInfo current2 in current.Value.references)
				{
					if (!this.objects.ContainsKey(current2.code))
					{
						current2.referencedObject = this.kUnmanagedObject;
					}
					else
					{
						current2.referencedObject = this.objects[current2.code];
						if (current2.fieldInfo == null)
						{
							current2.fieldInfo = new HeapshotReader.FieldInfo();
							current2.fieldInfo.name = current2.referencedObject.typeInfo.name;
						}
					}
				}
			}
		}

		private void ResolveInverseReferences()
		{
			foreach (KeyValuePair<uint, HeapshotReader.ObjectInfo> current in this.objects)
			{
				foreach (HeapshotReader.ReferenceInfo current2 in current.Value.references)
				{
					HeapshotReader.BackReferenceInfo backReferenceInfo = new HeapshotReader.BackReferenceInfo();
					backReferenceInfo.parentObject = current.Value;
					backReferenceInfo.fieldInfo = current2.fieldInfo;
					current2.referencedObject.inverseReferences.Add(backReferenceInfo);
				}
			}
		}

		private void ResolveRoots()
		{
			foreach (KeyValuePair<uint, HeapshotReader.ObjectInfo> current in this.objects)
			{
				if (current.Value.type == HeapshotReader.ObjectType.Root)
				{
					foreach (HeapshotReader.ReferenceInfo current2 in current.Value.references)
					{
						this.roots.Add(current2);
					}
				}
			}
		}
	}
}
