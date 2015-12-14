using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatingMap : MonoBehaviour 
{
	[System.Serializable]

	public class Count
	{
		public int minimum;
		public int maximum;

		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	public MapGenerator mapGenerator;

	public Vector3[] gridPositions;
	public bool[] positionsBool;

	public int columns;
	public int rows;
	public int columnsXrows;

	public GameObject ground;

	public Count treeCount = new Count (1, 1);
	public GameObject[] treeTiles;

	void Start () 
	{
		mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
		columns = mapGenerator.size_x;
		rows = mapGenerator.size_z;
		columnsXrows = columns*rows;

		//InstantiateRandomScale(ground, 3, 3);
		gridPositions = new Vector3[columnsXrows];
		positionsBool = new bool[columnsXrows];
		InitialiseGrid();
		LayoutObjectAtRandom(treeTiles, 100, 100);
		CheckPositions();
	}

	void InitialiseGrid ()
	{
		int i = columnsXrows;
		for(int x = 0; x < columns; x++)
		{
			for(int z = 0; z < rows; z++)
			{
				i--;
				gridPositions[i] = new Vector3(x, 0.5f, z);
			}
		}
	}

	/*
	GameObject InstantiateRandomScale(GameObject ground, int minScale, int maxScale)
	{

		GameObject clone = Instantiate(ground) as GameObject;
		clone.transform.localScale = new Vector3(1f * Random.Range(minScale, maxScale),0.1f,1f * Random.Range(minScale, maxScale));

		columns = Mathf.RoundToInt(clone.transform.localScale.x);
		rows = Mathf.RoundToInt(clone.transform.localScale.z);
		columnsXrows = columns*rows;

		return clone;
	}
	*/
		
	Vector3 RandomPosition ()
	{
		int randomIndex = Random.Range (0, gridPositions.Length);

		Vector3 randomPosition = gridPositions[randomIndex];

		return randomPosition;
	}
		
	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
	{
		int objectCount = Random.Range (minimum, maximum+1);

		for(int i = 0; i < objectCount; i++)
		{
			Vector3 randomPosition = RandomPosition();

			while (Physics.CheckSphere(randomPosition, 0.4f))
			{
				randomPosition = RandomPosition();
			}
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];

				Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}

	void CheckPositions()
	{
		//for (int i = columnsXrows -1; i > -1; i--)
		for (int i = 0;  i < positionsBool.Length; i++)
		{
			if(Physics.CheckSphere(gridPositions[i], 0.4f))
			{
				positionsBool[i] = true;
			}
			else
			{
				positionsBool[i] = false;
			}
		}
	}

}