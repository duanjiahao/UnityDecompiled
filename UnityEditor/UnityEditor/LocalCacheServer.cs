using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEditor.Utils;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal class LocalCacheServer : ScriptableSingleton<LocalCacheServer>
	{
		[SerializeField]
		public string path;

		[SerializeField]
		public int port;

		[SerializeField]
		public ulong size;

		[SerializeField]
		public int pid = -1;

		[SerializeField]
		public string time;

		public const string SizeKey = "LocalCacheServerSize";

		public const string PathKey = "LocalCacheServerPath";

		public const string CustomPathKey = "LocalCacheServerCustomPath";

		public static string GetCacheLocation()
		{
			string @string = EditorPrefs.GetString("LocalCacheServerPath");
			bool @bool = EditorPrefs.GetBool("LocalCacheServerCustomPath");
			string result = @string;
			if (!@bool || string.IsNullOrEmpty(@string))
			{
				result = Paths.Combine(new string[]
				{
					OSUtil.GetDefaultCachePath(),
					"CacheServer"
				});
			}
			return result;
		}

		public static void CreateCacheDirectory()
		{
			string cacheLocation = LocalCacheServer.GetCacheLocation();
			if (!Directory.Exists(cacheLocation))
			{
				Directory.CreateDirectory(cacheLocation);
			}
		}

		private void Create(int _port, ulong _size)
		{
			string text = Paths.Combine(new string[]
			{
				EditorApplication.applicationContentsPath,
				"Tools",
				"nodejs"
			});
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				text = Paths.Combine(new string[]
				{
					text,
					"node.exe"
				});
			}
			else
			{
				text = Paths.Combine(new string[]
				{
					text,
					"bin",
					"node"
				});
			}
			LocalCacheServer.CreateCacheDirectory();
			this.path = LocalCacheServer.GetCacheLocation();
			string text2 = Paths.Combine(new string[]
			{
				EditorApplication.applicationContentsPath,
				"Tools",
				"CacheServer",
				"main.js"
			});
			ProcessStartInfo startInfo = new ProcessStartInfo(text)
			{
				Arguments = string.Concat(new object[]
				{
					"\"",
					text2,
					"\" --port ",
					_port,
					" --path ",
					this.path,
					" --nolegacy --monitor-parent-process ",
					Process.GetCurrentProcess().Id,
					" --silent --size ",
					_size
				}),
				UseShellExecute = false,
				CreateNoWindow = true
			};
			Process process = new Process();
			process.StartInfo = startInfo;
			process.Start();
			this.port = _port;
			this.pid = process.Id;
			this.size = _size;
			this.time = process.StartTime.ToString();
			this.Save(true);
		}

		public static int GetRandomUnusedPort()
		{
			TcpListener tcpListener = new TcpListener(IPAddress.Any, 0);
			tcpListener.Start();
			int result = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
			tcpListener.Stop();
			return result;
		}

		public static bool PingHost(string host, int port, int timeout)
		{
			bool result;
			try
			{
				using (TcpClient tcpClient = new TcpClient())
				{
					IAsyncResult asyncResult = tcpClient.BeginConnect(host, port, null, null);
					asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double)timeout));
					result = tcpClient.Connected;
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool WaitForServerToComeAlive(int port)
		{
			bool result;
			for (int i = 0; i < 500; i++)
			{
				if (LocalCacheServer.PingHost("localhost", port, 10))
				{
					Console.WriteLine("Server Came alive after " + i * 10 + "ms");
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public static void Kill()
		{
			if (ScriptableSingleton<LocalCacheServer>.instance.pid != -1)
			{
				try
				{
					Process processById = Process.GetProcessById(ScriptableSingleton<LocalCacheServer>.instance.pid);
					processById.Kill();
					ScriptableSingleton<LocalCacheServer>.instance.pid = -1;
				}
				catch
				{
				}
			}
		}

		public static void CreateIfNeeded()
		{
			Process process = null;
			try
			{
				process = Process.GetProcessById(ScriptableSingleton<LocalCacheServer>.instance.pid);
			}
			catch
			{
			}
			ulong num = (ulong)((long)EditorPrefs.GetInt("LocalCacheServerSize", 10) * 1024L * 1024L * 1024L);
			if (process != null && process.StartTime.ToString() == ScriptableSingleton<LocalCacheServer>.instance.time)
			{
				if (ScriptableSingleton<LocalCacheServer>.instance.size == num && ScriptableSingleton<LocalCacheServer>.instance.path == LocalCacheServer.GetCacheLocation())
				{
					LocalCacheServer.CreateCacheDirectory();
					return;
				}
				LocalCacheServer.Kill();
			}
			ScriptableSingleton<LocalCacheServer>.instance.Create(LocalCacheServer.GetRandomUnusedPort(), num);
			LocalCacheServer.WaitForServerToComeAlive(ScriptableSingleton<LocalCacheServer>.instance.port);
		}

		public static void Setup()
		{
			if (EditorPrefs.GetInt("CacheServerMode") == 0)
			{
				LocalCacheServer.CreateIfNeeded();
			}
			else
			{
				LocalCacheServer.Kill();
			}
		}

		[UsedByNativeCode]
		public static int GetLocalCacheServerPort()
		{
			LocalCacheServer.Setup();
			return ScriptableSingleton<LocalCacheServer>.instance.port;
		}

		public static void Clear()
		{
			LocalCacheServer.Kill();
			string cacheLocation = LocalCacheServer.GetCacheLocation();
			if (Directory.Exists(cacheLocation))
			{
				Directory.Delete(cacheLocation, true);
			}
		}

		public static bool CheckCacheLocationExists()
		{
			return Directory.Exists(LocalCacheServer.GetCacheLocation());
		}

		public static bool CheckValidCacheLocation(string path)
		{
			bool result;
			if (Directory.Exists(path))
			{
				string[] fileSystemEntries = Directory.GetFileSystemEntries(path);
				string[] array = fileSystemEntries;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					string text2 = Path.GetFileName(text).ToLower();
					if (text2.Length != 2)
					{
						if (!(text2 == "temp"))
						{
							if (!(text2 == ".ds_store"))
							{
								if (!(text2 == "desktop.ini"))
								{
									result = false;
									return result;
								}
							}
						}
					}
				}
			}
			result = true;
			return result;
		}
	}
}
