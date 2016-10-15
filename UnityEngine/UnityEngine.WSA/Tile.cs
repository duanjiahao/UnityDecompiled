using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.WSA
{
	public sealed class Tile
	{
		private string m_TileId;

		private static Tile s_MainTile;

		public static Tile main
		{
			get
			{
				if (Tile.s_MainTile == null)
				{
					Tile.s_MainTile = new Tile(string.Empty);
				}
				return Tile.s_MainTile;
			}
		}

		public string id
		{
			get
			{
				return this.m_TileId;
			}
		}

		public bool hasUserConsent
		{
			get
			{
				return Tile.HasUserConsent(this.m_TileId);
			}
		}

		public bool exists
		{
			get
			{
				return Tile.Exists(this.m_TileId);
			}
		}

		private Tile(string tileId)
		{
			this.m_TileId = tileId;
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetTemplate(TileTemplate templ);

		public void Update(string xml)
		{
			Tile.Update(this.m_TileId, xml);
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Update(string tileId, string xml);

		public void Update(string medium, string wide, string large, string text)
		{
			Tile.UpdateImageAndText(this.m_TileId, medium, wide, large, text);
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateImageAndText(string tileId, string medium, string wide, string large, string text);

		public void PeriodicUpdate(string uri, float interval)
		{
			Tile.PeriodicUpdate(this.m_TileId, uri, interval);
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PeriodicUpdate(string tileId, string uri, float interval);

		public void StopPeriodicUpdate()
		{
			Tile.StopPeriodicUpdate(this.m_TileId);
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopPeriodicUpdate(string tileId);

		public void UpdateBadgeImage(string image)
		{
			Tile.UpdateBadgeImage(this.m_TileId, image);
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateBadgeImage(string tileId, string image);

		public void UpdateBadgeNumber(float number)
		{
			Tile.UpdateBadgeNumber(this.m_TileId, number);
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateBadgeNumber(string tileId, float number);

		public void RemoveBadge()
		{
			Tile.RemoveBadge(this.m_TileId);
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveBadge(string tileId);

		public void PeriodicBadgeUpdate(string uri, float interval)
		{
			Tile.PeriodicBadgeUpdate(this.m_TileId, uri, interval);
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PeriodicBadgeUpdate(string tileId, string uri, float interval);

		public void StopPeriodicBadgeUpdate()
		{
			Tile.StopPeriodicBadgeUpdate(this.m_TileId);
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopPeriodicBadgeUpdate(string tileId);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasUserConsent(string tileId);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Exists(string tileId);

		private static string[] MakeSecondaryTileSargs(SecondaryTileData data)
		{
			return new string[]
			{
				data.arguments,
				data.displayName,
				data.lockScreenBadgeLogo,
				data.phoneticName,
				data.square150x150Logo,
				data.square30x30Logo,
				data.square310x310Logo,
				data.square70x70Logo,
				data.tileId,
				data.wide310x150Logo
			};
		}

		private static bool[] MakeSecondaryTileBargs(SecondaryTileData data)
		{
			return new bool[]
			{
				data.backgroundColorSet,
				data.lockScreenDisplayBadgeAndTileText,
				data.roamingEnabled,
				data.showNameOnSquare150x150Logo,
				data.showNameOnSquare310x310Logo,
				data.showNameOnWide310x150Logo
			};
		}

		public static Tile CreateOrUpdateSecondary(SecondaryTileData data)
		{
			string[] sargs = Tile.MakeSecondaryTileSargs(data);
			bool[] bargs = Tile.MakeSecondaryTileBargs(data);
			Color32 backgroundColor = data.backgroundColor;
			string text = Tile.CreateOrUpdateSecondaryTile(sargs, bargs, ref backgroundColor, (int)data.foregroundText);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return new Tile(text);
		}

		[ThreadAndSerializationSafe]
		private static string CreateOrUpdateSecondaryTile(string[] sargs, bool[] bargs, ref Color32 backgroundColor, int foregroundText)
		{
			return Tile.INTERNAL_CALL_CreateOrUpdateSecondaryTile(sargs, bargs, ref backgroundColor, foregroundText);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string INTERNAL_CALL_CreateOrUpdateSecondaryTile(string[] sargs, bool[] bargs, ref Color32 backgroundColor, int foregroundText);

		public static Tile CreateOrUpdateSecondary(SecondaryTileData data, Vector2 pos)
		{
			string[] sargs = Tile.MakeSecondaryTileSargs(data);
			bool[] bargs = Tile.MakeSecondaryTileBargs(data);
			Color32 backgroundColor = data.backgroundColor;
			string text = Tile.CreateOrUpdateSecondaryTilePoint(sargs, bargs, ref backgroundColor, (int)data.foregroundText, pos);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return new Tile(text);
		}

		[ThreadAndSerializationSafe]
		private static string CreateOrUpdateSecondaryTilePoint(string[] sargs, bool[] bargs, ref Color32 backgroundColor, int foregroundText, Vector2 pos)
		{
			return Tile.INTERNAL_CALL_CreateOrUpdateSecondaryTilePoint(sargs, bargs, ref backgroundColor, foregroundText, ref pos);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string INTERNAL_CALL_CreateOrUpdateSecondaryTilePoint(string[] sargs, bool[] bargs, ref Color32 backgroundColor, int foregroundText, ref Vector2 pos);

		public static Tile CreateOrUpdateSecondary(SecondaryTileData data, Rect area)
		{
			string[] sargs = Tile.MakeSecondaryTileSargs(data);
			bool[] bargs = Tile.MakeSecondaryTileBargs(data);
			Color32 backgroundColor = data.backgroundColor;
			string text = Tile.CreateOrUpdateSecondaryTileArea(sargs, bargs, ref backgroundColor, (int)data.foregroundText, area);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return new Tile(text);
		}

		[ThreadAndSerializationSafe]
		private static string CreateOrUpdateSecondaryTileArea(string[] sargs, bool[] bargs, ref Color32 backgroundColor, int foregroundText, Rect area)
		{
			return Tile.INTERNAL_CALL_CreateOrUpdateSecondaryTileArea(sargs, bargs, ref backgroundColor, foregroundText, ref area);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string INTERNAL_CALL_CreateOrUpdateSecondaryTileArea(string[] sargs, bool[] bargs, ref Color32 backgroundColor, int foregroundText, ref Rect area);

		public static Tile GetSecondary(string tileId)
		{
			if (Tile.Exists(tileId))
			{
				return new Tile(tileId);
			}
			return null;
		}

		public static Tile[] GetSecondaries()
		{
			string[] allSecondaryTiles = Tile.GetAllSecondaryTiles();
			Tile[] array = new Tile[allSecondaryTiles.Length];
			for (int i = 0; i < allSecondaryTiles.Length; i++)
			{
				array[i] = new Tile(allSecondaryTiles[i]);
			}
			return array;
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetAllSecondaryTiles();

		public void Delete()
		{
			Tile.DeleteSecondary(this.m_TileId);
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DeleteSecondary(string tileId);

		public void Delete(Vector2 pos)
		{
			Tile.DeleteSecondaryPos(this.m_TileId, pos);
		}

		public static void DeleteSecondary(string tileId, Vector2 pos)
		{
			Tile.DeleteSecondaryPos(tileId, pos);
		}

		[ThreadAndSerializationSafe]
		private static void DeleteSecondaryPos(string tileId, Vector2 pos)
		{
			Tile.INTERNAL_CALL_DeleteSecondaryPos(tileId, ref pos);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DeleteSecondaryPos(string tileId, ref Vector2 pos);

		public void Delete(Rect area)
		{
			Tile.DeleteSecondaryArea(this.m_TileId, area);
		}

		public static void DeleteSecondary(string tileId, Rect area)
		{
			Tile.DeleteSecondary(tileId, area);
		}

		[ThreadAndSerializationSafe]
		private static void DeleteSecondaryArea(string tileId, Rect area)
		{
			Tile.INTERNAL_CALL_DeleteSecondaryArea(tileId, ref area);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DeleteSecondaryArea(string tileId, ref Rect area);
	}
}
