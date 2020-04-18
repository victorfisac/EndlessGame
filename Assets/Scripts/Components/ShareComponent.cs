#pragma warning disable 0649


using EndlessGame.Plugins;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace EndlessGame.Components
{
    public class ShareComponent : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private string shareTitle;
        [SerializeField]
        private string shareSubject;
        [SerializeField]
        private string shareMessage;

        [Header("References")]
        [SerializeField]
        private Button button;


        public const string SHARE_IMAGE_FILENAME = "/share_image.png";
        private const string PLAYERPREFS_BEST_SCORE = "BEST_SCORE";
        private const string SHARE_SCENE_NAME = "Share";


        private void Awake()
        {
            button.onClick.AddListener(OnShareButtonPressed);
        }

        private void OnShareButtonPressed()
        {
            button.interactable = false;

            SceneManager.LoadSceneAsync(SHARE_SCENE_NAME, LoadSceneMode.Additive).completed += OnShareSceneLoaded;
        }
        
        private void OnShareSceneLoaded(AsyncOperation pOperation)
        {
            if (pOperation.isDone)
            {
                SceneManager.sceneUnloaded += OnShareSceneUnloaded;
            }
            else
            {
                Debug.LogErrorFormat("LoadingScreen: failed to load '{0}' scene.", SHARE_SCENE_NAME);
            }
        }

        private void OnShareSceneUnloaded(Scene pScene)
        {
            if (pScene.name.Equals(SHARE_SCENE_NAME))
            {
                SceneManager.sceneUnloaded -= OnShareSceneUnloaded;

                OpenShareDialog();
                button.interactable = true;
            }
        }

        private void OpenShareDialog()
        {
            int _bestScore = PlayerPrefs.GetInt(PLAYERPREFS_BEST_SCORE);

            string _message = string.Format(shareMessage, _bestScore, GetDownloadLink());
            string _filePath = string.Concat(Application.persistentDataPath, SHARE_IMAGE_FILENAME);

            NativeShare _share = new NativeShare();
            _share.SetTitle(shareTitle);
            _share.SetSubject(shareSubject);
            _share.SetText(_message);
            _share.AddFile(_filePath);

            _share.Share();
        }

        private string GetDownloadLink()
        {
            return string.Concat("https://play.google.com/store/apps/details?id=", Application.identifier);
        }
    }
}