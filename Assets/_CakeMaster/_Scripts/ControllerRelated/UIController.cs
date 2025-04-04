using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;

namespace _CakeMaster._Scripts.ControllerRelated
{
    public class UIController : MonoBehaviour
    {
        public static UIController instance;

        [SerializeField] private GameObject HUD;
        [SerializeField] private GameObject winPanel, failPanel;
        [SerializeField] private TextMeshProUGUI goalText, movesText;
        [SerializeField] private Image goalCakeIcon;
        public Vector3 cakeIconWorldPos;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            cakeIconWorldPos = GetGoalIconWorldPosition(goalCakeIcon.rectTransform);
        }
        Vector3 GetGoalIconWorldPosition(RectTransform uiElement, float worldZ = 0f)
        {
            Vector3 screenPos = uiElement.position;

            float zDistance = Mathf.Abs(worldZ - Camera.main.transform.position.y);

            screenPos.z = zDistance;
            return Camera.main.ScreenToWorldPoint(screenPos);
        }


        public void UpdateGoalUi(int val)
        {
            goalText.text = val.ToString();
        }

        public void UpdateMovesUi(int val)
        {
            //Debug.Log("UpdateMovesUi");
            movesText.text = val.ToString();
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
            if(newState==GameState.Levelwin)
            {
                StartCoroutine(ShowPanel(true, 2));
            }

            if (newState == GameState.Levelfail)
            {
                StartCoroutine(ShowPanel(false, 0.5f));
            }
        }

        IEnumerator ShowPanel(bool isWin, float duration)
        {
            yield return new WaitForSeconds(duration);
            if(isWin)
                winPanel.SetActive(true);
            else failPanel.SetActive(true);
        }
        
    }   
}
