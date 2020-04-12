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
        private string[] assetBundles;

        [Header("Animation")]
        [SerializeField]
        private float loadingDelay;
        [SerializeField]
        private Animator loadingAnimator;


        private int m_loadedBundles = 0;

        private const string ANIMATOR_GOTO_TRIGGER = "GoToOut";
        private const string NEXT_SCENE_NAME = "Menu";


        private void Awake()
        {
            for (int i = 0, count = assetBundles.Length; i < count; i++)
            {
                AssetBundlesProvider.Instance.LoadAssetBundle(assetBundles[i], OnAssetBundleLoaded);
            }
        }

        private IEnumerator GoToMenu()
        {
            loadingAnimator.SetTrigger(ANIMATOR_GOTO_TRIGGER);

            yield return new WaitForSeconds(loadingDelay);

            SceneManager.LoadSceneAsync(NEXT_SCENE_NAME, LoadSceneMode.Additive).completed += OnSceneLoaded;
        }

        private void OnAssetBundleLoaded(AssetBundle pAssetBundle)
        {
            m_loadedBundles++;

            if (m_loadedBundles == assetBundles.Length)
            {
                StartCoroutine(GoToMenu());
            }
        }

        private void OnSceneLoaded(AsyncOperation pOperation)
        {
            if (pOperation.isDone)
            {
                SceneManager.UnloadSceneAsync(0);
            }
            else
            {
                Debug.LogErrorFormat("LoadingScreen: failed to load '{0}' scene.", NEXT_SCENE_NAME);
            }
        }
    }
}