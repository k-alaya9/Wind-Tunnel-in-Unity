using System.Collections.Generic;
using UnityEngine;

public class SpatialGrid
{
    private readonly Dictionary<Vector3Int, List<Particle>> grid;
    private readonly float cellSize;

    public SpatialGrid(float cellSize)
    {
        this.cellSize = cellSize;
        grid = new Dictionary<Vector3Int, List<Particle>>();
    }

    public void Clear()
    {
        grid.Clear();
    }

    public Vector3Int GetCellIndex(Vector3 position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / cellSize),
            Mathf.FloorToInt(position.y / cellSize),
            Mathf.FloorToInt(position.z / cellSize)
        );
    }

    public void AddParticle(Particle particle)
    {
        Vector3Int cellIndex = GetCellIndex(particle.position);
        if (!grid.ContainsKey(cellIndex))
        {
            grid[cellIndex] = new List<Particle>();
        }
        grid[cellIndex].Add(particle);
    }

    public List<Particle> GetNearbyParticles(Vector3 position)
    {
        Vector3Int cellIndex = GetCellIndex(position);
        List<Particle> nearbyParticles = new List<Particle>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    Vector3Int neighborIndex = cellIndex + new Vector3Int(x, y, z);
                    if (grid.ContainsKey(neighborIndex))
                    {
                        nearbyParticles.AddRange(grid[neighborIndex]);
                    }
                }
            }
        }
        return nearbyParticles;
    }
}
