﻿#pragma warning disable 0649


using System.Collections;
using DG.Tweening;
using EndlessGame.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace EndlesGame.Screens
{
    public class GameScreen : MonoBehaviour
    {
        [Header("Countdown")]
        [SerializeField]
        private Text countdownText;
        [SerializeField]
        private float countAnimDuration;
        [SerializeField]
        private float countAnimDelay;

        [Header("Animation")]
        [SerializeField]
        private float outAnimDuration;
        [SerializeField]
        private float resumeAnimDuration;

        [Header("Buttons")]
        [SerializeField]
        private Button pauseButton;

        [Header("References")]
        [SerializeField]
        private BlurredScreenshotComponent blurredScreenshot;


        private int m_currentCount = 3;

        private const string ANIMATION_ID_COUNT = "Countdown";
        private const string ANIMATION_ID_PAUSE = "Game_Pause";
        private const string ANIMATION_ID_OUT = "Game_Out";
        private const string CURRENT_SCENE_NAME = "Game";
        private const string NEXT_SCENE_NAME = "End";
        private const string PAUSE_SCENE_NAME = "Pause";
        private const string PLAYERPREFS_SCORE = "SCORE";
        private const string PLAYERPREFS_BEST_SCORE = "BEST_SCORE";


        private void Awake()
        {
            pauseButton.onClick.AddListener(OnPauseButtonPressed);
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(countAnimDelay*1.25f);

            DOTween.Play(ANIMATION_ID_COUNT);

            while (m_currentCount > 0)
            {
                yield return new WaitForSeconds(countAnimDuration);

                m_currentCount--;
                countdownText.text = m_currentCount.ToString();
            }

            countdownText.text = "GO!";

            yield return new WaitForSeconds(countAnimDuration);

            countdownText.gameObject.SetActive(false);

            // TODO_VICTOR: enable gameplay manager
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnGameplayEnd(720);
            }
        }

        private void OnPauseButtonPressed()
        {
            pauseButton.interactable = false;

            // TODO_VICTOR: disable gameplay manager

            blurredScreenshot.ShowBlurredBackground();

            SceneManager.LoadSceneAsync(PAUSE_SCENE_NAME, LoadSceneMode.Additive).completed += OnPauseSceneLoaded;
        }

        private void OnPauseSceneLoaded(AsyncOperation pOperation)
        {
            if (pOperation.isDone)
            {
                pauseButton.interactable = true;

                DOTween.Play(ANIMATION_ID_PAUSE);
                SceneManager.sceneUnloaded += OnPauseSceneUnloaded;
            }
            else
            {
                Debug.LogErrorFormat("LoadingScreen: failed to load '{0}' scene.", PAUSE_SCENE_NAME);
            }
        }

        private void OnPauseSceneUnloaded(Scene pScene)
        {
            if (pScene.name.Equals(PAUSE_SCENE_NAME))
            {
                StartCoroutine(ResumeGame());
            }

            SceneManager.sceneUnloaded -= OnPauseSceneUnloaded;
        }

        private IEnumerator ResumeGame()
        {
            blurredScreenshot.HideBlurredBackground();

            yield return new WaitForSeconds(resumeAnimDuration);

            // TODO_VICTOR: enable gameplay manager
        }

        private void OnGameplayEnd(int pScore)
        {
            // TODO_VICTOR: disable gameplay manager
            
            PlayerPrefs.SetInt(PLAYERPREFS_SCORE, pScore);

            pauseButton.interactable = false;

            SceneManager.LoadSceneAsync(NEXT_SCENE_NAME, LoadSceneMode.Additive).completed += OnEndSceneLoaded;
        }

        private void OnEndSceneLoaded(AsyncOperation pOperation)
        {
            if (pOperation.isDone)
            {
                StartCoroutine(GoToEnd());
            }
            else
            {
                Debug.LogErrorFormat("LoadingScreen: failed to load '{0}' scene.", PAUSE_SCENE_NAME);
            }
        }

        private IEnumerator GoToEnd()
        {
            DOTween.Play(ANIMATION_ID_OUT);

            yield return new WaitForSeconds(outAnimDuration);

            SceneManager.UnloadSceneAsync(CURRENT_SCENE_NAME);
        }
    }
}