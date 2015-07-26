using UnityEngine;
using System.Collections;

public class QuickFindGrid : ScriptableObject
{
	int[] id;

	public int size
	{
		get
		{
			return id.Length;
		}
	}

	public QuickFindGrid (int N)
	{
		int len = (int)Mathf.Pow (N, 2);

		id = new int[len];
		for(int i = 0; i < len; i++)
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

	public bool IsConnected (int a, int b)
	{
		return id[a] == id[b];
	}
}
