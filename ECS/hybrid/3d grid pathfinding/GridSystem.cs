using System;
using UnityEngine;

public class GridSystem<T>
{
	T[,,] grid;
	GameObject[,,] debugGridVisuals;
	public int X { get; private set; }
	public int Y { get; private set; }
	public int Z { get; private set; }
	public float CellSize { get; private set; }
	public Vector3 Origin { get; private set; }
	public Action<int, int, int> ValueChanged;

	public GridSystem(Vector3 origin, int x, int y, int z, float cellSize)
	{
		X = x;
		Y = y;
		Z = z;
		CellSize = cellSize;
		grid = new T[x, y, z];
		Origin = origin;
		debugGridVisuals = new GameObject[x, y, z];
	}

	public Vector3 GetWorldPosition(int x, int y, int z)
	{
		return new Vector3(x, y, z) * CellSize + Origin;
	}

	public void Set(int x, int y, int z, T value)
	{
		if (x >= 0 && x < X &&
			y >= 0 && y < Y &&
			z >= 0 && z < Z)
		{
			grid[x, y, z] = value;
			ValueChanged(x, y, z);
		}
	}

	Vector3Int GetXYZ(Vector3 worldPosition)
	{
		var result = Vector3Int.zero;
		var difference = worldPosition - Origin;

		result.x = Mathf.FloorToInt(difference.x / CellSize);
		result.y = Mathf.FloorToInt(difference.y / CellSize);
		result.z = Mathf.FloorToInt(difference.z / CellSize);
		return result;
	}

	public void SetDebugObject(int x, int y, int z, GameObject obj)
	{
		if (x >= 0 && x < X &&
			y >= 0 && y < Y &&
			z >= 0 && z < Z)
		{
			debugGridVisuals[x, y, z] = obj;
			obj.transform.position = GetWorldPosition(x, y, z) + new Vector3(CellSize, CellSize, CellSize) * 0.5f;
		}
	}

	public void Set(Vector3 worldPosition, T value)
	{
		var xyz = GetXYZ(worldPosition);
		Set(xyz.x, xyz.y, xyz.z, value);
	}

	public T Get(int x, int y, int z)
	{
		if (x >= 0 && x < X &&
			y >= 0 && y < Y &&
			z >= 0 && z < Z)
		{
			return grid[x, y, z];
		}
		else return default;
	}

	public T Get(Vector3 worldPosition)
	{
		var xyz = GetXYZ(worldPosition);
		return Get(xyz.x, xyz.y, xyz.z);
	}
}
