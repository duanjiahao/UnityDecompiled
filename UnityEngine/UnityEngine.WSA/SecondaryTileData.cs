using System;

namespace UnityEngine.WSA
{
	public struct SecondaryTileData
	{
		public string arguments;

		private Color32 background;

		public bool backgroundColorSet;

		public string displayName;

		public TileForegroundText foregroundText;

		public string lockScreenBadgeLogo;

		public bool lockScreenDisplayBadgeAndTileText;

		public string phoneticName;

		public bool roamingEnabled;

		public bool showNameOnSquare150x150Logo;

		public bool showNameOnSquare310x310Logo;

		public bool showNameOnWide310x150Logo;

		public string square150x150Logo;

		public string square30x30Logo;

		public string square310x310Logo;

		public string square70x70Logo;

		public string tileId;

		public string wide310x150Logo;

		public Color32 backgroundColor
		{
			get
			{
				return this.background;
			}
			set
			{
				this.background = value;
				this.backgroundColorSet = true;
			}
		}

		public SecondaryTileData(string id, string displayName)
		{
			this.arguments = string.Empty;
			this.background = new Color32(0, 0, 0, 0);
			this.backgroundColorSet = false;
			this.displayName = displayName;
			this.foregroundText = TileForegroundText.Default;
			this.lockScreenBadgeLogo = string.Empty;
			this.lockScreenDisplayBadgeAndTileText = false;
			this.phoneticName = string.Empty;
			this.roamingEnabled = true;
			this.showNameOnSquare150x150Logo = true;
			this.showNameOnSquare310x310Logo = false;
			this.showNameOnWide310x150Logo = false;
			this.square150x150Logo = string.Empty;
			this.square30x30Logo = string.Empty;
			this.square310x310Logo = string.Empty;
			this.square70x70Logo = string.Empty;
			this.tileId = id;
			this.wide310x150Logo = string.Empty;
		}
	}
}
