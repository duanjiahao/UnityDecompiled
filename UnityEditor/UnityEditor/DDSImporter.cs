using System;

namespace UnityEditor
{
	[Obsolete("DDSImporter is obsolete. Use IHVImageFormatImporter instead (UnityUpgradable) -> IHVImageFormatImporter", true)]
	public class DDSImporter : AssetImporter
	{
		public bool isReadable
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
	}
}
