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
	
	public GameObject cube;
	
	public int gridSqrtSize;
	
	// We want to use this to test that magic 0.59~ phase change value
	[Range (0, 1)]
	public float percentOpen;

	public Color closedBlockColor;
	public Color openBlockColor;
	public Color percolatedBlockColor;

	Vector3[] gridDir =
	{
		new Vector3(0, 0, 1),
		new Vector3(0, 0, -1),
		new Vector3(1, 0, 0),
		new Vector3(-1, 0, 0)
	};

	int gridSize;

	float blockSpacing = 1.01f;

	float _updateSpeed;
	
	Vector3 gridPosition;

	int[] openState;

	GameObject[] visualizerBlock;

	DisjointSet disjointSet;

	bool percolates = false;

	Text report;

	void Start ()
	{
		gridSize = gridSqrtSize * gridSqrtSize;
		
		gridPosition = this.transform.position;
		float gridPositionAdjustment = (float)gridSqrtSize * blockSpacing / 2f;
		Debug.Log (gridPositionAdjustment);
		gridPosition.x -= gridPositionAdjustment;
		gridPosition.z -= gridPositionAdjustment;

		openState = new int[gridSize];
		visualizerBlock = new GameObject[gridSize];
		disjointSet = new DisjointSet (gridSize);
		StartCoroutine (BuildGrid ());

		// join all 'top' indices to check percolation later
		int topRowFirstIndex = (int)(gridSize / gridSqrtSize) * (gridSqrtSize-1);
		for(int i = topRowFirstIndex; i < gridSize-1; i++)
		{
			disjointSet.Join (i, gridSize-1);
		}
	}
	
	Vector3 I2XZ (int index)
	{
		float x, y, z;
		
		x = index % gridSqrtSize;
		z = index / gridSqrtSize;

		return new Vector3 (x, 2, z);
	}

	int XZ2I (Vector3 xz)
	{
		return (int)(xz.z * gridSqrtSize + xz.x);
	}
	
	int getUnopenIndex ()
	{
		int newIndex = -1;
		bool passed = false;
		
		while(!passed)
		{
			newIndex = (int)Random.Range(0, (gridSize));
			if (openState[newIndex] == 1) passed = true;
		}
		return newIndex;
	}

	IEnumerator BuildGrid ()
	{
		int blocksBuilt = 0;
		Vector3 cubePos;
		
		report = GameObject.Find ("Report").GetComponent<Text> ();
		report.text = "Building new percolator.";

		for (int i = blocksBuilt; i < gridSize; i++)
		{
			cubePos = I2XZ (i);
			cubePos.x = cubePos.x*blockSpacing + gridPosition.x;
			cubePos.z = cubePos.z*blockSpacing + gridPosition.z;

			//record change
			openState[i] = 1;

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

	IEnumerator OpenSpaces ()
	{
		int totalNumOpenSpaces = (int)(gridSize * percentOpen);
		int numOpenBlocks = 0;
		int newUnopenedIndex;
		int connectingIndex;
		Vector3 oldXZ;
		Vector3 newXZ;

		while (numOpenBlocks < totalNumOpenSpaces)
		{
			newUnopenedIndex = getUnopenIndex();

			//record change
			openState[newUnopenedIndex] = 0;

			//visualize it
			visualizerBlock[newUnopenedIndex].GetComponent<Renderer>().material.color = openBlockColor;

			//update the union grid
			for (int i = 0; i < 4; i++)
			{
				oldXZ = I2XZ (newUnopenedIndex);
				newXZ = oldXZ + gridDir[i];

				if(newXZ.x >= 0 && newXZ.x < gridSqrtSize && newXZ.z >= 0 && newXZ.z < gridSqrtSize)
				{
					connectingIndex = XZ2I(newXZ);
					if (openState[connectingIndex] == 0)
					{
						disjointSet.Join (newUnopenedIndex, connectingIndex);
					}
				}
			}

			report.text = "Opening new spaces... " + numOpenBlocks + " open blocks (" + (float)numOpenBlocks / (float)gridSize * 100 + "% open).";
			
			//check all blocks for percolation
			for (int i = 0; i < gridSize; i++)
			{
				if (openState[i] == 0)
				{
					if (disjointSet.Find(i, gridSize-1))
					{
						visualizerBlock[i].GetComponent<Renderer>().material.color = percolatedBlockColor;

						if (i < gridSqrtSize)
						{
							percolates = true;
						}
					}
				}
			}

			if (percolates)
			{
				report.text = "Percolated at " + numOpenBlocks + " open blocks (" + (float)numOpenBlocks / (float)gridSize * 100 + "% open).";
				numOpenBlocks = totalNumOpenSpaces;
			}

			numOpenBlocks++;

			if (Time.timeScale == 0)
				yield return new WaitForFixedUpdate();
			else
				yield return new WaitForSeconds (_updateSpeed);
		}
	}
}
