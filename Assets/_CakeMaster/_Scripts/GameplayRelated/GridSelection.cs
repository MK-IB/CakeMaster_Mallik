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
                    gridCell.ToggleHighlighter(true, colorMapping[selectedCells[0].CakeColor]);
                    SoundsController.instance.PlayClip(SoundsController.instance.tap);
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

        public List<GameObject> totalAvailableSlicesList = new List<GameObject>();
        IEnumerator SortSelectedCells()
        {
            lineRenderer.positionCount = 0;
            if (selectedCells.Count < 2)
            {
                for(int i = 0; i < selectedCells.Count; i++)
                    selectedCells[i].ToggleHighlighter(false, Color.white);
                selectedCells = new List<GridCell>();
                yield break;
            }
            //MainController.instance.SetActionType(GameState.Sorting);
            int totalAvailableSlices = 0;
            //totalAvailableSlicesList = new List<GameObject>();
            for (int i = selectedCells.Count - 2; i >= 0; i--)
            {
                CakeElement cakeElement = selectedCells[i].containedCake;
                List<GameObject> activatedSlicesList = cakeElement.GetActivatedSlicesList();
                for (int j = 0; j < activatedSlicesList.Count; j++)
                {
                    totalAvailableSlicesList.Add(activatedSlicesList[j]);
                }
            }

            for (int i = selectedCells.Count - 2; i >= 0; i--)
                selectedCells[i].containedCake.ResetCakesData();
            
            for (int i = selectedCells.Count - 1; i > 0; i--)
            {
                totalAvailableSlices = totalAvailableSlicesList.Count;
                var targetCake = selectedCells[i].containedCake;
                int needed = 6 - targetCake.GetActivatedSlices();
                //Debug.Log($"totalAvailableSlices: {totalAvailableSlices} ** needed: {needed}");
                if (needed <= 0) continue;

                int toAdd = Mathf.Min(needed, totalAvailableSlices);
                if (toAdd <= 0) continue;

                yield return StartCoroutine(targetCake.AddSlices(toAdd, totalAvailableSlicesList));
                
                if (totalAvailableSlices <= 0)
                    break;
                //yield return new WaitForSeconds(0.1f);
            }

            totalAvailableSlicesList = new List<GameObject>();
            yield return new WaitForSeconds(.5f);
            for (int i = 0; i < selectedCells.Count; i++)
            {
                if(selectedCells[i].containedCake.GetActivatedSlices() >= 6)
                {
                    selectedCells[i].containedCake.AnimateCakeOnSorted();
                    //StartCoroutine(RefillGridCell(selectedCells[i]));
                    GameController.instance.UpdateGoal();
                }
            }
            GameController.instance.UpdateMoves();
            StartCoroutine(AfterSortState());
            
            yield return new WaitForSeconds(1.0f);
            MainController.instance.SetActionType(GameState.Refilling);
        }

        private GridCell lastWorkingCell = null;
        public IEnumerator RefillGridCell(GridCell gridCell)
        {
            List<GridCell> foundList = gridCellsList.FirstOrDefault(subList => subList.Contains(gridCell));
            //Debug.Log($"GRID cells list={gridCellsList.Count} ** FoundList={foundList[0].name} ");

            if (foundList.Contains(lastWorkingCell) && foundList.Contains(gridCell)) yield break;
            lastWorkingCell = gridCell;
            Debug.Log("Refill called");
            int gridIndex = foundList.IndexOf(gridCell);
            
            for (int i = gridIndex - 1; i >= 0; i--) 
            {
                CakeElement containedCake = foundList[i].containedCake;
                if (containedCake != null)
                {
                    GridCell belowCell = foundList[i + 1];
                    belowCell.containedCake = containedCake;
                    containedCake.transform.DOMove(belowCell.transform.position, 0.35f).SetEase(Ease.OutBounce);
                    belowCell.SetupContainedCake();
                }
            }

            yield return null;
        }
        IEnumerator AfterSortState()
        {
            for(int i = 0; i < selectedCells.Count; i++)
                selectedCells[i].ToggleHighlighter(false, Color.white);
            selectedCells = new List<GridCell>();
            yield return new WaitForSeconds(0.35f);
        }
    }
}
