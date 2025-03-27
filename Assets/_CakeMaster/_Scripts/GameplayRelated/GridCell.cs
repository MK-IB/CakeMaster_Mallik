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
            CakeElement cakeElement = fullCake.GetComponent<CakeElement>();
            _cakeColor = cakeElement.cakeColor;
            _activeSlicesNumber = cakeElement.ActivateSlices();
        }

        public void IsSelected()
        {
            highlighter.SetActive(true);
        }
    }
}
