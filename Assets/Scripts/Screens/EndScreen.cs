﻿#pragma warning disable 0649


using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace EndlesGame.Screens
{
    public class EndScreen : MonoBehaviour
    {
        [Header("Animations")]
        [SerializeField]
        private float scoreAnimDuration;
        [SerializeField]
        private float outAnimDuration;
        [SerializeField]
        private float endAnimDuration;

        [Header("Buttons")]
        [SerializeField]
        private Button retryButton;
        [SerializeField]
        private Button rankingButton;
        [SerializeField]
        private Button exitButton;

        [Header("References")]
        [SerializeField]
        private Text scoreText;
        [SerializeField]
        private Text bestScoreText;
        [SerializeField]
        private GameObject bestScoreCnt;


        private bool m_isBestScore = false;

        private const float SCORE_ANIM_DELAY = 1f;
        private const string PLAYERPREFS_SCORE = "SCORE";
        private const string PLAYERPREFS_BEST_SCORE = "BEST_SCORE";
        private const string ANIMATION_ID_OUT = "End_Out";
        private const string ANIMATION_ID_END = "End_End";
        private const string CURRENT_SCENE_NAME = "End";
        private const string NEXT_SCENE_NAME = "Game";
        private const string MAIN_SCENE_NAME = "Main";
        private const string CELEBRATION_SCENE_NAME = "Celebration";


        private void Awake()
        {
            retryButton.onClick.AddListener(OnRetryButtonPressed);
            rankingButton.onClick.AddListener(OnRankingButtonPressed);
            exitButton.onClick.AddListener(OnExitButtonPressed);
        }

        private void Start()
        {
            int _score = PlayerPrefs.GetInt(PLAYERPREFS_SCORE);
            int _scoreCounter = 0;

            DOTween.To(() => { return _scoreCounter; }, (x) => { _scoreCounter = x; }, _score, scoreAnimDuration)
            .SetDelay(SCORE_ANIM_DELAY)
            .OnUpdate(() => {
                scoreText.text = _scoreCounter.ToString();
            });

            int _bestScore = PlayerPrefs.GetInt(PLAYERPREFS_BEST_SCORE);
            m_isBestScore = (_score > _bestScore);
            
            if (m_isBestScore)
            {
                PlayerPrefs.SetInt(PLAYERPREFS_BEST_SCORE, _score);
                
                if (_bestScore > 0)
                {
                    bestScoreCnt.SetActive(m_isBestScore);
                    bestScoreText.text = _bestScore.ToString();
                }

                SceneManager.LoadSceneAsync(CELEBRATION_SCENE_NAME, LoadSceneMode.Additive);
            }
        }

        private void OnRetryButtonPressed()
        {
            retryButton.interactable = false;
            rankingButton.interactable = false;
            exitButton.interactable = false;

            SceneManager.LoadSceneAsync(NEXT_SCENE_NAME, LoadSceneMode.Additive).completed += OnGameSceneLoaded;
        }

        private void OnRankingButtonPressed()
        {
            // TODO_VICTOR: open ranking through Google Play Games service
        }

        private void OnExitButtonPressed()
        {
            retryButton.interactable = false;
            rankingButton.interactable = false;
            exitButton.interactable = false;

            StartCoroutine(GoToMenu());
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

            if (m_isBestScore)
            {
                SceneManager.UnloadSceneAsync(CELEBRATION_SCENE_NAME);
            }
        }

        private IEnumerator GoToMenu()
        {
            DOTween.Play(ANIMATION_ID_END);

            yield return new WaitForSeconds(endAnimDuration);

            SceneManager.LoadSceneAsync(MAIN_SCENE_NAME, LoadSceneMode.Single);
        }
    }
}