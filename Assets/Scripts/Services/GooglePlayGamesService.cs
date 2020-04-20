using System;
using GooglePlayGames;
using UnityEngine;


namespace EndlessGame.Services
{
    public class GooglePlayGamesService
    {
        private static GooglePlayGamesService m_instance = null;

        private bool m_initialized = false;
        private bool m_signedIn = false;
        private GooglePlayGamesData m_data = null;

        private const string LEADERBOARD_ID = "CgkI4aminN4PEAIQAQ";
        private const string ACHIEVEMENT_ID_FIRST_TRY = "CgkI4aminN4PEAIQAg";
        private const string ACHIEVEMENT_ID_SHARE = "CgkI4aminN4PEAIQBg";
        private const string PLAYERPREFS_BEST_SCORE = "BEST_SCORE";


        public void Initialize(GooglePlayGamesData pData)
        {
            if (m_initialized)
            {
                return;
            }

            m_data = pData;
            m_initialized = true;

            Debug.Log("GooglePlayGamesService: initialization completed.");
        }


        public void SignIn(Action<bool> pOnSignInCompleted = null)
        {
            Debug.Log("GooglePlayGamesService: authentication process started.");

            PlayGamesPlatform.Instance.Authenticate((result) => {
                m_signedIn = result;

                Debug.LogFormat("GooglePlayGamesService: authentication process completed with result '{0}'.", result);

                if (pOnSignInCompleted != null)
                {
                    pOnSignInCompleted(result);
                }
            });
        }

        public void ShowLeaderboard()
        {
            if (m_signedIn)
            {
                Debug.Log("GooglePlayGamesService: opening native leaderboard interface.");
                
                PlayGamesPlatform.Instance.ShowLeaderboardUI(LEADERBOARD_ID);
            }
            else
            {
                SignIn((result) => {
                    if (result)
                    {
                        int _bestScore = PlayerPrefs.GetInt(PLAYERPREFS_BEST_SCORE);
                        ReportScore(_bestScore, ShowLeaderboard);
                    }
                });
            }
        }

        public void ReportScore(int pScore, Action OnScoreReportedCallback = null)
        {
            if (!m_signedIn)
            {
                return;
            }

            for (int i = 0, count = m_data.Achievements.Length; i < count; i++)
            {
                GooglePlayGamesElement _element = m_data.Achievements[i];

                if (pScore >= _element.Score)
                {
                    PlayGamesPlatform.Instance.ReportProgress(_element.AchievementId, 100f, (result) => OnAchievementReported(result, _element.AchievementId));
                }
            }

            PlayGamesPlatform.Instance.ReportProgress(ACHIEVEMENT_ID_FIRST_TRY, 100f, (result) => OnAchievementReported(result, ACHIEVEMENT_ID_FIRST_TRY));

            PlayGamesPlatform.Instance.ReportScore(pScore, LEADERBOARD_ID, (result) => {
                OnScoreReported(result, pScore);

                if (OnScoreReportedCallback != null)
                {
                    OnScoreReportedCallback();
                }
            });
        }

        public void ReportAchievementShare()
        {
            if (m_signedIn)
            {
                return;
            }

            PlayGamesPlatform.Instance.ReportProgress(ACHIEVEMENT_ID_SHARE, 100f, (result) => OnAchievementReported(result, ACHIEVEMENT_ID_SHARE));
        }

        private void OnScoreReported(bool pSuccess, int pScore)
        {
            if (pSuccess)
            {
                Debug.LogFormat("GooglePlayGamesService: reported score '{0}' with success.", pScore);
            }
            else
            {
                Debug.LogErrorFormat("GooglePlayGamesService: failed to report score '{0}'.", pScore);
            }
        }

        private void OnAchievementReported(bool pSuccess, string pAchievementId)
        {
            if (pSuccess)
            {
                Debug.LogFormat("GooglePlayGamesService: reported achievement '{0}' with success.", pAchievementId);
            }
            else
            {
                Debug.LogErrorFormat("GooglePlayGamesService: failed to report achievement '{0}'.", pAchievementId);
            }
        }

        public static GooglePlayGamesService Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new GooglePlayGamesService();
                }

                return m_instance;
            }
        }
    }
}