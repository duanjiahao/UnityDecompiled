using System;

namespace UnityEngine
{
	[Obsolete("CalendarUnit is deprecated. Please use iOS.CalendarUnit instead (UnityUpgradable) -> UnityEngine.iOS.CalendarUnit", true)]
	public enum CalendarUnit
	{
		Era,
		Year,
		Month,
		Day,
		Hour,
		Minute,
		Second,
		Week,
		Weekday,
		WeekdayOrdinal,
		Quarter
	}
}
