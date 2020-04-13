using System;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;


namespace EndlessGame.Helpers
{
    public class AssetBundlesProvider
    {
        private static AssetBundlesProvider m_instance = null;
        private static object m_lock = new object();


        private Dictionary<string, AssetBundle> m_bundlesDict = new Dictionary<string, AssetBundle>();

        private const string BASE_URL = "https://www.victorfisac.com/files/endlessgame/";


        public void LoadAssetBundlesManifest(Action<LoadingManifest> pOnManifestLoaded)
        {
            string _uri = string.Concat(BASE_URL, "manifest.json");

            HTTPRequest _request = new HTTPRequest(new Uri(_uri), HTTPMethods.Get, (req, res) => {
                if (res.IsSuccess)
                {
                    LoadingManifest _manifest = JsonUtility.FromJson<LoadingManifest>(res.DataAsText);

                    if (pOnManifestLoaded != null)
                        pOnManifestLoaded(_manifest);
                }
                else
                {
                    Debug.LogError("AssetBundlesProvider: failed to load asset bundles manifest.");
                }

                req.Dispose();
                res.Dispose();
            });

            _request.Send();
        }


        public void LoadAssetBundle(string pAssetBundle, Action<AssetBundle> pOnAssetBundleLoaded = null)
        {
            if (m_bundlesDict.ContainsKey(pAssetBundle))
            {
                pOnAssetBundleLoaded(m_bundlesDict[pAssetBundle]);
                return;
            }

            string _uri = string.Concat(BASE_URL, pAssetBundle);
            
            HTTPRequest _request = new HTTPRequest(new Uri(_uri), HTTPMethods.Get, (req, res) => {
                if (res.IsSuccess)
                {
                    AssetBundleCreateRequest _bundleRequest = AssetBundle.LoadFromMemoryAsync(res.Data);
                    _bundleRequest.completed += (op) => { 
                        if (op.isDone)
                        {
                            m_bundlesDict.Add(pAssetBundle, _bundleRequest.assetBundle);

                            if (pOnAssetBundleLoaded != null)
                                pOnAssetBundleLoaded(_bundleRequest.assetBundle);
                        }
                        else
                        {
                            Debug.LogErrorFormat("AssetBundlesProvider: failed to load asset bundle '{0}'.", pAssetBundle);
                        }
                    };
                }
                else
                {
                    Debug.LogErrorFormat("AssetBundlesProvider: failed to download asset bundle '{0}'.\n{1}", pAssetBundle, res.DataAsText);
                }

                req.Dispose();
                res.Dispose();
            });

            _request.Send();
        }

        public void GetAssetFromBundle<T>(string pAssetName, string pAssetBundle, Action<T> pOnAssetRetrieved = null) where T : UnityEngine.Object
        {
            if (!m_bundlesDict.ContainsKey(pAssetBundle))
            {
                Debug.LogErrorFormat("AssetBundlesProvider: failed to retrieve asset '{0}' from asset bundle '{1}'.", pAssetName, pAssetBundle);
                return;
            }

            AssetBundle _assetBundle = m_bundlesDict[pAssetBundle];
            AssetBundleRequest _request = _assetBundle.LoadAssetAsync(pAssetName);

            _request.completed += (op) => {
                if (pOnAssetRetrieved != null)
                    pOnAssetRetrieved(_request.asset as T);
            };
        }


        public static AssetBundlesProvider Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new AssetBundlesProvider();
                }

                return m_instance;
            }
        }
    }

    [SerializeField]
    public class LoadingManifest
    {
        public string[] bundles;
    }
}