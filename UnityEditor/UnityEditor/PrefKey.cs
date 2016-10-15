using System;
using UnityEngine;

namespace UnityEditor
{
	internal class PrefKey : IPrefType
	{
		private bool m_Loaded;

		private string m_name;

		private Event m_event;

		private string m_Shortcut;

		private string m_DefaultShortcut;

		public string Name
		{
			get
			{
				this.Load();
				return this.m_name;
			}
		}

		public Event KeyboardEvent
		{
			get
			{
				this.Load();
				return this.m_event;
			}
			set
			{
				this.Load();
				this.m_event = value;
			}
		}

		public bool activated
		{
			get
			{
				this.Load();
				return Event.current.Equals(this) && !GUIUtility.textFieldInput;
			}
		}

		public PrefKey()
		{
			this.m_Loaded = true;
		}

		public PrefKey(string name, string shortcut)
		{
			this.m_name = name;
			this.m_Shortcut = shortcut;
			this.m_DefaultShortcut = shortcut;
			Settings.Add(this);
			this.m_Loaded = false;
		}

		public void Load()
		{
			if (this.m_Loaded)
			{
				return;
			}
			this.m_Loaded = true;
			this.m_event = Event.KeyboardEvent(this.m_Shortcut);
			PrefKey prefKey = Settings.Get<PrefKey>(this.m_name, this);
			this.m_name = prefKey.Name;
			this.m_event = prefKey.KeyboardEvent;
		}

		public string ToUniqueString()
		{
			this.Load();
			return string.Concat(new object[]
			{
				this.m_name,
				";",
				(!this.m_event.alt) ? string.Empty : "&",
				(!this.m_event.command) ? string.Empty : "%",
				(!this.m_event.shift) ? string.Empty : "#",
				(!this.m_event.control) ? string.Empty : "^",
				this.m_event.keyCode
			});
		}

		public void FromUniqueString(string s)
		{
			this.Load();
			int num = s.IndexOf(";");
			if (num < 0)
			{
				Debug.LogError("Malformed string in Keyboard preferences");
				return;
			}
			this.m_name = s.Substring(0, num);
			this.m_event = Event.KeyboardEvent(s.Substring(num + 1));
		}

		internal void ResetToDefault()
		{
			this.Load();
			this.m_event = Event.KeyboardEvent(this.m_DefaultShortcut);
		}

		public static implicit operator Event(PrefKey pkey)
		{
			pkey.Load();
			return pkey.m_event;
		}
	}
}
