using System.Collections;
using System.Collections.Generic;
using _CakeMaster._Scripts.ControllerRelated;
using _CakeMaster._Scripts.ElementRelated;
using DG.Tweening;
using UnityEngine;

namespace _CakeMaster._Scripts.GameplayRelated
{
    public class GridCell : MonoBehaviour
    {
        [SerializeField] private GameObject highlighter;
        private CakesDetail _cakesdetail;
        private int _activeSlicesNumber;
        private CakeColors _cakeColor;
        private Animator _animator;
        private GridSelection _gridSelection;
        
        public CakeElement containedCake;
        private bool isEmpty = false;
        
        public bool IsEmpty { get => isEmpty; }
        public CakeColors CakeColor
        {
            get => _cakeColor;
        }
        void Start()
        {
            _gridSelection = transform.root.GetComponentInParent<GridSelection>();
            _cakesdetail = _gridSelection.cakesDetail;
            InitiateCakes(transform.position + Vector3.up * 0.3f + Vector3.forward * 1);
        }
        private void OnEnable()
        {
            MainController.GameStateChanged += GameManager_GameStateChanged;
        }
        private void OnDisable()
        {
            MainController.GameStateChanged -= GameManager_GameStateChanged;
        }
        void GameManager_GameStateChanged(GameState newState, GameState oldState)
        {
            if (newState == GameState.Refilling)
            {
                if (containedCake == null)
                {
                    isEmpty = true;
                    StartCoroutine(CheckRefill()); // <-- Add this
                }
                else if(!containedCake.gameObject.activeSelf || containedCake.GetActivatedSlices() == 0)
                {
                    Destroy(containedCake.gameObject);
                    StartCoroutine(CheckRefill());
                }
                
            }

            /*if (newState == GameState.RecheckFill)
            {
                if(containedCake == null)
                    InitiateCakes();
                else if(!containedCake.gameObject.activeInHierarchy || containedCake.GetActivatedSlices() == 0)
                {
                    InitiateCakes();
                }
            }*/
        }

        public void ClearCake()
        {
            containedCake = null;
        }

        public void SetCake(CakeElement cake)
        {
            containedCake = cake;
            SetupContainedCake();
        }

        IEnumerator CheckRefill()
        {
            isEmpty = true;
            yield return null;
            StartCoroutine(_gridSelection.RefillGridCell(this));
            Debug.Log($"EMPTY GRID = {containedCake}");
        }
        

        public void InitiateCakes(Vector3 spawnPos)
        {
            List<GameObject> cakes = _cakesdetail.cakes;
            GameObject target = cakes[Random.Range(0, cakes.Count)];
            GameObject fullCake = Instantiate(target, spawnPos, Quaternion.identity);
            fullCake.transform.DOMove(transform.position, 0.35f).SetEase(Ease.OutBounce);
            //Debug.Log("FULL CAKE" + fullCake.name);
            isEmpty = false;
            containedCake = fullCake.GetComponent<CakeElement>();
            _activeSlicesNumber = containedCake.ActivateSlices();
            SetupContainedCake();
            //containedCake.SetFxColor(_gridSelection.GetCakeColor(_cakeColor));
        }

        public void SetupContainedCake()
        {
            _cakeColor = containedCake.cakeColor;
            _activeSlicesNumber = containedCake.GetActivatedSlices();
            _animator = containedCake.GetComponent<Animator>();
        }

        public void ToggleHighlighter(bool state, Color highlightCol)
        {
            highlighter.GetComponent<SpriteRenderer>().color = highlightCol;
            highlighter.SetActive(state);
            _animator.SetBool("scaleDown", state);
            if(state)containedCake.SelectionFx();
        }
    }
}
