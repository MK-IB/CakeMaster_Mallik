using System;
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
        public Color blue, green, red, brown;
        [SerializeField] private List<GridCell> gridCells;
        private List<List<GridCell>> gridCellsList = new List<List<GridCell>>();
        
        private LineRenderer lineRenderer;
        Dictionary<CakeColors, Color> colorMapping = new Dictionary<CakeColors, Color>();
        void Awake()
        {
            ArrangeGridsInArray();
        }

        private void Start()
        {
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }
            colorMapping.Add(CakeColors.Blue, blue);
            colorMapping.Add(CakeColors.Green, green);
            colorMapping.Add(CakeColors.Red, red);
            colorMapping.Add(CakeColors.Brown, brown);
            SetupLineRenderer();
        }
        void SetupLineRenderer()
        {
            lineRenderer.positionCount = 0;
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            SetupLineColor(Color.yellow);
        }

        void SetupLineColor(Color color)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
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
                    //Debug.Log($"columnCount: {columnList.Count}");
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
                
                        if (distance > 2.5f)
                            return;
                    }

                    selectedCells.Add(gridCell);
                    gridCell.ToggleHighlighter(true);
                    UpdateLineRenderer();
                }
            }
        }
        void UpdateLineRenderer()
        {
            SetupLineColor(colorMapping[currentCakeColor]);
            lineRenderer.positionCount = selectedCells.Count;
            for (int i = 0; i < selectedCells.Count; i++)
            {
                lineRenderer.SetPosition(i, selectedCells[i].transform.position);
            }
        }

        IEnumerator SortSelectedCells()
        {
            lineRenderer.positionCount = 0;
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
            
            for (int i = selectedCells.Count - 1; i > 0; i--)
            {
                var targetCake = selectedCells[i].containedCake;
                int currentSlices = targetCake.GetActivatedSlices();

                if (currentSlices >= 6) continue;

                int needed = 6 - currentSlices;
                int toAdd = Mathf.Min(needed, totalAvailableSlices);

                if (toAdd > 0)
                {
                    targetCake.AddSlices(toAdd, selectedCells[i - 1].transform.position);
                    /*try
                    {
                        if (selectedCells[i - 1] != null || selectedCells != null)
                            targetCake.AddSlices(toAdd, selectedCells[i - 1].transform.position);
                    }
                    catch
                    {
                        
                    }*/
                    
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
                    GameController.instance.UpdateMoves();
                }
                
                if (totalAvailableSlices <= 0)
                    break; // Stop if we've used all available slices
            }
            
            StartCoroutine(AfterSortState());
        }

        private GridCell lastWorkingCell = null;
        public void RefillGridCell(GridCell gridCell)
        {
            List<GridCell> foundList = gridCellsList.FirstOrDefault(subList => subList.Contains(gridCell));
            //Debug.Log($"GRID cells list={gridCellsList.Count} ** FoundList={foundList[0].name} ");

            if (foundList.Contains(lastWorkingCell) && foundList.Contains(gridCell)) return;
            lastWorkingCell = gridCell;
            
            //find the gap num
            //run loop matching that gap step
            //shift cakes matching that gap step
            int gridIndex = foundList.IndexOf(gridCell);
            for (int i = gridIndex - 1; i >= 0; i--) 
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
