using UnityEngine;


namespace EndlessGame.Game
{
    [CreateAssetMenu(fileName = "NewGameConfig", menuName = "EndlessGame/Create Game Config data")]
    public class GameConfig : ScriptableObject
    {
        [Header("Circle")]
        public float CircleSpeed;

        [Header("Ball")]
        public float BallSpeed;
        public float BallSpeedFactor;

        [Header("Colors")]
        public Color[] Colors;
        public int InitColors;
        public int ColorsFactor;
    }
}