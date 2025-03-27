using System.Collections;
using System.Collections.Generic;
using _CakeMaster._Scripts.ControllerRelated;
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
        public List<GridCell> selectedCells = new List<GridCell>();
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
                SortSelectedCells();
            }
            if(_canCheckGrid)
                CastRayFromTouch(Input.mousePosition);
        }

        public CakeColors currentCakeColor;
        void CastRayFromTouch(Vector2 screenPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 50))
            {
                GridCell gridCell = hit.collider.GetComponent<GridCell>();
                if (currentCakeColor == CakeColors.None)
                    currentCakeColor = gridCell.CakeColor;
                if (gridCell != null && !selectedCells.Contains(gridCell) && currentCakeColor == gridCell.CakeColor)
                {
                    selectedCells.Add(gridCell);
                    gridCell.ToggleHighlighter();
                }
            }
        }

        void SortSelectedCells()
        {
            if (selectedCells.Count < 2) return;
            
            int totalAvailableSlices = 0;
            for (int i = 0; i < selectedCells.Count - 1; i++)
            {
                totalAvailableSlices += selectedCells[i].containedCake.GetActivatedSlices();
            }
            
            for (int i = selectedCells.Count - 1; i >= 0; i--)
            {
                var targetCake = selectedCells[i].containedCake;
                int currentSlices = targetCake.GetActivatedSlices();

                if (currentSlices >= 6) continue;

                int needed = 6 - currentSlices;
                int toAdd = Mathf.Min(needed, totalAvailableSlices);

                if (toAdd > 0)
                {
                    targetCake.AddSlices(toAdd);
                    totalAvailableSlices -= toAdd;
                    
                    int slicesToTake = toAdd;

                    for (int j = 0; j < selectedCells.Count - 1 && slicesToTake > 0; j++)
                    {
                        var donorCake = selectedCells[j].containedCake;
                        int donorSlices = donorCake.GetActivatedSlices();

                        int slicesFromThisCake = Mathf.Min(donorSlices, slicesToTake);

                        for (int s = 0; s < slicesFromThisCake; s++)
                        {
                            donorCake.DeactivateOneSlice(); // <-- You'll need this method
                            slicesToTake--;
                        }
                    }
                    if(targetCake.GetActivatedSlices() >=6) targetCake.AnimateCakeOnSorted();
                }

                if (totalAvailableSlices <= 0)
                    break; // Stop if we've used all available slices
            }
            
            StartCoroutine(AfterSortState());
        }

        IEnumerator AfterSortState()
        {
            for(int i = 0; i < selectedCells.Count; i++)
                selectedCells[i].ToggleHighlighter();
            selectedCells = new List<GridCell>();
            
            GameController.instance.UpdateGoal();
            
            yield return null;
        }
        

    }
}
