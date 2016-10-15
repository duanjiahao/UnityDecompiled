using System;
using System.Collections;
using UnityEngine;

[Serializable]
internal class SerializedStringTable
{
	[SerializeField]
	private string[] keys;

	[SerializeField]
	private int[] values;

	private Hashtable table;

	public Hashtable hashtable
	{
		get
		{
			this.SanityCheck();
			return this.table;
		}
	}

	public int Length
	{
		get
		{
			this.SanityCheck();
			return this.keys.Length;
		}
	}

	private void SanityCheck()
	{
		if (this.keys == null)
		{
			this.keys = new string[0];
			this.values = new int[0];
		}
		if (this.table == null)
		{
			this.table = new Hashtable();
			for (int i = 0; i < this.keys.Length; i++)
			{
				this.table[this.keys[i]] = this.values[i];
			}
		}
	}

	private void SynchArrays()
	{
		this.keys = new string[this.table.Count];
		this.values = new int[this.table.Count];
		this.table.Keys.CopyTo(this.keys, 0);
		this.table.Values.CopyTo(this.values, 0);
	}

	public void Set(string key, int value)
	{
		this.SanityCheck();
		this.table[key] = value;
		this.SynchArrays();
	}

	public void Set(string key)
	{
		this.Set(key, 0);
	}

	public bool Contains(string key)
	{
		this.SanityCheck();
		return this.table.Contains(key);
	}

	public int Get(string key)
	{
		this.SanityCheck();
		if (!this.table.Contains(key))
		{
			return -1;
		}
		return (int)this.table[key];
	}

	public void Remove(string key)
	{
		this.SanityCheck();
		if (this.table.Contains(key))
		{
			this.table.Remove(key);
		}
		this.SynchArrays();
	}
}
