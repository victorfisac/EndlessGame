#pragma warning disable 0649


using EndlessGame.Audio;
using UnityEngine;
using UnityEngine.UI;


namespace EndlessGame.Components
{
    public class AudioToggleComponent : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Sprite m_soundOnSprite;
        [SerializeField]
        private Sprite m_soundOffSprite;

        [Header("References")]
        [SerializeField]
        private Image m_image;
        [SerializeField]
        private Button m_button;


        private AudioManager m_manager = null;


        private void Awake()
        {
            m_manager = AudioManager.Instance;
            m_button.onClick.AddListener(OnButtonPressed);

            ApplyAudioChanges();
        }

        private void OnButtonPressed()
        {
            m_manager.Enabled = !m_manager.Enabled;

            ApplyAudioChanges();

            m_manager.Play(ClipType.BUTTON_PRESSED);
        }

        private void ApplyAudioChanges()
        {
            AudioListener.volume = (m_manager.Enabled ? 1f : 0f);

            m_image.sprite = (m_manager.Enabled ? m_soundOnSprite : m_soundOffSprite);
        }
    }
}