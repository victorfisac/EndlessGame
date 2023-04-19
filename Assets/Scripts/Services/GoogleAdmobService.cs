using System;
using EndlessGame.Audio;
using UnityEngine;


namespace EndlessGame.Services
{
    public delegate void OnServiceInitialized(bool pSuccess);


    public interface IGoogleAdmobService
    {
        void Initialize(GoogleAdmobData pData);
        void ShowIntersticial();
        void ShowBanner();
        void HideBanner();
        void SetVolume(bool pActive);

        OnServiceInitialized OnServiceInitializedCallback { get; set; }
        bool Initialized { get; }
    }
    

    public class GoogleAdmobService : IGoogleAdmobService
    {
        private static GoogleAdmobService m_instance = null;

        private bool m_initialized = false;
        private bool m_initializing = false;
        private OnServiceInitialized m_onServiceInitialized = null;
        private GoogleAdmobData m_data = null;


        public void Initialize(GoogleAdmobData pData)
        {
            if (m_initialized || m_initializing)
            {
                if (m_onServiceInitialized != null)
                {
                    m_onServiceInitialized(m_initialized);
                }

                return;
            }

            if (pData == null || string.IsNullOrEmpty(pData.bannerId) || string.IsNullOrEmpty(pData.intersticialId))
            {
                Debug.LogError("GoogleAdmobService: provided invalid identifiers for advertisements.");
                return;
            }

            m_data = pData;
            m_initializing = true;

            Debug.LogFormat("GoogleAdmobService: initialization completed ('{0}' testing devices).", m_data.testingDevices.Length);
        }

        public void ShowIntersticial()
        {
            
        }

        public void ShowBanner()
        {
            
        }

        public void HideBanner()
        {
            
        }

        public void SetVolume(bool pActive)
        {
            
        }

        public static GoogleAdmobService Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new GoogleAdmobService();
                }

                return m_instance;
            }
        }

        public OnServiceInitialized OnServiceInitializedCallback
        {
            get { return m_onServiceInitialized; }
            set { m_onServiceInitialized = value; }
        }

        public bool Initialized
        {
            get { return m_initialized; }
        }
    }
}