#pragma warning disable 0649


using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using EndlessGame.Components;


namespace EndlessGame.Screens
{
    public class ShareScreen : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Text scoreText;
        [SerializeField]
        private RectTransform renderBoundsA;
        [SerializeField]
        private RectTransform renderBoundsB;
        [SerializeField]
        private Camera renderCamera;


        private const string CURRENT_SCENE_NAME = "Share";
        private const string PLAYERPREFS_BEST_SCORE = "BEST_SCORE";


        private void Start()
        {
            gameObject.SetActive(true);

            int _bestScore = PlayerPrefs.GetInt(PLAYERPREFS_BEST_SCORE);
            scoreText.text = _bestScore.ToString();

            RenderTextureFromCamera();

            SceneManager.UnloadSceneAsync(CURRENT_SCENE_NAME);
        }

        private void RenderTextureFromCamera()
        {
            RenderTexture _rt = new RenderTexture(Screen.width, Screen.height, 24);
            
            renderCamera.targetTexture = _rt;
            renderCamera.Render();

            RenderTexture.active = _rt;

            Rect _screenRect = BoundsToScreenSpace();
            Texture2D _renderTex = new Texture2D((int)_screenRect.width, (int)_screenRect.height, TextureFormat.RGB24, false);
            _renderTex.ReadPixels(_screenRect, 0, 0);
            _renderTex.Apply();

            RenderTexture.active = null;
            renderCamera.targetTexture = null;

            byte[] _pixels = _renderTex.EncodeToPNG();
            string _savePath = string.Concat(Application.dataPath, ShareComponent.SHARE_IMAGE_FILENAME);

            File.WriteAllBytes(_savePath, _pixels);
            Destroy(_renderTex);
            _rt.Release();
            Destroy(_rt);
        }

        private void RenderTextureAndSave()
        {
            Rect _screenRect = BoundsToScreenSpace();
            
            Texture2D _renderTex = new Texture2D((int)_screenRect.width, (int)_screenRect.height, TextureFormat.RGB24, false);
            _renderTex.ReadPixels(_screenRect, 0, 0);
            
            byte[] _pixels = _renderTex.EncodeToPNG();
            string _savePath = string.Concat(Application.persistentDataPath, ShareComponent.SHARE_IMAGE_FILENAME);

            File.WriteAllBytes(_savePath, _pixels);
            Destroy(_renderTex);

            gameObject.SetActive(false);
        }

        private Rect BoundsToScreenSpace()
        {
            Rect _rect = new Rect();

            Vector3 _a = renderCamera.WorldToScreenPoint(renderBoundsA.position);
            Vector3 _b = renderCamera.WorldToScreenPoint(renderBoundsB.position);

            _rect.x = _a.x;
            _rect.y = _a.y;
            _rect.width = (int)Mathf.Abs(_b.x - _a.x);
            _rect.height = (int)Mathf.Abs(_b.y - _a.y);
            
            return _rect;
        }
    }
}