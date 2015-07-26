using UnityEngine;
using System.Collections;

public class DisjointSet : ScriptableObject
{
	
	int[] id;
	int[] sz;
	
	public int size
	{
		get
		{
			return id.Length;
		}
	}
	
	/*
	 * Constructor
	 */
	public DisjointSet(int N)
	{
		id = new int[N];
		sz = new int[N];

		for (int i = 0; i < N; i++)
		{
			id[i] = i;
			sz[i] = 1;
		}
	}
	
	private int Root (int i)
	{
		while (id[i] != i) i = id[i];
		return i;
	}
	
	public void Join (int a, int b)
	{
		int i = Root (a);
		int j = Root (b);
		if (sz[i] < sz[j]) { id[i] = j; sz[j] += sz[i]; }
		else               { id[j] = i; sz[i] += sz[j]; }
	}
	
	public bool Find (int a, int b)
	{
		return Root (a) == Root (b);
	}
}
