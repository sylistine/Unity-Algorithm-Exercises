using UnityEngine;
using System.Collections;

public class QuickUnionGrid : ScriptableObject
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
	public QuickUnionGrid(int N)
	{
		int len = (int)Mathf.Pow(N, 2f);

		id = new int[len];
		for (int i = 0; i < len; i++) id[i] = i;
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

	public bool IsConnected (int a, int b)
	{
		return Root (a) == Root (b);
	}
}
