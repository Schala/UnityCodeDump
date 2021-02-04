using UnityEngine;

public class GridTest : MonoBehaviour
{
    GridSystem<int> grid;
    [SerializeField] int x = 10;
    [SerializeField] int y = 10;
    [SerializeField] int z = 10;
    [SerializeField] float cellSize = 10f;
    [SerializeField] Material openPathMaterial;
    [SerializeField] Material closedPathMaterial;
    [SerializeField] Material inaccessiblePathMaterial;
    [SerializeField] GameObject debugVisualPrefab = null;

	private void Awake()
	{
        grid = new GridSystem<int>(transform.position, x, y, z, cellSize);
	}

	void Start()
    {
        if (debugVisualPrefab == null) return;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
                for (int k = 0; k < z; k++)
                    grid.SetDebugObject(i, j, k, Instantiate(debugVisualPrefab));
        }
    }

    void Update()
    {
        
    }
}
