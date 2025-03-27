using System;
using UnityEngine;

namespace _CakeMaster._Scripts.ControllerRelated
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;
        [SerializeField] private int goal;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            UIController.instance.UpdateGoalUi(goal);
        }

        public void UpdateGoal()
        {
            goal -= 1;
            UIController.instance.UpdateGoalUi(goal);
            if (goal == 0)
            {
                
            }
        }
        
    }
}
