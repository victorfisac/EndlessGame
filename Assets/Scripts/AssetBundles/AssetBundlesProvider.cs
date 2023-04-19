using System;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;


namespace EndlessGame.AssetBundles
{
    public interface IAssetBundlesProvider
    {
        void LoadVersionManifest(Action<VersionManifest> pOnManifestLoaded);
        void LoadAssetBundlesManifest(Action<LoadingManifest> pOnManifestLoaded);
        void LoadAssetBundle(string pAssetBundle, Action<AssetBundle> pOnAssetBundleLoaded = null);
    }


    public class AssetBundlesProvider : IAssetBundlesProvider
    {
        private static AssetBundlesProvider m_instance = null;
        private static object m_lock = new object();

        private string m_subfolder = string.Empty;
        private Dictionary<string, AssetBundle> m_bundlesDict = new Dictionary<string, AssetBundle>();

        private const string BASE_URL = "http://www.victorfisac.com/files/endlessgame/";


        public void LoadVersionManifest(Action<VersionManifest> pOnManifestLoaded)
        {
            string _uri = string.Concat(BASE_URL, "version.json");

            HTTPRequest _request = new HTTPRequest(new Uri(_uri), HTTPMethods.Get, (req, res) => {
                VersionManifest _manifest = null;

                if (res != null)
                {
                    if (res.IsSuccess)
                    {
                        _manifest = JsonUtility.FromJson<VersionManifest>(res.DataAsText);

                        for (int i = 0, count = _manifest.versions.Length; i < count; i++)
                        {
                            VersionDefinition _version = _manifest.versions[i];

                            if (_version.version.Equals(Application.version))
                            {
                                m_subfolder = _version.folder;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("AssetBundlesProvider: failed to load asset bundles manifest.");
                    }

                    res.Dispose();
                }

                if (pOnManifestLoaded != null)
                {
                    pOnManifestLoaded(_manifest);
                }

                req.Dispose();
            });

            _request.ConnectTimeout = TimeSpan.FromSeconds(5);
            _request.DisableCache = true;
            _request.Send();
        }


        public void LoadAssetBundlesManifest(Action<LoadingManifest> pOnManifestLoaded)
        {
            string _uri = string.Concat(BASE_URL, m_subfolder, "/manifest.json");

            HTTPRequest _request = new HTTPRequest(new Uri(_uri), HTTPMethods.Get, (req, res) => {
                LoadingManifest _manifest = null;

                if (res != null)
                {
                    if (res.IsSuccess)
                    {
                        _manifest = JsonUtility.FromJson<LoadingManifest>(res.DataAsText);
                    }
                    else
                    {
                        Debug.LogError("AssetBundlesProvider: failed to load asset bundles manifest.");
                    }

                    res.Dispose();
                }

                if (pOnManifestLoaded != null)
                {
                    pOnManifestLoaded(_manifest);
                }

                req.Dispose();
            });

            _request.ConnectTimeout = TimeSpan.FromSeconds(5);
            _request.Send();
        }


        public void LoadAssetBundle(string pAssetBundle, Action<AssetBundle> pOnAssetBundleLoaded = null)
        {
            if (m_bundlesDict.ContainsKey(pAssetBundle))
            {
                pOnAssetBundleLoaded(m_bundlesDict[pAssetBundle]);
                return;
            }

            string _uri = string.Concat(BASE_URL, m_subfolder, "/", pAssetBundle);
            
            HTTPRequest _request = new HTTPRequest(new Uri(_uri), HTTPMethods.Get, (req, res) => {
                if (res != null)
                {
                    if (res.IsSuccess)
                    {
                        AssetBundleCreateRequest _bundleRequest = AssetBundle.LoadFromMemoryAsync(res.Data);
                        _bundleRequest.completed += (op) => { 
                            if (op.isDone)
                            {
                                m_bundlesDict.Add(pAssetBundle, _bundleRequest.assetBundle);

                                Debug.LogFormat("AssetBundlesProvider: loaded asset bundle '{0}' with success.", pAssetBundle);

                                if (pOnAssetBundleLoaded != null)
                                {
                                    pOnAssetBundleLoaded(_bundleRequest.assetBundle);
                                }
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

                        if (pOnAssetBundleLoaded != null)
                        {
                            LoadAssetBundle(pAssetBundle, pOnAssetBundleLoaded);
                        }
                    }

                    res.Dispose();
                }

                req.Dispose();
            });

            _request.ConnectTimeout = TimeSpan.FromSeconds(5);
            _request.Send();
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

    [SerializeField]
    public class VersionManifest
    {
        public VersionDefinition[] versions;
    }

    [Serializable]
        public class VersionDefinition
        {
            public string version;
            public string folder;
        }
}