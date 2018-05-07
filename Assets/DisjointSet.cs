using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a completely disconnected tree of values 0 to N-1.
/// The values themselves are not stored in the set, but are implicitly referenced by index.
/// </summary>
public class DisjointSet : ScriptableObject
{
	public readonly int Size;

	int[] parent;
	int[] treeSize;
	
	public DisjointSet( int N )
	{
		Size = N;
		parent = new int[N];
		treeSize = new int[N];

		for( int i = 0; i < N; i++ )
		{
			parent[i] = i;
			treeSize[i] = 1;
		}
	}

	/// <summary>
	///Join two parts of the set by their roots.
	/// </summary>
	public void Join( int a, int b )
	{
		int i = Root( a );
		int j = Root( b );

		// Determine which root will be the parent by it's total size.
		// The smaller one becomes a child and helps keep the tree flat.
		if( treeSize[i] < treeSize[j] )
		{
			parent[i] = j;
			treeSize[j] += treeSize[i];
		}
		else
		{
			parent[j] = i;
			treeSize[i] += treeSize[j];
		}
	}
	
	/// <summary>
	/// Finds whether or not two values in the set have been joined.
	/// </summary>
	/// <returns>True when the two values have the same root.</returns>
	public bool AreConnected( int a, int b )
	{
		return Root( a ) == Root( b );
	}

	public int Root(int i)
	{
		// A root is defined as an value who's parent is itself.
		while (parent[i] != i)
		{
			// Not only are we searching for the root,
			// but since the root is the only thing we care about,
			// we can flatten the tree every time we search for it.

			// This line moves the value's parent one "generation" closer to it's root in the tree.
			parent[i] = parent[parent[i]];

			// Then we check whether that new index is it's own root.
			i = parent[i];
		}
		return i;
	}
}
