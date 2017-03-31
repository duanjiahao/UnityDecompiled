using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class CrashReport
	{
		private static List<CrashReport> internalReports;

		private static object reportsLock = new object();

		private readonly string id;

		public readonly DateTime time;

		public readonly string text;

		[CompilerGenerated]
		private static Comparison<CrashReport> <>f__mg$cache0;

		public static CrashReport[] reports
		{
			get
			{
				CrashReport.PopulateReports();
				object obj = CrashReport.reportsLock;
				CrashReport[] result;
				lock (obj)
				{
					result = CrashReport.internalReports.ToArray();
				}
				return result;
			}
		}

		public static CrashReport lastReport
		{
			get
			{
				CrashReport.PopulateReports();
				object obj = CrashReport.reportsLock;
				CrashReport result;
				lock (obj)
				{
					if (CrashReport.internalReports.Count > 0)
					{
						result = CrashReport.internalReports[CrashReport.internalReports.Count - 1];
						return result;
					}
				}
				result = null;
				return result;
			}
		}

		private CrashReport(string id, DateTime time, string text)
		{
			this.id = id;
			this.time = time;
			this.text = text;
		}

		private static int Compare(CrashReport c1, CrashReport c2)
		{
			long ticks = c1.time.Ticks;
			long ticks2 = c2.time.Ticks;
			int result;
			if (ticks > ticks2)
			{
				result = 1;
			}
			else if (ticks < ticks2)
			{
				result = -1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private static void PopulateReports()
		{
			object obj = CrashReport.reportsLock;
			lock (obj)
			{
				if (CrashReport.internalReports == null)
				{
					string[] reports = CrashReport.GetReports();
					CrashReport.internalReports = new List<CrashReport>(reports.Length);
					string[] array = reports;
					for (int i = 0; i < array.Length; i++)
					{
						string text = array[i];
						double value;
						string reportData = CrashReport.GetReportData(text, out value);
						DateTime dateTime = new DateTime(1970, 1, 1);
						DateTime dateTime2 = dateTime.AddSeconds(value);
						CrashReport.internalReports.Add(new CrashReport(text, dateTime2, reportData));
					}
					List<CrashReport> arg_AB_0 = CrashReport.internalReports;
					if (CrashReport.<>f__mg$cache0 == null)
					{
						CrashReport.<>f__mg$cache0 = new Comparison<CrashReport>(CrashReport.Compare);
					}
					arg_AB_0.Sort(CrashReport.<>f__mg$cache0);
				}
			}
		}

		public static void RemoveAll()
		{
			CrashReport[] reports = CrashReport.reports;
			for (int i = 0; i < reports.Length; i++)
			{
				CrashReport crashReport = reports[i];
				crashReport.Remove();
			}
		}

		public void Remove()
		{
			if (CrashReport.RemoveReport(this.id))
			{
				object obj = CrashReport.reportsLock;
				lock (obj)
				{
					CrashReport.internalReports.Remove(this);
				}
			}
		}

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetReports();

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetReportData(string id, out double secondsSinceUnixEpoch);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveReport(string id);
	}
}
