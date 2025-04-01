using UnityEngine;

public class CubeController : MonoBehaviour
{
    private int row, col;
    private Color cubeColor;

    public void Initialize(int row, int col, Color color)
    {
        this.row = row;
        this.col = col;
        this.cubeColor = color;
    }

    void OnMouseDown()
    {
        Debug.Log($"Clicked on Cube at ({row}, {col}) with Color: {cubeColor}");
    }
}