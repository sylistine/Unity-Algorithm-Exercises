using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Percolation3D : MonoBehaviour
{
	public float updateSpeed
	{
		get	{ return _updateSpeed; }
		set { _updateSpeed = value; }
	}
	
	public GameObject cube;
	
	public int gridCbrtSize;
	
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
		new Vector3(-1, 0, 0),
		new Vector3(0, 1, 0),
		new Vector3(0, -1, 0)
	};
	
	int gridSize;
	
	float blockSpacing = 1f;
	
	float _updateSpeed;
	
	Vector3 gridPosition;
	
	int[] openState;
	
	GameObject[] visualizerBlock;
	
	DisjointSet disjointSet;
	
	bool percolates = false;
	
	Text report;
	
	void Start ()
	{
		gridSize = gridCbrtSize * gridCbrtSize * gridCbrtSize;
		
		gridPosition = this.transform.position;
		float gridPositionAdjustment = (float)gridCbrtSize * blockSpacing / 2f;
		Debug.Log (gridPositionAdjustment);
		gridPosition.x -= gridPositionAdjustment;
		gridPosition.z -= gridPositionAdjustment;
		
		openState = new int[gridSize];
		visualizerBlock = new GameObject[gridSize];
		disjointSet = new DisjointSet (gridSize);
		StartCoroutine (BuildGrid ());
		
		// join all 'top' indices to check percolation later
		int topRowFirstIndex = (int)(gridSize / gridCbrtSize) * (gridCbrtSize-1);
		for(int i = topRowFirstIndex; i < gridSize-1; i++)
		{
			disjointSet.Join (i, gridSize-1);
		}
	}
	
	Vector3 I2XYZ (int index)
	{
		float x, y, z;
		y = (int)(index / (gridCbrtSize * gridCbrtSize));
		z = (int)((index - y * gridCbrtSize * gridCbrtSize) / gridCbrtSize);
		x = index % gridCbrtSize;

		return new Vector3 (x, y, z);
	}
	
	int XYZ2I (Vector3 xyz)
	{
		return (int)(xyz.y * Mathf.Pow (gridCbrtSize, 2f) + xyz.z * gridCbrtSize + xyz.x);
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
			cubePos = I2XYZ (i);
			cubePos.y = cubePos.y * blockSpacing + gridPosition.y + 0.5f;
			cubePos.z = cubePos.z * blockSpacing + gridPosition.z;
			cubePos.x = cubePos.x * blockSpacing + gridPosition.x;
			
			//record change
			openState[i] = 1;
			
			//visualize the grid
			visualizerBlock[i] = (GameObject)Instantiate (cube, cubePos, Quaternion.Euler(0,0,0));
			visualizerBlock[i].transform.parent = this.transform;
			visualizerBlock[i].SetActive(false);
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
			visualizerBlock[newUnopenedIndex].SetActive(true);
			visualizerBlock[newUnopenedIndex].GetComponent<Renderer>().material.color = openBlockColor;
			
			//update the union grid
			for (int i = 0; i < 6; i++)
			{
				oldXZ = I2XYZ (newUnopenedIndex);
				newXZ = oldXZ + gridDir[i];
				
				if(newXZ.x >= 0 && newXZ.x < gridCbrtSize && newXZ.z >= 0 && newXZ.z < gridCbrtSize && newXZ.y >= 0 && newXZ.y < gridCbrtSize)
				{
					connectingIndex = XYZ2I(newXZ);
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
						
						if (i < gridCbrtSize * gridCbrtSize)
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
