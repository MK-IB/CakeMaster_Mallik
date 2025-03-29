using System.Collections;
using System.Collections.Generic;
using _CakeMaster._Scripts.ControllerRelated;
using _CakeMaster._Scripts.ElementRelated;
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
        public CakeColors CakeColor
        {
            get => _cakeColor;
        }
        void Start()
        {
            _gridSelection = transform.root.GetComponentInParent<GridSelection>();
            _cakesdetail = _gridSelection.cakesDetail;
            InitiateCakes();
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
                if(containedCake == null)
                    _gridSelection.RefillGridCell(this);
                else if(!containedCake.gameObject.activeInHierarchy || containedCake.GetActivatedSlices() == 0)
                {
                    StartCoroutine(CheckRefill());
                }
                
            }

        }

        IEnumerator CheckRefill()
        {
            Destroy(containedCake.gameObject);
            yield return null;
            _gridSelection.RefillGridCell(this);
            Debug.Log($"EMPTY GRID = {transform.name}");
        }
        

        void InitiateCakes()
        {
            List<GameObject> cakes = _cakesdetail.cakes;
            GameObject target = cakes[Random.Range(0, cakes.Count)];
            Vector3 spawnPos = transform.position + Vector3.up * 0.3f;
            GameObject fullCake = Instantiate(target, spawnPos, Quaternion.identity);
            //Debug.Log("FULL CAKE" + fullCake.name);
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

        public void ToggleHighlighter(bool state)
        {
            highlighter.SetActive(state);
            _animator.SetBool("scaleDown", state);
            if(state)containedCake.SelectionFx();
        }
    }
}
