using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Percolation2D : MonoBehaviour
{
	public float updateSpeed
	{
		get	{ return _updateSpeed; }
		set { _updateSpeed = value; }
	}
	
	[SerializeField]
	private GameObject cube;
	
	[SerializeField]
	private int gridLength;

	[SerializeField]
	private Color closedBlockColor;
	[SerializeField]
	private Color openBlockColor;
	[SerializeField]
	private Color percolatingColor;
	[SerializeField]
	private Color percolatedColor;

	private int gridSize;

	private float blockSpacing = 1.01f;

	private float _updateSpeed;

	private Vector3 gridPosition;

	private GameObject[] visualizerBlock;
	private bool[] isOpen;

	private DisjointSet disjointSet;
	private bool percolates = false;

	private Text report;

	void Start ()
	{
		gridSize = gridLength * gridLength;

		report = GameObject.Find("Report").GetComponent<Text>();

		gridPosition = this.transform.position;
		float gridPositionAdjustment = (float)gridLength * blockSpacing / 2f;
		Debug.Log( gridPositionAdjustment );
		gridPosition.x -= gridPositionAdjustment;
		gridPosition.z -= gridPositionAdjustment;

		isOpen = new bool[gridSize];
		visualizerBlock = new GameObject[gridSize];
		disjointSet = new DisjointSet( gridSize );
		StartCoroutine( BuildGrid( ) );
	}

	private IEnumerator BuildGrid()
	{
		int blocksBuilt = 0;
		Vector3 cubePos;

		report.text = "Building new percolator.";

		for( int i = blocksBuilt; i < gridSize; i++ )
		{
			cubePos = Idx2WorldPos (i);
			cubePos.x = cubePos.x * blockSpacing + gridPosition.x;
			cubePos.z = cubePos.z * blockSpacing + gridPosition.z;

			//visualize the grid
			visualizerBlock[i] = (GameObject)Instantiate (cube, cubePos, Quaternion.Euler(0,0,0));
			visualizerBlock[i].transform.parent = this.transform;
			
			if (Time.timeScale == 0)
				yield return new WaitForFixedUpdate();
			else
				yield return new WaitForSeconds (_updateSpeed);
		}

		yield return new WaitForSeconds(0.5f);

		StartCoroutine (OpenSpaces());
	}

	private IEnumerator OpenSpaces ()
	{
		int numOpenBlocks = 0;

		while (!percolates)
		{
			numOpenBlocks++;
			report.text = string.Format("Opening new spaces... {0} open blocks ({1}% open).", numOpenBlocks, 100f * numOpenBlocks / gridSize);
			OpenNewBlock();
			
			// Check all blocks for percolation.
			// Can this be optimized? We know in this loop that only two possible distjoint sets may have been joined...
			for (var i = 0; i < gridSize; i++)
			{
				if (!isOpen[i] || !ConnectedToTop(i)) continue;

				// This block is connected to the top.
				visualizerBlock[i].GetComponent<Renderer>().material.color = percolatingColor;

				if (i >= gridLength) continue;

				// This block is located on the bottom: we've percolated.
				percolates = true;
			}

			if (percolates)
			{
				for (var i = 0; i < gridSize; i++)
				{
					if (!isOpen[i] ||
					    !ConnectedToTop(i) ||
					    !ConnectedToBottom(i))
						continue;

					visualizerBlock[i].GetComponent<Renderer>().material.color = percolatedColor;
				}
			}

			if (Time.timeScale == 0)
				yield return new WaitForFixedUpdate();
			else
				yield return new WaitForSeconds(_updateSpeed);
		}

		report.text = "Percolated at " + numOpenBlocks + " open blocks (" + (float)numOpenBlocks / (float)gridSize * 100 + "% open).";
	}

	private void OpenNewBlock()
	{
		// Open a new closed block.
		var newOpenBoxIdx = GetRandomClosedBox();
		isOpen[newOpenBoxIdx] = true;

		// Visualize it.
		visualizerBlock[newOpenBoxIdx].GetComponent<Renderer>().material.color = openBlockColor;

		// Join open neighbors.
		JoinNeighbors(newOpenBoxIdx);
	}

	private Vector3 Idx2WorldPos(int idx)
	{
		float x = idx % gridLength;
		float z = idx / gridLength;

		return new Vector3(x, 2, z);
	}

	private int GetRandomClosedBox()
	{
		int newIndex = -1;
		bool passed = false;

		while (!passed)
		{
			newIndex = (int)Random.Range(0, (gridSize));
			if (isOpen[newIndex] == false) passed = true;
		}
		return newIndex;
	}

	private void JoinNeighbors(int idx)
	{
		if (idx < gridSize - gridLength)
		{
			TryJoin(idx, idx + gridLength);
		}
		if (idx >= gridLength)
		{
			TryJoin(idx, idx - gridLength);
		}
		if (idx % gridLength > 0)
		{
			TryJoin(idx, idx - 1);
		}
		if (idx % gridLength < gridLength - 1)
		{
			TryJoin(idx, idx + 1);
		}
	}

	private void TryJoin(int a, int b)
	{
		if (isOpen[b] == true)
		{
			disjointSet.Join(a, b);
		}
	}

	private bool ConnectedToTop(int idx)
	{
		for (int i = gridSize - gridLength; i < gridSize; ++i)
		{
			if (isOpen[i] && disjointSet.AreConnected(idx, i))
			{
				return true;
			}
		}

		return false;
	}

	private bool ConnectedToBottom(int idx)
	{
		for (int i = 0; i < gridLength; ++i)
		{
			if (isOpen[i] && disjointSet.AreConnected(idx, i))
			{
				return true;
			}
		}

		return false;
	}
}
