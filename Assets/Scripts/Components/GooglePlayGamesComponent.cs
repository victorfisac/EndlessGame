#pragma warning disable 0649


using EndlessGame.Services;
using UnityEngine;


namespace EndlessGame.Components
{
    public class GooglePlayGamesComponent : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private GooglePlayGamesData config;


        private void Start()
        {
            GooglePlayGamesService _service = GooglePlayGamesService.Instance;
            _service.Initialize(config);
        }
    }
}