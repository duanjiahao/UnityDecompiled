using System;

namespace UnityEngine
{
	public class Logger : ILogger, ILogHandler
	{
		private const string kNoTagFormat = "{0}";

		private const string kTagFormat = "{0}: {1}";

		public ILogHandler logHandler
		{
			get;
			set;
		}

		public bool logEnabled
		{
			get;
			set;
		}

		public LogType filterLogType
		{
			get;
			set;
		}

		private Logger()
		{
		}

		public Logger(ILogHandler logHandler)
		{
			this.logHandler = logHandler;
			this.logEnabled = true;
			this.filterLogType = LogType.Log;
		}

		public bool IsLogTypeAllowed(LogType logType)
		{
			bool result;
			if (this.logEnabled)
			{
				if (logType == LogType.Exception)
				{
					result = true;
					return result;
				}
				if (this.filterLogType != LogType.Exception)
				{
					result = (logType <= this.filterLogType);
					return result;
				}
			}
			result = false;
			return result;
		}

		private static string GetString(object message)
		{
			return (message == null) ? "Null" : message.ToString();
		}

		public void Log(LogType logType, object message)
		{
			if (this.IsLogTypeAllowed(logType))
			{
				this.logHandler.LogFormat(logType, null, "{0}", new object[]
				{
					Logger.GetString(message)
				});
			}
		}

		public void Log(LogType logType, object message, Object context)
		{
			if (this.IsLogTypeAllowed(logType))
			{
				this.logHandler.LogFormat(logType, context, "{0}", new object[]
				{
					Logger.GetString(message)
				});
			}
		}

		public void Log(LogType logType, string tag, object message)
		{
			if (this.IsLogTypeAllowed(logType))
			{
				this.logHandler.LogFormat(logType, null, "{0}: {1}", new object[]
				{
					tag,
					Logger.GetString(message)
				});
			}
		}

		public void Log(LogType logType, string tag, object message, Object context)
		{
			if (this.IsLogTypeAllowed(logType))
			{
				this.logHandler.LogFormat(logType, context, "{0}: {1}", new object[]
				{
					tag,
					Logger.GetString(message)
				});
			}
		}

		public void Log(object message)
		{
			if (this.IsLogTypeAllowed(LogType.Log))
			{
				this.logHandler.LogFormat(LogType.Log, null, "{0}", new object[]
				{
					Logger.GetString(message)
				});
			}
		}

		public void Log(string tag, object message)
		{
			if (this.IsLogTypeAllowed(LogType.Log))
			{
				this.logHandler.LogFormat(LogType.Log, null, "{0}: {1}", new object[]
				{
					tag,
					Logger.GetString(message)
				});
			}
		}

		public void Log(string tag, object message, Object context)
		{
			if (this.IsLogTypeAllowed(LogType.Log))
			{
				this.logHandler.LogFormat(LogType.Log, context, "{0}: {1}", new object[]
				{
					tag,
					Logger.GetString(message)
				});
			}
		}

		public void LogWarning(string tag, object message)
		{
			if (this.IsLogTypeAllowed(LogType.Warning))
			{
				this.logHandler.LogFormat(LogType.Warning, null, "{0}: {1}", new object[]
				{
					tag,
					Logger.GetString(message)
				});
			}
		}

		public void LogWarning(string tag, object message, Object context)
		{
			if (this.IsLogTypeAllowed(LogType.Warning))
			{
				this.logHandler.LogFormat(LogType.Warning, context, "{0}: {1}", new object[]
				{
					tag,
					Logger.GetString(message)
				});
			}
		}

		public void LogError(string tag, object message)
		{
			if (this.IsLogTypeAllowed(LogType.Error))
			{
				this.logHandler.LogFormat(LogType.Error, null, "{0}: {1}", new object[]
				{
					tag,
					Logger.GetString(message)
				});
			}
		}

		public void LogError(string tag, object message, Object context)
		{
			if (this.IsLogTypeAllowed(LogType.Error))
			{
				this.logHandler.LogFormat(LogType.Error, context, "{0}: {1}", new object[]
				{
					tag,
					Logger.GetString(message)
				});
			}
		}

		public void LogFormat(LogType logType, string format, params object[] args)
		{
			if (this.IsLogTypeAllowed(logType))
			{
				this.logHandler.LogFormat(logType, null, format, args);
			}
		}

		public void LogException(Exception exception)
		{
			if (this.logEnabled)
			{
				this.logHandler.LogException(exception, null);
			}
		}

		public void LogFormat(LogType logType, Object context, string format, params object[] args)
		{
			if (this.IsLogTypeAllowed(logType))
			{
				this.logHandler.LogFormat(logType, context, format, args);
			}
		}

		public void LogException(Exception exception, Object context)
		{
			if (this.logEnabled)
			{
				this.logHandler.LogException(exception, context);
			}
		}
	}
}
