#pragma warning disable 0649


using System.Collections;
using DG.Tweening;
using EndlessGame.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace EndlessGame.Screens
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


        private int m_score = 0;
        private bool m_isBestScore = false;
        private AudioManager m_audioManager = null;

        private const float SCORE_ANIM_DELAY = 1f;
        private const float CELEBRATION_ANIM_DELAY = 0.5f;
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
            m_audioManager = AudioManager.Instance;
            retryButton.onClick.AddListener(OnRetryButtonPressed);
            rankingButton.onClick.AddListener(OnRankingButtonPressed);
            exitButton.onClick.AddListener(OnExitButtonPressed);
        }

        private void Start()
        {
            m_score = PlayerPrefs.GetInt(PLAYERPREFS_SCORE);
            
            AnimateScore();
            CheckBestScore();

            if (m_isBestScore)
            {
                StartCoroutine(OpenCelebration());
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.enabled = false;
                OnExitButtonPressed();
            }
        }

        private void OnRetryButtonPressed()
        {
            retryButton.interactable = false;
            rankingButton.interactable = false;
            exitButton.interactable = false;

            m_audioManager.Play(ClipType.GAME_START);

            SceneManager.LoadSceneAsync(NEXT_SCENE_NAME, LoadSceneMode.Additive).completed += OnGameSceneLoaded;
        }

        private void OnRankingButtonPressed()
        {
            m_audioManager.Play(ClipType.BUTTON_PRESSED);


            // TODO_VICTOR: open ranking through Google Play Games service
        }

        private void OnExitButtonPressed()
        {
            retryButton.interactable = false;
            rankingButton.interactable = false;
            exitButton.interactable = false;

            m_audioManager.Play(ClipType.BUTTON_PRESSED);

            StartCoroutine(GoToMenu());
        }

        private void AnimateScore()
        {
            int _scoreCounter = 0;

            DOTween.To(() => { return _scoreCounter; }, (x) => { _scoreCounter = x; }, m_score, scoreAnimDuration)
            .SetDelay(SCORE_ANIM_DELAY).
            OnPlay(() => {
                if (m_score > 5)
                {
                    m_audioManager.Play(ClipType.GAMEPLAY_SCORING);
                }
            })
            .OnUpdate(() => {
                scoreText.text = _scoreCounter.ToString();
            });
        }

        private void CheckBestScore()
        {
            int _bestScore = 0;

            if (PlayerPrefs.HasKey(PLAYERPREFS_BEST_SCORE))
            {
                _bestScore = PlayerPrefs.GetInt(PLAYERPREFS_BEST_SCORE);
            }

            if (_bestScore > 0)
            {
                bestScoreCnt.SetActive(true);
                bestScoreText.text = _bestScore.ToString();
            }

            m_isBestScore = (m_score > _bestScore);
            
            if (m_isBestScore)
            {
                PlayerPrefs.SetInt(PLAYERPREFS_BEST_SCORE, m_score);
                PlayerPrefs.Save();
            }
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

        private IEnumerator OpenCelebration()
        {
            yield return new WaitForSeconds(SCORE_ANIM_DELAY + scoreAnimDuration*0.75f);

            m_audioManager.Play(ClipType.CELEBRATION);

            SceneManager.LoadSceneAsync(CELEBRATION_SCENE_NAME, LoadSceneMode.Additive).completed += (op) => {
                if (op.isDone)
                {
                    Debug.LogFormat("EndScreen: loaded Celebration scene with success.");
                }
                else
                {
                    Debug.LogError("EndScreen: failed to load Celebration scene.");
                }
            };
        }
    }
}