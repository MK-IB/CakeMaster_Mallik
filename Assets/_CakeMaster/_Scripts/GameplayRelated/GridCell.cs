using System.Collections.Generic;
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
        public CakeElement containedCake;
        public CakeColors CakeColor
        {
            get => _cakeColor;
        }
        void Start()
        {
            _cakesdetail = transform.root.GetComponent<GridSelection>().cakesDetail;
            InitiateCakes();
        }

        void InitiateCakes()
        {
            List<GameObject> cakes = _cakesdetail.cakes;
            GameObject target = cakes[Random.Range(0, cakes.Count)];
            Vector3 spawnPos = transform.position + Vector3.up * 0.3f;
            GameObject fullCake = Instantiate(target, spawnPos, Quaternion.identity);
            //Debug.Log("FULL CAKE" + fullCake.name);
            containedCake = fullCake.GetComponent<CakeElement>();
            _cakeColor = containedCake.cakeColor;
            _activeSlicesNumber = containedCake.ActivateSlices();
        }

        public void ToggleHighlighter()
        {
            highlighter.SetActive(!highlighter.activeSelf);
        }
    }
}
