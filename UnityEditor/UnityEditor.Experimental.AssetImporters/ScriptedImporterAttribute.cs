using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UnityEditor.Experimental.AssetImporters
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ScriptedImporterAttribute : Attribute
	{
		[CompilerGenerated]
		private static Func<string, bool> <>f__mg$cache0;

		public int version
		{
			get;
			private set;
		}

		public int importQueuePriority
		{
			get;
			private set;
		}

		public string[] fileExtensions
		{
			get;
			private set;
		}

		public ScriptedImporterAttribute(int version, string[] exts)
		{
			this.Init(version, exts, 0);
		}

		public ScriptedImporterAttribute(int version, string ext)
		{
			this.Init(version, new string[]
			{
				ext
			}, 0);
		}

		public ScriptedImporterAttribute(int version, string[] exts, int importQueueOffset)
		{
			this.Init(version, exts, importQueueOffset);
		}

		public ScriptedImporterAttribute(int version, string ext, int importQueueOffset)
		{
			this.Init(version, new string[]
			{
				ext
			}, importQueueOffset);
		}

		private void Init(int version, string[] exts, int importQueueOffset)
		{
			if (exts != null)
			{
				if (ScriptedImporterAttribute.<>f__mg$cache0 == null)
				{
					ScriptedImporterAttribute.<>f__mg$cache0 = new Func<string, bool>(string.IsNullOrEmpty);
				}
				if (!exts.Any(ScriptedImporterAttribute.<>f__mg$cache0))
				{
					this.version = version;
					this.importQueuePriority = importQueueOffset;
					this.fileExtensions = exts;
					return;
				}
			}
			throw new ArgumentException("Must provide valid, none null, file extension strings.");
		}
	}
}
