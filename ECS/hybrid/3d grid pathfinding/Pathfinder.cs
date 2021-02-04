using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct PathNode
{
    public int x;
    public int y;
    public int z;
    public int index;
    public int fCost;
    public int hCost;
    public int gCost;
    public int prevIndex;
    public bool accessible;

    public void GetFCost() => fCost = gCost + hCost;
}

public class Pathfinder : MonoBehaviour
{
    const int MOVE_STRAIGHT_COST = 10;
    const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] int3 gridSize;
    [SerializeField] int3 from;
    [SerializeField] int3 to;

	private void Start()
	{
        Find(from, to);
	}

	void Find(int3 start, int3 end)
    {
        var nodes = new NativeArray<PathNode>(gridSize.x * gridSize.y * gridSize.z, Allocator.Temp);
        int indexIter = 0;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
                for (int z = 0; z < gridSize.z; z++)
                {
                    var node = new PathNode
                    {
                        x = x,
                        y = y,
                        z = z,
                        index = indexIter++,
						gCost = int.MaxValue,
						hCost = GetDistanceCost(new int3(x, y, z), end),
                        accessible = true,
                        prevIndex = -1
                    };
					node.GetFCost();
                    nodes[node.index] = node;
                }
        }

        for (int i = 0; i < nodes.Length; i++)
            if (nodes[i].x == end.x && nodes[i].y == end.y && nodes[i].z == end.z)
                print(i);

        var endIndex = GetIndex(nodes, end);
        var startNode = nodes[GetIndex(nodes, start)];
        startNode.gCost = 0;
        startNode.GetFCost();
        nodes[startNode.index] = startNode;

        var openList = new NativeList<int>(Allocator.Temp);
        var closedList = new NativeList<int>(Allocator.Temp);

        openList.Add(startNode.index);

        while (openList.Length > 0)
        {
            var currentIndex = GetLowestFCostIndex(openList, nodes);
            var currentNode = nodes[currentIndex];

            if (currentIndex == endIndex) break;

            // remove current node from the list
            for (int i = 0; i < openList.Length; i++)
            {
                if (openList[i] == currentIndex)
                {
                    openList.RemoveAtSwapBack(i);
                    break;
                }
            }
            closedList.Add(currentIndex);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x == 0 && y == 0 && z == 0) continue; // skip itself
                        var neighborPosition = new int3(currentNode.x + x, currentNode.y + y, currentNode.z + z);
                        if (!IsValidCell(neighborPosition)) continue; // not inside grid

                        var neighborIndex = GetIndex(nodes, neighborPosition);
                        if (closedList.Contains(neighborIndex)) continue; // already checked

                        var neighborNode = nodes[neighborIndex];
                        if (!neighborNode.accessible) continue; // inaccessible

                        var tentativeGCost = currentNode.gCost + GetDistanceCost(currentNode, neighborNode);
                        if (tentativeGCost < neighborNode.gCost)
                        {
                            neighborNode.prevIndex = currentIndex;
                            neighborNode.gCost = tentativeGCost;
                            neighborNode.GetFCost();
                            nodes[neighborIndex] = neighborNode;

                            if (!openList.Contains(neighborNode.index))
                                openList.Add(neighborNode.index);
                        }
                    }
            }
        }

        var endNode = nodes[endIndex];
        //if (endNode.prevIndex == -1)

        var path = GetPath(nodes, endNode);
        for (int i = 0; i < path.Length; i++) print($"{path[i].x}, {path[i].y}, {path[i].z}");
        path.Dispose();

        nodes.Dispose();
        openList.Dispose();
        closedList.Dispose();
    }

    NativeList<int3> GetPath(NativeArray<PathNode> nodes, PathNode end)
    {
        if (end.prevIndex == -1) return new NativeList<int3>(Allocator.Temp); // no path found

		var path = new NativeList<int3>(Allocator.Temp)
		{
			new int3(end.x, end.y, end.z)
		};

		var currentNode = end;
        while (currentNode.prevIndex != -1)
        {
            var prevNode = nodes[currentNode.prevIndex];
            path.Add(new int3(prevNode.x, prevNode.y, prevNode.z));
            currentNode = prevNode;
        }

        return path;
    }

    bool IsValidCell(int3 position)
    {
        return position.x >= 0 && position.y >= 0 && position.z >= 0 &&
            position.x < gridSize.x && position.y < gridSize.y && position.z < gridSize.z;
    }

    int GetIndex(NativeArray<PathNode> nodes, int3 position) // => position.x + position.y + position.z * gridSize.x * gridSize.y;
    {
        for (int i = 0; i < nodes.Length; i++)
            if (position.x == nodes[i].x && position.y == nodes[i].y && position.z == nodes[i].z)
                return i;
        return -1;
    }

    int GetDistanceCost(int3 a, int3 b)
    {
        var x = math.abs(a.x - b.x);
        var y = math.abs(a.y - b.y);
        var z = math.abs(a.z - b.z);
        var remaining = math.abs(x - y - z);

        return MOVE_DIAGONAL_COST * math.min(math.min(x, y), z) + MOVE_STRAIGHT_COST * remaining;
    }

    int GetDistanceCost(PathNode a, PathNode b) => GetDistanceCost(new int3(a.x, a.y, a.z), new int3(b.x, b.y, b.z));

    int GetLowestFCostIndex(NativeList<int> openList, NativeArray<PathNode> nodes)
    {
        var lowestCostNode = nodes[openList[0]];
        for (int i = 1; i < openList.Length; i++)
        {
            var testNode = nodes[openList[i]];
            if (testNode.fCost < lowestCostNode.fCost)
                lowestCostNode = testNode;
        }

        return lowestCostNode.index;
    }
}
