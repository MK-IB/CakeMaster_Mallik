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
        [SerializeField] private TextMeshProUGUI goalText;
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

            // Calculate the screen Z (distance from camera to desired world Z)
            float zDistance = Mathf.Abs(worldZ - Camera.main.transform.position.y); // Y used if top-down

            screenPos.z = zDistance;

            // Convert to world point
            return Camera.main.ScreenToWorldPoint(screenPos);
        }


        public void UpdateGoalUi(int val)
        {
            goalText.text = val.ToString();
        }
        
        /*private void OnEnable()
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
                winPanel.SetActive(true);
                //SoundsController.instance.PlaySound(SoundsController.instance.win);
            }

            if (newState == GameState.Levelfail)
            {
                failPanel.SetActive(true);
                //SoundsController.instance.PlaySound(SoundsController.instance.fail);
            }
        }*/
        
    }   
}
