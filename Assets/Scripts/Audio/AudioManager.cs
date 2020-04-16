using System.Collections.Generic;
using DG.Tweening;
using EndlessGame.AssetBundles;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace EndlessGame.Audio
{
    public interface IAudioManager
    {
        void Play(ClipType pType);
        bool Enabled { get; set; }
    }


    public class AudioManager : MonoBehaviour, IAudioManager
    {
        private static AudioManager m_instance = null;


        private bool m_initalized = false;
        private bool m_enabled = true;
        private AudioData m_data = null;
        private List<AudioChannel> m_channels = new List<AudioChannel>();
        

        private const string AUDIO_ASSET_BUNDLE = "audio.ab";
        private const string AUDIO_ASSET_NAME = "AudioData";
        private const string MENU_SCENE_NAME = "Menu";
        private const string PLAYERPREFS_SOUND = "SOUND";


        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;

                SceneManager.sceneLoaded += OnMainSceneLoaded;

                if (PlayerPrefs.HasKey(PLAYERPREFS_SOUND))
                {
                    m_enabled = (PlayerPrefs.GetInt(PLAYERPREFS_SOUND) == 1);
                }
                else
                {
                    PlayerPrefs.SetInt(PLAYERPREFS_SOUND, 1);
                }
            }
        }

        public void Play(ClipType pType)
        {
            if (!m_initalized)
            {
                Debug.LogErrorFormat("AudioManager: initialization is still not completed (skipping audio '{0}').", pType);
                return;
            }

            AudioChannel _channel = GetAvailableAudioSource();
            AudioDataElement _data = GetAudioDataByType(pType);

            _channel.Available = false;
            _channel.Source.playOnAwake = false;
            _channel.Source.clip = _data.Clip;
            _channel.Source.loop = _data.Loop;
            _channel.Source.volume = _data.Volume;

            if (_channel.Source.clip != null)
            {
                _channel.Source.Play();

                DOTween.To(() => 0, (x) => {}, 0, _channel.Source.clip.length).OnComplete(() => {
                    _channel.Available = true;
                    _channel.Source.clip = null;
                });
            }
        }

        private AudioChannel CreateAudioChannel()
        {
            AudioChannel _newChannel = new AudioChannel();
            _newChannel.Source = gameObject.AddComponent<AudioSource>();

            m_channels.Add(_newChannel);

            return _newChannel;
        }

        private AudioChannel GetAvailableAudioSource()
        {
            AudioChannel _result = null;

            for (int i = 0, count = m_channels.Count; i < count; i++)
            {
                if (m_channels[i].Available)
                {
                    _result = m_channels[i];
                    break;
                }
            }

            if (_result == null)
            {
                _result = CreateAudioChannel();
            }

            return _result;
        }

        private AudioDataElement GetAudioDataByType(ClipType pType)
        {
            AudioDataElement _result = null;

            for (int i = 0, count = m_data.Clips.Length; i < count; i++)
            {
                if (m_data.Clips[i].Type.Equals(pType))
                {
                    _result = m_data.Clips[i];
                    break;
                }
            }

            if (_result == null)
            {
                Debug.LogErrorFormat("AudioManager: could not found audio clip for type '{0}'.", pType);
            }

            return _result;
        }

        private void OnMainSceneLoaded(Scene pScene, LoadSceneMode pLoadSceneMode)
        {
            if (pScene.name.Equals(MENU_SCENE_NAME))
            {
                AssetBundlesProvider.Instance.LoadAssetBundle(AUDIO_ASSET_BUNDLE, OnAssetBundleLoaded);

                SceneManager.sceneLoaded -= OnMainSceneLoaded;
            }
        }

        private void OnAssetBundleLoaded(AssetBundle pAssetBundle)
        {
            AssetBundleRequest _request = pAssetBundle.LoadAssetAsync(AUDIO_ASSET_NAME);
            _request.completed += (op) => {
                m_data = _request.asset as AudioData;
                m_initalized = true;

                Play(ClipType.MUSIC);
            };
        }


        public static AudioManager Instance
        {
            get { return m_instance; }
        }

        public bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }


        public class AudioChannel
        {
            public bool Available = true;
            public AudioSource Source;
        }
    }
}