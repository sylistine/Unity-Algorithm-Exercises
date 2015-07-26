using UnityEngine;
using System.Collections;

public class DisjointSetLazyUnion : ScriptableObject
{
	int[] id;

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
	public DisjointSetLazyUnion(int N)
	{
		id = new int[N];
		for (int i = 0; i < N; i++) id[i] = i;
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
		id[i] = j;
	}

	public bool Find (int a, int b)
	{
		return Root (a) == Root (b);
	}
}
