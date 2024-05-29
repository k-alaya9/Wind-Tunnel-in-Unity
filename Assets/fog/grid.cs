//using UnityEngine;

//public class Grid : MonoBehaviour
//{
//    public int gridSizeX = 10;  // Number of cells in the X direction
//    public int gridSizeY = 10;  // Number of cells in the Y direction
//    public int gridSizeZ = 10;  // Number of cells in the Z direction

//    public float cellSize = 10f;  // Size of each grid cell

//    public Gradient colorGradient;  // Gradient for cell colors

//    public float transparency = 0.5f;  // Transparency of the grid cells

//    private GameObject[,,] gridCells;  // Array to store the game objects in each grid cell

//    public GameObject[,,] GridCells => gridCells;  // Public property to access the grid cells

//    private void Start()
//    {
//        CreateGrid();
//    }

//    private void CreateGrid()
//    {
//        // Initialize the gridCells array
//        gridCells = new GameObject[gridSizeX, gridSizeY, gridSizeZ];

//        // Create game objects for each grid cell
//        for (int x = 0; x < gridSizeX; x++)
//        {
//            for (int y = 0; y < gridSizeY; y++)
//            {
//                for (int z = 0; z < gridSizeZ; z++)
//                {
//                    Vector3 cellPosition = new Vector3(x * cellSize, y * cellSize, z * cellSize);
//                    GameObject cellObject = GameObject.CreatePrimitive(PrimitiveType.Cube);  // Create a cube as a grid cell
//                    cellObject.transform.position = cellPosition;  // Set the position of the grid cell
//                    cellObject.transform.localScale = new Vector3(cellSize, cellSize, cellSize);  // Set the size of the grid cell
//                    cellObject.transform.SetParent(transform);  // Set the grid as the parent of the grid cell

//                    Renderer cellRenderer = cellObject.GetComponent<Renderer>();  // Get the renderer component of the grid cell

//                    // Set a blended color for each grid cell based on the gradient
//                    float t = (float)x / gridSizeX;  // Calculate the interpolation parameter
//                    Color cellColor = colorGradient.Evaluate(t);  // Evaluate the gradient at the given parameter
//                    cellColor.a = transparency;  // Set the transparency of the color
//                    cellRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));  // Set a transparent material
//                    cellRenderer.material.color = cellColor;  // Set the color of the grid cell

//                    gridCells[x, y, z] = cellObject;  // Store the grid cell in the array
//                }
//            }
//        }
//    }
//}


using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int gridSizeX = 10;  // Number of cells in the X direction
    public int gridSizeY = 10;  // Number of cells in the Y direction
    public int gridSizeZ = 10;  // Number of cells in the Z direction

    public float cellSize = 10f;  // Size of each grid cell

    public Gradient colorGradient;  // Gradient for cell colors

    public float transparency = 0.5f;  // Transparency of the grid cells

    private GameObject[,,] gridCells;  // Array to store the game objects in each grid cell

    public GameObject[,,] GridCells => gridCells;  // Public property to access the grid cells

    private void Start()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        // Initialize the gridCells array
        gridCells = new GameObject[gridSizeX, gridSizeY, gridSizeZ];

        // Create game objects for each grid cell
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 cellPosition = new Vector3(x * cellSize, y * cellSize, z * cellSize);
                    GameObject cellObject = GameObject.CreatePrimitive(PrimitiveType.Cube);  // Create a cube as a grid cell
                    cellObject.transform.position = cellPosition;  // Set the position of the grid cell
                    cellObject.transform.localScale = new Vector3(cellSize, cellSize, cellSize);  // Set the size of the grid cell
                    cellObject.transform.SetParent(transform);  // Set the grid as the parent of the grid cell
                    cellObject.AddComponent<GridCell>();  // Add GridCell component

                    Renderer cellRenderer = cellObject.GetComponent<Renderer>();  // Get the renderer component of the grid cell

                    // Set a blended color for each grid cell based on the gradient
                    float t = (float)x / gridSizeX;  // Calculate the interpolation parameter
                    Color cellColor = colorGradient.Evaluate(t);  // Evaluate the gradient at the given parameter
                    cellColor.a = transparency;  // Set the transparency of the color
                    cellRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));  // Set a transparent material
                    cellRenderer.material.color = cellColor;  // Set the color of the grid cell

                    gridCells[x, y, z] = cellObject;  // Store the grid cell in the array
                }
            }
        }

        Debug.Log("Grid created with size: " + gridSizeX + " x " + gridSizeY + " x " + gridSizeZ);
    }
}

//public class GridCell : MonoBehaviour
//{
//    private List<Particle> particles = new List<Particle>();

//    public void AddParticle(Particle particle)
//    {
//        particles.Add(particle);
//    }

//    public List<Particle> GetParticles()
//    {
//        return particles;
//    }

//    public void Clear()
//    {
//        particles.Clear();
////    }
//}


