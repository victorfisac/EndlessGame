#pragma warning disable 0649


using System.Collections;
using DG.Tweening;
using EndlessGame.Audio;
using EndlessGame.Services;
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
        [SerializeField]
        private float endAnimDuration;

        [Header("Buttons")]
        [SerializeField]
        private Button playButton;
        [SerializeField]
        private Button rankingButton;


        private AudioManager m_audioManager = null;

        private const string ANIMATION_ID_OUT = "Menu_Out";
        private const string ANIMATION_ID_END = "Menu_End";
        private const string CURRENT_SCENE_NAME = "Menu";
        private const string NEXT_SCENE_NAME = "Game";


        private void Awake()
        {
            m_audioManager = AudioManager.Instance;
            playButton.onClick.AddListener(OnPlayButtonPressed);
            rankingButton.onClick.AddListener(OnRankingButtonPressed);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.enabled = false;
                StartCoroutine(GoToExit());
            }
        }

        private void OnPlayButtonPressed()
        {
            playButton.interactable = false;
            rankingButton.interactable = false;

            m_audioManager.Play(ClipType.GAME_START);

            SceneManager.LoadSceneAsync(NEXT_SCENE_NAME, LoadSceneMode.Additive).completed += OnGameSceneLoaded;
        }

        private void OnRankingButtonPressed()
        {
            m_audioManager.Play(ClipType.BUTTON_PRESSED);

            GooglePlayGamesService.Instance.ShowLeaderboard();
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

        private IEnumerator GoToExit()
        {
            DOTween.Play(ANIMATION_ID_END);

            yield return new WaitForSeconds(endAnimDuration);

            Application.Quit();
        }
    }
}