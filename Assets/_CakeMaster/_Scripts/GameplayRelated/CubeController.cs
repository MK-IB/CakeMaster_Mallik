using UnityEngine;

public class CubeController : MonoBehaviour
{
    private int row, col;
    private Color cubeColor;
    public string colorKey;
    public string ColorKey => colorKey;

    public int Row { get => row; }
    public int Column { get => col; }
    public Color Color { get => cubeColor; }

    public void Initialize(int row, int col, Color color)
    {
        this.row = row;
        this.col = col;
        cubeColor = color;
        colorKey = GetColorKey(color);
    }
    private string GetColorKey(Color color)
    {
        if (color == Color.red) return "Red";
        if (color == Color.blue) return "Blue";
        // Add more if needed
        return "Unknown";
    }
    void OnMouseDown()
    {
        Debug.Log($"Clicked on Cube at ({row}, {col}) with Color: {cubeColor}");
    }
}