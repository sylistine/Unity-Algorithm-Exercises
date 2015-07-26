using UnityEngine;
using System.Collections;

public class DisjointSetSlowUnion : ScriptableObject
{
	int[] id;

	public int size
	{
		get
		{
			return id.Length;
		}
	}

	public DisjointSetSlowUnion (int N)
	{
		id = new int[N];
		for(int i = 0; i < N; i++)
		{
			id[i] = i;
		}
	}

	public void Join (int a, int b)
	{
		int rootA = id[a],
			rootB = id[b],
			len = size;

		for(int i = 0; i < len; i++)
		{
			if(id[i] == rootA) id[i] = rootB;
		}
	}

	public bool Find (int a, int b)
	{
		return id[a] == id[b];
	}
}
