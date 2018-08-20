using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public bool displayGridGizmos;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	float nodeRadius;
	int penalty = 100000;
	LayerMask walkableMask;

    public Node[,] grid { get; set; }
    public Node[,] grid_1 { get; set; }
    public Node[,] grid_2 { get; set; }
    public Node[,] grid_3 { get; set; }
    public Node[,] grid_nsz { get; set; }

    float nodeDiameter;
	int gridSizeX, gridSizeY;


	void Awake()
    {
        nodeRadius = config.nodeRadius;

        nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);

        if (config.mode.Equals("Preknown"))
        {
            CreateMappedGrid();
        }
        else
        {
            CreateGrid();
        }

    }

    public int MaxSize
    {
		get
        {
			return gridSizeX * gridSizeY;
		}
	}

	void CreateGrid()
    {

		grid = new Node[gridSizeX,gridSizeY];

        grid_1 = new Node[gridSizeX, gridSizeY];
        grid_2 = new Node[gridSizeX, gridSizeY];
        grid_3 = new Node[gridSizeX, gridSizeY];

        grid_nsz = new Node[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {

                int movementPenalty = 0;                
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = true;   
                                        
                grid[x, y] = new Node(walkable,worldPoint, x,y, movementPenalty);
                
                grid_1[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                grid_2[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                grid_3[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);

                grid_nsz[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
		}
	}

    void CreateMappedGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        grid_1 = new Node[gridSizeX, gridSizeY];
        grid_2 = new Node[gridSizeX, gridSizeY];
        grid_3 = new Node[gridSizeX, gridSizeY];

        grid_nsz = new Node[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                int movementPenalty = 0;
                
                Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                
                if (!walkable)
                {
                    movementPenalty += penalty;
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);

                grid_1[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                grid_2[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                grid_3[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);

                grid_nsz[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
        }        
    }

    public void UpdateGrid(Vector3 obstacle_point, int id, bool walkable)
    {
        int x, y;
        int movementPenalty = 0;

        x = Mathf.RoundToInt((obstacle_point.x + gridWorldSize.x / 2) / (nodeDiameter) - 0.5f);
        y = Mathf.RoundToInt((obstacle_point.z + gridWorldSize.y / 2) / (nodeDiameter) - 0.5f);
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius)
                                        + Vector3.forward * (y * nodeDiameter + nodeRadius);

        grid[x, y] = new Node(false, worldPoint, x, y, penalty);

        grid_nsz[x, y] = new Node(false, worldPoint, x, y, penalty);

        if (id == 1)
        {
            grid_1[x, y] = new Node(false, worldPoint, x, y, penalty);
        }

        if (id == 2)
        {
            grid_2[x, y] = new Node(false, worldPoint, x, y, penalty);
        }

        if (id == 3)
        {
            grid_3[x, y] = new Node(false, worldPoint, x, y, penalty);
        }
        
        try
        {
            for (int i = x - 1; i <= x + 1; i++)
                for (int j = y - 1; j <= y + 1; j++)
                    grid[i, j] = new Node(walkable, worldPoint, i, j, movementPenalty);
        }
        catch
        {

        }
    }

    public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
	}

	void OnDrawGizmos() {
        Vector3 gridCubeSize = new Vector3((nodeDiameter - .1f), 0.1f, (nodeDiameter - .1f));
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, gridCubeSize);
            }
        }
    }

}