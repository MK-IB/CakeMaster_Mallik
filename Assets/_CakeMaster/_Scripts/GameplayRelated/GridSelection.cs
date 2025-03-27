using System.Collections;
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

// Step 1: Count total available slices from all except the last
            int totalAvailableSlices = 0;
            for (int i = 0; i < selectedCells.Count - 1; i++)
            {
                totalAvailableSlices += selectedCells[i].containedCake.GetActivatedSlices();
            }

// Step 2: Start from the last cake and go backward
            for (int i = selectedCells.Count - 1; i >= 0; i--)
            {
                var targetCake = selectedCells[i].containedCake;
                int currentSlices = targetCake.GetActivatedSlices();

                if (currentSlices >= 6) continue; // Skip full cakes

                int needed = 6 - currentSlices;
                int toAdd = Mathf.Min(needed, totalAvailableSlices);

                if (toAdd > 0)
                {
                    // Add slices to this cake
                    targetCake.AddSlices(toAdd); // or your equivalent method
                    totalAvailableSlices -= toAdd;

                    // Now, take slices from previous cakes and deactivate
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
                }

                if (totalAvailableSlices <= 0)
                    break; // Stop if we've used all available slices
            }

            /*CakeElement lastCake = selectedCells[selectedCells.Count - 1].containedCake;
            CakeElement secondLastCake = selectedCells[selectedCells.Count - 2].containedCake;
            int empty = lastCake.GetEmptySpaces();
            int prevAvailable = secondLastCake.GetActivatedSlices();
            //Debug.Log($"empty: {empty}, prevAvailable: {prevAvailable}");
            if(empty == prevAvailable)
            {
                lastCake.ActivateSlices(prevAvailable);
                secondLastCake.DeactivateSlices(prevAvailable);
            }
            else if(prevAvailable < empty)
            {
                lastCake.ActivateSlices(prevAvailable);
                secondLastCake.DeactivateSlices(prevAvailable);
            }
            else if(empty < prevAvailable)
            {
                lastCake.ActivateSlices(empty);
                secondLastCake.DeactivateSlices(empty);
            }*/

            StartCoroutine(AfterSortState());
            //StartCoroutine(secondLastCake.MoveSlicesToTarget(lastCake.GetEmptySpaces(), lastCake.transform));
        }

        IEnumerator AfterSortState()
        {
            for(int i = 0; i < selectedCells.Count; i++)
                selectedCells[i].ToggleHighlighter();
            selectedCells = new List<GridCell>();
            
            yield return null;
        }
        

    }
}
