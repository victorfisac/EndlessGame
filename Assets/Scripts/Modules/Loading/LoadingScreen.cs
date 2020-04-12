using System.Collections;
using EndlessGame.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EndlessGame.Modules.Loading
{
    public class LoadingScreen : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float loadingDelay = 1f;

        [Header("Animation")]
        [SerializeField]
        private float outAnimDuration = 1f;
        [SerializeField]
        private float endAnimDuration = 0.1f;
        [SerializeField]
        private Animator loadingAnimator;

        [Header("References")]
        [SerializeField]
        private RectTransform logoTrans;


        private int m_loadedBundles = 0;
        private LoadingManifest m_manifest = null;

        private const string ANIMATOR_OUT_TRIGGER = "GoToOut";
        private const string ANIMATOR_END_TRIGGER = "GoToEnd";
        private const string NEXT_SCENE_NAME = "Menu";
        private const string LOGO_CONTAINER_TAG = "Respawn";


        private void Awake()
        {
            Application.targetFrameRate = 60;

            AssetBundlesProvider.Instance.LoadAssetBundlesManifest(OnAssetBundlesManifestLoaded);
        }

        private void OnAssetBundlesManifestLoaded(LoadingManifest pManifest)
        {
            m_manifest = pManifest;
            
            for (int i = 0, count = m_manifest.bundles.Length; i < count; i++)
            {
                AssetBundlesProvider.Instance.LoadAssetBundle(m_manifest.bundles[i], OnAssetBundleLoaded);
            }
        }

        private void OnAssetBundleLoaded(AssetBundle pAssetBundle)
        {
            m_loadedBundles++;

            if (m_loadedBundles == m_manifest.bundles.Length)
            {
                StartCoroutine(GoToOut());
            }
        }

        private IEnumerator GoToOut()
        {
            yield return new WaitForSeconds(loadingDelay);

            loadingAnimator.SetTrigger(ANIMATOR_OUT_TRIGGER);

            yield return new WaitForSeconds(outAnimDuration);

            SceneManager.LoadSceneAsync(NEXT_SCENE_NAME, LoadSceneMode.Additive).completed += OnMenuSceneLoaded;
        }

        private void OnMenuSceneLoaded(AsyncOperation pOperation)
        {
            if (pOperation.isDone)
            {
                MoveLogoToMenuScene();
                StartCoroutine(GoToMenu());
            }
            else
            {
                Debug.LogErrorFormat("LoadingScreen: failed to load '{0}' scene.", NEXT_SCENE_NAME);
            }
        }

        private IEnumerator GoToMenu()
        {
            loadingAnimator.SetTrigger(ANIMATOR_END_TRIGGER);

            yield return new WaitForSeconds(outAnimDuration);

            SceneManager.UnloadSceneAsync(0);
        }

        private void MoveLogoToMenuScene()
        {
            Transform _container = GameObject.FindWithTag(LOGO_CONTAINER_TAG).transform;
            
            logoTrans.SetParent(_container, false);
            logoTrans.anchoredPosition = Vector2.zero;
        }
    }
}