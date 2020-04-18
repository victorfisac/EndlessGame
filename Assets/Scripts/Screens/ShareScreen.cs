#pragma warning disable 0649


using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EndlessGame.Plugins;
using System.Collections;


namespace EndlessGame.Screens
{
    public class ShareScreen : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private string shareTitle;
        [SerializeField]
        private string shareSubject;
        [SerializeField]
        private string shareMessage;

        [Header("Bounds")]
        [SerializeField]
        private RectTransform renderBoundsA;
        [SerializeField]
        private RectTransform renderBoundsB;

        [Header("Refereces")]
        [SerializeField]
        private Text scoreText;


        private const string CURRENT_SCENE_NAME = "Share";
        private const string PLAYERPREFS_BEST_SCORE = "BEST_SCORE";
        private const string SHARE_IMAGE_FILENAME = "/share_image.png";


        private void Awake()
        {
            gameObject.SetActive(true);

            int _bestScore = PlayerPrefs.GetInt(PLAYERPREFS_BEST_SCORE);
            scoreText.text = _bestScore.ToString();

            StartCoroutine(ShareProcess());
        }

        private IEnumerator ShareProcess()
        {
            yield return new WaitForEndOfFrame();

            RenderTextureAndSave();

            OpenShareDialog();

            SceneManager.UnloadSceneAsync(CURRENT_SCENE_NAME);
        }

        private void RenderTextureAndSave()
        {
            Rect _screenRect = BoundsToScreenSpace();
            
            Texture2D _renderTex = new Texture2D((int)_screenRect.width, (int)_screenRect.height, TextureFormat.RGB24, false);
            _renderTex.ReadPixels(_screenRect, 0, 0);
            
            byte[] _pixels = _renderTex.EncodeToPNG();
            string _savePath = string.Concat(Application.persistentDataPath, SHARE_IMAGE_FILENAME);

            File.WriteAllBytes(_savePath, _pixels);
            Destroy(_renderTex);

            gameObject.SetActive(false);
        }

        private void OpenShareDialog()
        {
            string _message = string.Format(shareMessage, scoreText.text, GetDownloadLink());
            string _filePath = string.Concat(Application.persistentDataPath, SHARE_IMAGE_FILENAME);

            NativeShare _share = new NativeShare();
            _share.SetTitle(shareTitle);
            _share.SetSubject(shareSubject);
            _share.SetText(_message);
            _share.AddFile(_filePath);

            _share.Share();
        }

        private Rect BoundsToScreenSpace()
        {
            Rect _rect = new Rect();

            Vector3 _a = Camera.main.WorldToScreenPoint(renderBoundsA.position);
            Vector3 _b = Camera.main.WorldToScreenPoint(renderBoundsB.position);

            _rect.x = _a.x;
            _rect.y = _a.y;
            _rect.width = (int)Mathf.Abs(_b.x - _a.x);
            _rect.height = (int)Mathf.Abs(_b.y - _a.y);
            
            return _rect;
        }

        private string GetDownloadLink()
        {
            return string.Concat("https://play.google.com/store/apps/details?id=", Application.identifier);
        }
    }
}