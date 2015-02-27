using System;
using UnityEditor;
internal class PostProcessFlashPlayerOptions
{
	public BuildTarget Target;
	public string StagingAreaData;
	public string StagingArea;
	public string StagingAreaDataManaged;
	public string PlayerPackage;
	public string InstallPath;
	public string CompanyName;
	public string ProductName;
	public BuildOptions Options;
	public RuntimeClassRegistry UsedClassRegistry;
	public int Width;
	public int Height;
	public bool IsAutoRunPlayer
	{
		get
		{
			return (this.Options & BuildOptions.AutoRunPlayer) != BuildOptions.None;
		}
	}
}
