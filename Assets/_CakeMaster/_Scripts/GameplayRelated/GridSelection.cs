using System.Collections.Generic;
using _CakeMaster._Scripts.ElementRelated;
using UnityEngine;

namespace _CakeMaster._Scripts.GameplayRelated
{
    public enum CakeColors
    {
        None,
        Blue,
        Green,
        Red,
        Yellow,
        Brown,
        Pink
    }
    public class GridSelection : MonoBehaviour
    {
        List<GridCell> selectedCells = new List<GridCell>();
        public CakesDetail cakesDetail;
        void Start()
        {
        
        }

        bool _canCheckGrid;
        void Update()
        {
            if(Input.GetMouseButtonDown(0))
                _canCheckGrid = true;
            if(Input.GetMouseButtonUp(0))
            {
                _canCheckGrid = false;
                currentCakeColor = default;
            }
            if(_canCheckGrid)
                CastRayFromTouch(Input.mousePosition);
        }

        public CakeColors currentCakeColor;
        void CastRayFromTouch(Vector2 screenPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                GridCell gridCell = hit.collider.GetComponent<GridCell>();
                if (currentCakeColor == CakeColors.None)
                    currentCakeColor = gridCell.CakeColor;
                if (gridCell != null && !selectedCells.Contains(gridCell) && currentCakeColor == gridCell.CakeColor)
                {
                    selectedCells.Add(gridCell);
                    gridCell.IsSelected();
                }
            }
        }

    }
}
