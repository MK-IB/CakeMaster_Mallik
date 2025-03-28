using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _CakeMaster._Scripts.ControllerRelated;
using _CakeMaster._Scripts.ElementRelated;
using DG.Tweening;
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
        //public Color blue, green, red, brown;
        [SerializeField] private List<GridCell> gridCells;
        
        private List<List<GridCell>> gridCellsList = new List<List<GridCell>>();
        void Awake()
        {
            ArrangeGridsInArray();
        }

        void ArrangeGridsInArray()
        {
            int columnCount = 0;
            List<GridCell> columnList = new List<GridCell>();
            for (int i = 0; i < gridCells.Count; i++)
            {
                columnCount++;
                columnList.Add(gridCells[i]);
                if (columnCount >= 7)
                {
                    gridCellsList.Add(columnList);
                    Debug.Log($"columnCount: {columnList.Count}");
                    columnList = new List<GridCell>();
                    columnCount = 0;
                }

            }
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
                StartCoroutine(SortSelectedCells());
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
                if(gridCell == null) return;
                if (currentCakeColor == CakeColors.None)
                    currentCakeColor = gridCell.CakeColor;
                if (!selectedCells.Contains(gridCell) && currentCakeColor == gridCell.CakeColor)
                {
                    if (selectedCells.Count > 0)
                    {
                        GridCell lastSelectedCell = selectedCells[selectedCells.Count - 1];
                        float distance = Vector3.Distance(lastSelectedCell.transform.position, gridCell.transform.position);
                
                        if (distance > 2.0f)
                            return;
                    }

                    selectedCells.Add(gridCell);
                    gridCell.ToggleHighlighter(true);
                }
            }
        }

        IEnumerator SortSelectedCells()
        {
            if (selectedCells.Count < 2)
            {
                for(int i = 0; i < selectedCells.Count; i++)
                    selectedCells[i].ToggleHighlighter(false);
                selectedCells = new List<GridCell>();
                yield break;
            }
            MainController.instance.SetActionType(GameState.Sorting);
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
                    try
                    {
                        if (selectedCells[i - 1] != null || selectedCells != null)
                            targetCake.AddSlices(toAdd, selectedCells[i - 1].transform.position);
                    }
                    catch
                    {
                        
                    }
                    
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
                    if(targetCake.GetActivatedSlices() >=6)
                    {
                        yield return new WaitForSeconds(0.35f);
                        targetCake.AnimateCakeOnSorted();
                        GameController.instance.UpdateGoal();
                        yield return new WaitForSeconds(0.35f);
                        //DetectTheListFamily(selectedCells[i]);
                    }
                }
                
                if (totalAvailableSlices <= 0)
                    break; // Stop if we've used all available slices
            }
            
            StartCoroutine(AfterSortState());
        }

        public void RefillGridCell(GridCell gridCell)
        {
            List<GridCell> foundList = gridCellsList.FirstOrDefault(subList => subList.Contains(gridCell));
            //Debug.Log($"GRID cells list={gridCellsList.Count} ** FoundList={foundList[0].name} ");

            int gridIndex = foundList.IndexOf(gridCell);
            Debug.Log($"Found gridCell: {gridIndex}");
            int emptyIndex = -1; // Track the lowest empty cell index

            for (int i = gridIndex - 1; i >= 0; i--) // Start from bottom to top
            {
                CakeElement containedCake = foundList[i].containedCake;
                if (containedCake != null)
                {
                    GridCell belowCell = foundList[i + 1];
                    belowCell.containedCake = containedCake;
                    containedCake.transform.DOMove(belowCell.transform.position, 0.25f);
                    belowCell.SetupContainedCake();
                }
            }

            MainController.instance.SetActionType(GameState.Gameplay);
        }
        IEnumerator AfterSortState()
        {
            for(int i = 0; i < selectedCells.Count; i++)
                selectedCells[i].ToggleHighlighter(false);
            selectedCells = new List<GridCell>();

            yield return new WaitForSeconds(0.35f);
            MainController.instance.SetActionType(GameState.Refilling);
        }
    }
}
