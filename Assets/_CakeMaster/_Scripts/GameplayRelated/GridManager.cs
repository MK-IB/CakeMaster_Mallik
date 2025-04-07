using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    public GameObject cubePrefab; // Assign a Cube prefab in Inspector
    public int rows = 5, cols = 5; // Grid size
    private GameObject[,] grid;
    private Color[] colors = { Color.red, Color.blue }; // Two colors
    
    LineRenderer lineRenderer; // Assign a LineRenderer in the inspector
    private bool isSelecting = false;
    [SerializeField] private Vector3 lineOffset;
    [SerializeField] private Color lineStartColor, lineEndColor;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
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
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, cube1.transform.position + lineOffset);
        }
    }
    void DetectSecondCube(Vector2 touchPosition)
    {
        CubeController cube = GetCubeFromTouch(touchPosition);
        if (cube != null && cube != cube1 && (cube1.Row != cube.Row && cube1.Column != cube.Column))
        {
            cube2 = cube;
            //Debug.Log($"CUBE 2: {cube2}");
            DetectOtherCubes();
        }
    }

    void DetectOtherCubes()
    {
        cube3 = grid[cube1.Row, cube2.Column].GetComponent<CubeController>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(1, cube3.transform.position + lineOffset);
        
        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(2, cube2.transform.position + lineOffset);
        
        cube4 = grid[cube2.Row, cube1.Column].GetComponent<CubeController>();
        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(3, cube4.transform.position + lineOffset);
        lineRenderer.positionCount = 5;
        lineRenderer.SetPosition(4, cube1.transform.position + lineOffset);
        VisualForSorting();
    }

    bool isSortingPossible = false;
    void VisualForSorting()
    {
        Debug.Log("TRUEE");
        if(cube1 == null || cube2 == null || cube3 == null || cube4 == null) return;
        if (cube1.ColorKey == cube2.ColorKey &&
            cube2.ColorKey == cube3.ColorKey &&
            cube3.ColorKey == cube4.ColorKey)
        {
            
            lineRenderer.startColor = lineStartColor;
            lineRenderer.endColor = lineEndColor;
            isSortingPossible = true;
        }
        else
        {
            lineRenderer.startColor = Color.grey;
            lineRenderer.endColor = Color.grey;
            isSortingPossible = false;
        }
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
        lineRenderer.positionCount = 0;
        Debug.Log($"CUBE 1 POS: {cube1.Row}, {cube1.Column}");
        Debug.Log($"CUBE 2 POS: {cube2.Row}, {cube2.Column}");
        if (isSortingPossible)
        {
            int minRow = Mathf.Min(cube1.Row, cube2.Row, cube3.Row, cube4.Row);
            int maxRow = Mathf.Max(cube1.Row, cube2.Row, cube3.Row, cube4.Row);
            int minCol = Mathf.Min(cube1.Column, cube2.Column, cube3.Column, cube4.Column);
            int maxCol = Mathf.Max(cube1.Column, cube2.Column, cube3.Column, cube4.Column);

            for (int row = minRow; row <= maxRow; row++)
            {
                for (int col = minCol; col <= maxCol; col++)
                {
                    grid[row, col].SetActive(false);
                }
            }
        }
    }
}