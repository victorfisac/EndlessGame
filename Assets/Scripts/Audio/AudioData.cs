using UnityEngine;
using System;


namespace EndlessGame.Audio
{
    public enum ClipType { MUSIC = -1, BUTTON_PRESSED, GAME_START, GAME_END, CELEBRATION, GAMEPLAY_COLLISION, GAMEPLAY_COUNTDOWN, GAMEPLAY_SCORING }


    [CreateAssetMenu(fileName = "NewAudioData", menuName = "EndlessGame/Create AudioData data")]
    public class AudioData : ScriptableObject
    {
        public AudioDataElement[] Clips;
    }

    [Serializable]
    public class AudioDataElement
    {
        public ClipType Type;
        public AudioClip Clip;
        public bool Loop;
        [Range(0f, 1f)]
        public float Volume = 1f;
        public bool Pitch;
        public Vector2 RandomPitch = Vector2.zero;
    }
}