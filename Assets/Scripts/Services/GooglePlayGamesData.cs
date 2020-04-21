using UnityEngine;
using System;


namespace EndlessGame.Services
{
    [CreateAssetMenu(fileName = "NewGooglePlayGamesData", menuName = "EndlessGame/Create Google Play Games data")]
    public class GooglePlayGamesData : ScriptableObject
    {
        public GooglePlayGamesElement[] Achievements = new GooglePlayGamesElement[0];
    }

    [Serializable]
    public class GooglePlayGamesElement
    {
        public int Score;
        public string AchievementId;
    }
}