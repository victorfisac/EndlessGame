using System;
using EndlessGame.Audio;
using GoogleMobileAds.Api;
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

        #if !UNITY_EDITOR
        private InterstitialAd m_interstitialView = null;
        private BannerView m_bannerView = null;
        #endif
        

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
            MobileAds.Initialize(OnAdmobInitialized);

            Debug.LogFormat("GoogleAdmobService: initialization completed ('{0}' testing devices).", m_data.testingDevices.Length);
        }

        public void ShowIntersticial()
        {
            #if !UNITY_EDITOR
            if (m_bannerView != null)
            {
                Debug.LogWarning("GoogleAdmobService: already showing an intersticial advertisement.");
                return;
            }
            
            m_interstitialView = new InterstitialAd(m_data.intersticialId);

            AdRequest _request = CreateAdRequest();
            m_interstitialView.OnAdLoaded += OnIntersticialLoaded;
            m_interstitialView.OnAdFailedToLoad += OnIntersticialFailed;
            m_interstitialView.OnAdClosed += OnIntersticialClosed;

            m_interstitialView.LoadAd(_request);
            #endif
        }

        public void ShowBanner()
        {
            #if !UNITY_EDITOR
            if (m_bannerView != null)
            {
                Debug.LogWarning("GoogleAdmobService: already showing a banner advertisement.");
                return;
            }
            
            m_bannerView = new BannerView(m_data.bannerId, AdSize.SmartBanner, AdPosition.Bottom);

            AdRequest _request = CreateAdRequest();
            m_bannerView.OnAdLoaded += OnBannerLoaded;
            m_bannerView.OnAdFailedToLoad += OnBannerFailed;

            m_bannerView.LoadAd(_request);
            #endif
        }

        public void HideBanner()
        {
            #if !UNITY_EDITOR
            if (m_bannerView == null)
            {
                Debug.LogWarning("GoogleAdmobService: there is no banner advertisement to hide.");
                return;
            }
            
            m_bannerView.Hide();
            m_bannerView.Destroy();

            m_bannerView = null;
            #endif
        }

        public void SetVolume(bool pActive)
        {
            if (!m_initialized)
            {
                Debug.LogWarning("GoogleAdmobService: service is not initialized yet.");
                return;
            }

            MobileAds.SetApplicationMuted(!pActive);
        }

        private void OnAdmobInitialized(InitializationStatus pResult)
        {
            m_initializing = false;
            m_initialized = (pResult != null);

            if (m_initialized)
            {
                MobileAds.SetApplicationMuted(AudioManager.Instance.Enabled);

                Debug.Log("GoogleAdmobService: initialized service with success.");
            }
            #if !UNITY_EDITOR
            else
            {
                Debug.LogError("GoogleAdmobService: failed to initialize service.");
            }
            #endif

            if (m_onServiceInitialized != null)
            {
                m_onServiceInitialized(m_initialized);
            }
        }

        private void OnBannerLoaded(object pSender, EventArgs pArguments)
        {
            Debug.Log("GoogleAdmobService: loaded banner advertisement with success.");
        }

        private void OnBannerFailed(object pSender, AdFailedToLoadEventArgs pArguments)
        {
            Debug.LogErrorFormat("GoogleAdmobService: failed to load banner advertisement ({0}).", pArguments.Message);
        }

        private void OnIntersticialLoaded(object pSender, EventArgs pArguments)
        {
            Debug.Log("GoogleAdmobService: loaded intersticial advertisement with success.");
        }

        private void OnIntersticialFailed(object pSender, AdFailedToLoadEventArgs pArguments)
        {
            Debug.LogErrorFormat("GoogleAdmobService: failed to load intersticial advertisement ({0}).", pArguments.Message);
        }

        private void OnIntersticialClosed(object pSender, EventArgs pArguments)
        {
            #if !UNITY_EDITOR
            m_interstitialView.Destroy();
            m_interstitialView = null;
            #endif
            
            Debug.Log("GoogleAdmobService: closed intersticial advertisement with success.");
        }

        private AdRequest CreateAdRequest()
        {
            AdRequest.Builder _builder = new AdRequest.Builder();

            for (int i = 0, count = m_data.testingDevices.Length; i < count; i++)
            {
                _builder.AddTestDevice(m_data.testingDevices[i]);
            }

            AdRequest _request = _builder.Build();

            return _request;
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