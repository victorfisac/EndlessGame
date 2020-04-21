#pragma warning disable 0649


using EndlessGame.Services;
using UnityEngine;


namespace EndlessGame.Components
{
    public class GoogleAdmobComponent : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private GoogleAdmobData config;


        private GoogleAdmobService m_service = null;


        private void Start()
        {
            m_service = GoogleAdmobService.Instance;
            m_service.OnServiceInitializedCallback += OnServiceInitialized;
            m_service.Initialize(config);
        }

        private void OnDestroy()
        {
            m_service.OnServiceInitializedCallback -= OnServiceInitialized;
        }

        private void OnServiceInitialized(bool pSuccess)
        {
            if (pSuccess)
            {
                m_service.ShowBanner();
            }
        }
    }
}