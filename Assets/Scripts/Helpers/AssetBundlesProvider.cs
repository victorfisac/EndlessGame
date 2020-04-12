using System;
using BestHTTP;
using UnityEngine;
using UnityEngine.Networking;

namespace EndlessGame.Helpers
{
    public class AssetBundlesProvider
    {
        private static AssetBundlesProvider m_instance = null;
        private static object m_lock = new object();

        private const string BASE_URL = "https://www.victorfisac.com/files/endlessgame/";


        public void LoadAssetBundle(string pAssetBundle, Action<AssetBundle> pOnAssetBundleLoaded = null)
        {
            string _uri = string.Concat(BASE_URL, pAssetBundle);
            
            HTTPRequest _request = new HTTPRequest(new Uri(_uri), HTTPMethods.Get, (req, res) => {
                if (res.IsSuccess)
                {
                    AssetBundleCreateRequest _bundleRequest = AssetBundle.LoadFromMemoryAsync(res.Data);
                    _bundleRequest.completed += (op) => { 
                        if (op.isDone)
                        {
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
}