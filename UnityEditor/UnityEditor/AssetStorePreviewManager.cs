using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class AssetStorePreviewManager
	{
		public class CachedAssetStoreImage
		{
			private const double kFadeTime = 0.5;

			public Texture2D image;

			public double lastUsed;

			public double lastFetched;

			public int requestedWidth;

			public string label;

			internal AsyncHTTPClient client;

			public Color color
			{
				get
				{
					return Color.Lerp(new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), Mathf.Min(1f, (float)((EditorApplication.timeSinceStartup - this.lastFetched) / 0.5)));
				}
			}
		}

		private static AssetStorePreviewManager s_SharedAssetStorePreviewManager = null;

		private static RenderTexture s_RenderTexture = null;

		private Dictionary<string, AssetStorePreviewManager.CachedAssetStoreImage> m_CachedAssetStoreImages;

		private const double kQueryDelay = 0.2;

		private const int kMaxConcurrentDownloads = 15;

		private const int kMaxConvertionsPerTick = 1;

		private int m_MaxCachedAssetStoreImages = 10;

		private int m_Aborted = 0;

		private int m_Success = 0;

		internal int Requested = 0;

		internal int CacheHit = 0;

		private int m_CacheRemove = 0;

		private int m_ConvertedThisTick = 0;

		private AssetStorePreviewManager.CachedAssetStoreImage m_DummyItem = new AssetStorePreviewManager.CachedAssetStoreImage();

		private static bool s_NeedsRepaint = false;

		internal static AssetStorePreviewManager Instance
		{
			get
			{
				if (AssetStorePreviewManager.s_SharedAssetStorePreviewManager == null)
				{
					AssetStorePreviewManager.s_SharedAssetStorePreviewManager = new AssetStorePreviewManager();
					AssetStorePreviewManager.s_SharedAssetStorePreviewManager.m_DummyItem.lastUsed = -1.0;
				}
				return AssetStorePreviewManager.s_SharedAssetStorePreviewManager;
			}
		}

		private static Dictionary<string, AssetStorePreviewManager.CachedAssetStoreImage> CachedAssetStoreImages
		{
			get
			{
				if (AssetStorePreviewManager.Instance.m_CachedAssetStoreImages == null)
				{
					AssetStorePreviewManager.Instance.m_CachedAssetStoreImages = new Dictionary<string, AssetStorePreviewManager.CachedAssetStoreImage>();
				}
				return AssetStorePreviewManager.Instance.m_CachedAssetStoreImages;
			}
		}

		public static int MaxCachedImages
		{
			get
			{
				return AssetStorePreviewManager.Instance.m_MaxCachedAssetStoreImages;
			}
			set
			{
				AssetStorePreviewManager.Instance.m_MaxCachedAssetStoreImages = value;
			}
		}

		public static bool CacheFull
		{
			get
			{
				return AssetStorePreviewManager.CachedAssetStoreImages.Count >= AssetStorePreviewManager.MaxCachedImages;
			}
		}

		public static int Downloading
		{
			get
			{
				int num = 0;
				foreach (KeyValuePair<string, AssetStorePreviewManager.CachedAssetStoreImage> current in AssetStorePreviewManager.CachedAssetStoreImages)
				{
					if (current.Value.client != null)
					{
						num++;
					}
				}
				return num;
			}
		}

		private AssetStorePreviewManager()
		{
		}

		public static string StatsString()
		{
			return string.Format("Reqs: {0}, Ok: {1}, Abort: {2}, CacheDel: {3}, Cache: {4}/{5}, CacheHit: {6}", new object[]
			{
				AssetStorePreviewManager.Instance.Requested,
				AssetStorePreviewManager.Instance.m_Success,
				AssetStorePreviewManager.Instance.m_Aborted,
				AssetStorePreviewManager.Instance.m_CacheRemove,
				AssetStorePreviewManager.CachedAssetStoreImages.Count,
				AssetStorePreviewManager.Instance.m_MaxCachedAssetStoreImages,
				AssetStorePreviewManager.Instance.CacheHit
			});
		}

		public static AssetStorePreviewManager.CachedAssetStoreImage TextureFromUrl(string url, string label, int textureSize, GUIStyle labelStyle, GUIStyle iconStyle, bool onlyCached)
		{
			AssetStorePreviewManager.CachedAssetStoreImage result;
			if (string.IsNullOrEmpty(url))
			{
				result = AssetStorePreviewManager.Instance.m_DummyItem;
			}
			else
			{
				bool flag = true;
				AssetStorePreviewManager.CachedAssetStoreImage cachedAssetStoreImage;
				if (AssetStorePreviewManager.CachedAssetStoreImages.TryGetValue(url, out cachedAssetStoreImage))
				{
					cachedAssetStoreImage.lastUsed = EditorApplication.timeSinceStartup;
					bool flag2 = cachedAssetStoreImage.requestedWidth == textureSize;
					bool flag3 = cachedAssetStoreImage.image != null && cachedAssetStoreImage.image.width == textureSize;
					bool flag4 = cachedAssetStoreImage.requestedWidth == -1;
					if ((flag3 || flag2 || onlyCached) && !flag4)
					{
						AssetStorePreviewManager.Instance.CacheHit++;
						bool flag5 = cachedAssetStoreImage.client != null;
						bool flag6 = cachedAssetStoreImage.label == null;
						bool flag7 = flag5 || flag6;
						bool flag8 = AssetStorePreviewManager.Instance.m_ConvertedThisTick > 1;
						AssetStorePreviewManager.s_NeedsRepaint = (AssetStorePreviewManager.s_NeedsRepaint || flag8);
						result = ((!flag7 && !flag8) ? AssetStorePreviewManager.RenderEntry(cachedAssetStoreImage, labelStyle, iconStyle) : cachedAssetStoreImage);
						return result;
					}
					flag = false;
					if (AssetStorePreviewManager.Downloading >= 15)
					{
						result = ((!(cachedAssetStoreImage.image == null)) ? cachedAssetStoreImage : AssetStorePreviewManager.Instance.m_DummyItem);
						return result;
					}
				}
				else
				{
					if (onlyCached || AssetStorePreviewManager.Downloading >= 15)
					{
						result = AssetStorePreviewManager.Instance.m_DummyItem;
						return result;
					}
					cachedAssetStoreImage = new AssetStorePreviewManager.CachedAssetStoreImage();
					cachedAssetStoreImage.image = null;
					cachedAssetStoreImage.lastUsed = EditorApplication.timeSinceStartup;
				}
				if (cachedAssetStoreImage.image == null)
				{
					cachedAssetStoreImage.lastFetched = EditorApplication.timeSinceStartup;
				}
				cachedAssetStoreImage.requestedWidth = textureSize;
				cachedAssetStoreImage.label = label;
				AsyncHTTPClient asyncHTTPClient = AssetStorePreviewManager.SetupTextureDownload(cachedAssetStoreImage, url, "previewSize-" + textureSize);
				AssetStorePreviewManager.ExpireCacheEntries();
				if (flag)
				{
					AssetStorePreviewManager.CachedAssetStoreImages.Add(url, cachedAssetStoreImage);
				}
				asyncHTTPClient.Begin();
				AssetStorePreviewManager.Instance.Requested++;
				result = cachedAssetStoreImage;
			}
			return result;
		}

		private static AsyncHTTPClient SetupTextureDownload(AssetStorePreviewManager.CachedAssetStoreImage cached, string url, string tag)
		{
			AsyncHTTPClient client = new AsyncHTTPClient(url);
			cached.client = client;
			client.tag = tag;
			client.doneCallback = delegate(AsyncHTTPClient c)
			{
				cached.client = null;
				if (!client.IsSuccess())
				{
					if (client.state != AsyncHTTPClient.State.ABORTED)
					{
						string text = string.Concat(new string[]
						{
							"error ",
							client.text,
							" ",
							client.state.ToString(),
							" '",
							url,
							"'"
						});
						if (ObjectListArea.s_Debug)
						{
							Debug.LogError(text);
						}
						else
						{
							Console.Write(text);
						}
					}
					else
					{
						AssetStorePreviewManager.Instance.m_Aborted++;
					}
				}
				else
				{
					if (cached.image != null)
					{
						UnityEngine.Object.DestroyImmediate(cached.image);
					}
					cached.image = c.texture;
					AssetStorePreviewManager.s_NeedsRepaint = true;
					AssetStorePreviewManager.Instance.m_Success++;
				}
			};
			return client;
		}

		private static void ExpireCacheEntries()
		{
			while (AssetStorePreviewManager.CacheFull)
			{
				string key = null;
				AssetStorePreviewManager.CachedAssetStoreImage cachedAssetStoreImage = null;
				foreach (KeyValuePair<string, AssetStorePreviewManager.CachedAssetStoreImage> current in AssetStorePreviewManager.CachedAssetStoreImages)
				{
					if (cachedAssetStoreImage == null || cachedAssetStoreImage.lastUsed > current.Value.lastUsed)
					{
						cachedAssetStoreImage = current.Value;
						key = current.Key;
					}
				}
				AssetStorePreviewManager.CachedAssetStoreImages.Remove(key);
				AssetStorePreviewManager.Instance.m_CacheRemove++;
				if (cachedAssetStoreImage == null)
				{
					Debug.LogError("Null entry found while removing cache entry");
					break;
				}
				if (cachedAssetStoreImage.client != null)
				{
					cachedAssetStoreImage.client.Abort();
					cachedAssetStoreImage.client = null;
				}
				if (cachedAssetStoreImage.image != null)
				{
					UnityEngine.Object.DestroyImmediate(cachedAssetStoreImage.image);
				}
			}
		}

		private static AssetStorePreviewManager.CachedAssetStoreImage RenderEntry(AssetStorePreviewManager.CachedAssetStoreImage cached, GUIStyle labelStyle, GUIStyle iconStyle)
		{
			AssetStorePreviewManager.CachedAssetStoreImage result;
			if (cached.label == null || cached.image == null)
			{
				result = cached;
			}
			else
			{
				Texture2D image = cached.image;
				cached.image = new Texture2D(cached.requestedWidth, cached.requestedWidth, TextureFormat.RGB24, false, true);
				AssetStorePreviewManager.ScaleImage(cached.requestedWidth, cached.requestedWidth, image, cached.image, iconStyle);
				UnityEngine.Object.DestroyImmediate(image);
				cached.label = null;
				AssetStorePreviewManager.Instance.m_ConvertedThisTick++;
				result = cached;
			}
			return result;
		}

		internal static void ScaleImage(int w, int h, Texture2D inimage, Texture2D outimage, GUIStyle bgStyle)
		{
			SavedRenderTargetState savedRenderTargetState = new SavedRenderTargetState();
			if (AssetStorePreviewManager.s_RenderTexture != null && (AssetStorePreviewManager.s_RenderTexture.width != w || AssetStorePreviewManager.s_RenderTexture.height != h))
			{
				UnityEngine.Object.DestroyImmediate(AssetStorePreviewManager.s_RenderTexture);
				AssetStorePreviewManager.s_RenderTexture = null;
			}
			if (AssetStorePreviewManager.s_RenderTexture == null)
			{
				AssetStorePreviewManager.s_RenderTexture = RenderTexture.GetTemporary(w, h, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				AssetStorePreviewManager.s_RenderTexture.hideFlags = HideFlags.HideAndDontSave;
			}
			RenderTexture renderTexture = AssetStorePreviewManager.s_RenderTexture;
			RenderTexture.active = renderTexture;
			Rect rect = new Rect(0f, 0f, (float)w, (float)h);
			EditorGUIUtility.SetRenderTextureNoViewport(renderTexture);
			GL.LoadOrtho();
			GL.LoadPixelMatrix(0f, (float)w, (float)h, 0f);
			ShaderUtil.rawViewportRect = new Rect(0f, 0f, (float)w, (float)h);
			ShaderUtil.rawScissorRect = new Rect(0f, 0f, (float)w, (float)h);
			GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
			Rect screenRect = rect;
			if (inimage.width > inimage.height)
			{
				float num = screenRect.height * ((float)inimage.height / (float)inimage.width);
				screenRect.height = (float)((int)num);
				screenRect.y += (float)((int)(num * 0.5f));
			}
			else if (inimage.width < inimage.height)
			{
				float num2 = screenRect.width * ((float)inimage.width / (float)inimage.height);
				screenRect.width = (float)((int)num2);
				screenRect.x += (float)((int)(num2 * 0.5f));
			}
			if (bgStyle != null && bgStyle.normal != null && bgStyle.normal.background != null)
			{
				Graphics.DrawTexture(rect, bgStyle.normal.background);
			}
			Graphics.DrawTexture(screenRect, inimage);
			outimage.ReadPixels(rect, 0, 0, false);
			outimage.Apply();
			outimage.hideFlags = HideFlags.HideAndDontSave;
			savedRenderTargetState.Restore();
		}

		public static bool CheckRepaint()
		{
			bool result = AssetStorePreviewManager.s_NeedsRepaint;
			AssetStorePreviewManager.s_NeedsRepaint = false;
			return result;
		}

		public static void AbortSize(int size)
		{
			AsyncHTTPClient.AbortByTag("previewSize-" + size.ToString());
			foreach (KeyValuePair<string, AssetStorePreviewManager.CachedAssetStoreImage> current in AssetStorePreviewManager.CachedAssetStoreImages)
			{
				if (current.Value.requestedWidth == size)
				{
					current.Value.requestedWidth = -1;
					current.Value.client = null;
				}
			}
		}

		public static void AbortOlderThan(double timestamp)
		{
			foreach (KeyValuePair<string, AssetStorePreviewManager.CachedAssetStoreImage> current in AssetStorePreviewManager.CachedAssetStoreImages)
			{
				AssetStorePreviewManager.CachedAssetStoreImage value = current.Value;
				if (value.lastUsed < timestamp && value.client != null)
				{
					value.requestedWidth = -1;
					value.client.Abort();
					value.client = null;
				}
			}
			AssetStorePreviewManager.Instance.m_ConvertedThisTick = 0;
		}
	}
}
