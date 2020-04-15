#pragma warning disable 0649


using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace EndlessGame.Screens
{
    public class MenuScreen : MonoBehaviour
    {
        [Header("Animations")]
        [SerializeField]
        private float outAnimDuration;

        [Header("Buttons")]
        [SerializeField]
        private Button playButton;
        [SerializeField]
        private Button rankingButton;


        private const string ANIMATION_ID_OUT = "Menu_Out";
        private const string CURRENT_SCENE_NAME = "Menu";
        private const string NEXT_SCENE_NAME = "Game";


        private void Awake()
        {
            playButton.onClick.AddListener(OnPlayButtonPressed);
            rankingButton.onClick.AddListener(OnRankingButtonPressed);
        }

        private void OnPlayButtonPressed()
        {
            playButton.interactable = false;
            rankingButton.interactable = false;

            SceneManager.LoadSceneAsync(NEXT_SCENE_NAME, LoadSceneMode.Additive).completed += OnGameSceneLoaded;
        }

        private void OnRankingButtonPressed()
        {
            // TODO_VICTOR: open ranking through Google Play Games service
        }

        private void OnGameSceneLoaded(AsyncOperation pOperation)
        {
            if (pOperation.isDone)
            {
                StartCoroutine(GoToGame());
            }
            else
            {
                Debug.LogErrorFormat("LoadingScreen: failed to load '{0}' scene.", NEXT_SCENE_NAME);
            }
        }

        private IEnumerator GoToGame()
        {
            DOTween.Play(ANIMATION_ID_OUT);

            yield return new WaitForSeconds(outAnimDuration);

            SceneManager.UnloadSceneAsync(CURRENT_SCENE_NAME);
        }
    }
}