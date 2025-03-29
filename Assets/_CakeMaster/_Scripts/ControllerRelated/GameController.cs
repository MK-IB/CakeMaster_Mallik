using System;
using UnityEngine;

namespace _CakeMaster._Scripts.ControllerRelated
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;
        [SerializeField] private int goal, moves;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            UIController.instance.UpdateGoalUi(goal);
            UIController.instance.UpdateMovesUi(moves);
        }

        public void UpdateGoal()
        {
            if(goal > 0)goal -= 1;
            UIController.instance.UpdateGoalUi(goal);
            if (goal == 0)
                MainController.instance.SetActionType(GameState.Levelwin);
        }
        public void UpdateMoves()
        {
            if(moves > 0)moves -= 1;
            UIController.instance.UpdateMovesUi(moves);
            if (moves == 0 && goal > 0)
                MainController.instance.SetActionType(GameState.Levelfail);
        }
        
    }
}
