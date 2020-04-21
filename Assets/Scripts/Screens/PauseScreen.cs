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
    public class PauseScreen : MonoBehaviour
    {
        [Header("Animations")]
        [SerializeField]
        private float outAnimDuration;
        [SerializeField]
        private float endAnimDuration;

        [Header("Buttons")]
        [SerializeField]
        private Button continueButton;
        [SerializeField]
        private Button exitButton;


        private AudioManager m_audioManager = null;

        private const string ANIMATION_ID_OUT = "Pause_Out";
        private const string ANIMATION_ID_END = "Pause_End";
        private const string CURRENT_SCENE_NAME = "Pause";
        private const string NEXT_SCENE_NAME = "Main";


        private void Awake()
        {
            m_audioManager = AudioManager.Instance;
            continueButton.onClick.AddListener(OnContinueButtonPressed);
            exitButton.onClick.AddListener(OnExitButtonPressed);

            GoogleAdmobService.Instance.ShowBanner();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.enabled = false;
                OnExitButtonPressed();
            }
        }

        private void OnContinueButtonPressed()
        {
            continueButton.interactable = false;
            exitButton.interactable = false;

            m_audioManager.Play(ClipType.BUTTON_PRESSED);

            GoogleAdmobService.Instance.HideBanner();

            StartCoroutine(GoToGame());
        }

        private void OnExitButtonPressed()
        {
            continueButton.interactable = false;
            exitButton.interactable = false;

            m_audioManager.Play(ClipType.BUTTON_PRESSED);

            GoogleAdmobService.Instance.HideBanner();

            StartCoroutine(GoToMenu());
        }

        private IEnumerator GoToGame()
        {
            DOTween.Play(ANIMATION_ID_OUT);

            yield return new WaitForSeconds(outAnimDuration);

            SceneManager.UnloadSceneAsync(CURRENT_SCENE_NAME);
        }

        private IEnumerator GoToMenu()
        {
            DOTween.Play(ANIMATION_ID_END);

            yield return new WaitForSeconds(endAnimDuration);

            SceneManager.LoadSceneAsync(NEXT_SCENE_NAME, LoadSceneMode.Single);
        }
    }
}