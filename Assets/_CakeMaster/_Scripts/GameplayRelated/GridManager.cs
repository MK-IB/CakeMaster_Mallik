using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    public GameObject cubePrefab; // Assign a Cube prefab in Inspector
    public int rows = 5, cols = 5; // Grid size
    private GameObject[,] grid;
    private Color[] colors = { Color.red, Color.blue }; // Two colors
    
    public LineRenderer lineRenderer; // Assign a LineRenderer in the inspector
    private bool isSelecting = false;

    void Start()
    {
        lineRenderer.positionCount = 0;
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new GameObject[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 position = new Vector3(col, -row, 0);
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                cube.transform.parent = transform;
                
                Color randomColor = colors[Random.Range(0, colors.Length)];
                cube.transform.GetChild(0).GetComponent<Renderer>().material.color = randomColor;
                
                CubeController cubeScript = cube.AddComponent<CubeController>();
                cubeScript.Initialize(row, col, randomColor);

                grid[row, col] = cube;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // First touch
        {
            DetectFirstCube(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0)) // Dragging
        {
            DetectSecondCube(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0)) // Release touch
        {
            ResetSelection();
        }
    }

    CubeController cube1, cube2, cube3, cube4;

    void DetectFirstCube(Vector2 screenPosition)
    {
        cube1 = GetCubeFromTouch(screenPosition);
        if (cube1 != null)
        {
            isSelecting = true;
        }
    }
    void DetectSecondCube(Vector2 touchPosition)
    {
        
    }
    CubeController GetCubeFromTouch(Vector2 touchPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 50))
        {
            return hit.collider.GetComponent<CubeController>();
        }
        return null;
    }

    void ResetSelection()
    {
        
    }
}