using System;
using UnityEngine;

namespace _CakeMaster._Scripts.ControllerRelated
{
    public enum GameState
    {
        None,
        Create,
        Gameplay,
        Sorting,
        Refilling,
        RecheckFill,
        Levelwin,
        Levelfail
    }

    public class MainController : MonoBehaviour
    {
        public static MainController instance;
        
        [SerializeField] private GameState _gameState;
        public static event Action<GameState, GameState> GameStateChanged;

        public GameState GameState
        {
            get => _gameState;
            private set
            {
                if (value != _gameState)
                {
                    GameState oldState = _gameState;
                    _gameState = value;
                    if (GameStateChanged != null)
                        GameStateChanged(_gameState, oldState);
                }
            }
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            CreateGame();
        }

        void CreateGame()
        {
            GameState = GameState.Create;
        }

        public void SetActionType(GameState _curState)
        {
            GameState = _curState;
        }
    }
}