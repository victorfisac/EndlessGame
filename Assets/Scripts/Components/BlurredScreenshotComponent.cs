#pragma warning disable 0649


using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


namespace EndlessGame.Components
{
    public class BlurredScreenshotComponent : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Material blitMaterial;
        [SerializeField]
        private float animDuration;

        [Header("References")]
        [SerializeField]
        private RawImage blurImage;


        private const string ANIMATION_ID_IN = "Blurred_In";
        private const string ANIMATION_ID_OUT = "Blurred_Out";


        #if UNITY_EDITOR
        private void Awake()
        {
            blitMaterial.shader = Shader.Find(blitMaterial.shader.name);
        }
        #endif


        public void ShowBlurredBackground()
        {
            CreateTexture();

            blurImage.enabled = true;
            DOTween.Restart(ANIMATION_ID_IN);
        }

        public void HideBlurredBackground()
        {
            DOTween.Restart(ANIMATION_ID_OUT);

            StartCoroutine(DestroyTextureWithDelay());
        }

        private void CreateTexture()
        {
            RenderTexture _renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            RenderTexture _tempBuffer = new RenderTexture(_renderTexture.width, _renderTexture.height, 0);

            Camera[] allCameras = Camera.allCameras;
            for (int i = 0; i < allCameras.Length; i++)
            {
                if (allCameras[i].enabled)
                {
                    allCameras[i].targetTexture = _renderTexture;
                    allCameras[i].Render();
                    allCameras[i].targetTexture = null;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                float _blurValue = 0.012f;
                
                blitMaterial.SetFloat("_BlurAmountX", _blurValue);
                blitMaterial.SetFloat("_BlurAmountY", _blurValue * 0.9f);

                Graphics.Blit(_renderTexture, _tempBuffer, blitMaterial, -1);
                Graphics.Blit(_tempBuffer, _renderTexture, blitMaterial, -1);
            }

            blurImage.texture = _renderTexture;
        }

        private IEnumerator DestroyTextureWithDelay()
        {
            yield return new WaitForSeconds(animDuration);

            Destroy(blurImage.texture);
            blurImage.enabled = false;
        }
    }
}