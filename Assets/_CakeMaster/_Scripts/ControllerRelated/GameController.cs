using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _CakeMaster._Scripts.ControllerRelated
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;
        [SerializeField] private int goal, moves;
        [SerializeField] private GameObject confettiFx;

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
            {
                MainController.instance.SetActionType(GameState.Levelwin);
                DOVirtual.DelayedCall(1f, ()=>
                {
                    confettiFx.SetActive(true);
                }); 
            }
        }
        public void UpdateMoves()
        {
            if(moves > 0)moves -= 1;
            UIController.instance.UpdateMovesUi(moves);
            if (moves == 0 && goal > 0)
                MainController.instance.SetActionType(GameState.Levelfail);
        }

        public void OnClick_ReloadButton()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }
}
