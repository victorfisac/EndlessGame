using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BestHTTP;
using EndlessGame.AssetBundles;
using EndlessGame.Credentials;
using UnityEditor;
using UnityEngine;


namespace EndlessGame.Editor
{
    public class AssetBundlesPipeline : EditorWindow
    {
        private static string[] m_bundlesToBuild = null;
        private static int m_uploadedBundles = 0;
        private static string m_currentUploadingBundle = string.Empty;
        private static string m_versionFolder = string.Empty;
        private static FTPClient m_client = null;
        
        private const string MANIFEST_PATH = "files/endlessgame/version.json";
        private const string FTP_BASE_PATH = "public_html/files/endlessgame/";


        [MenuItem("Build/Asset Bundles")]
        public static void BuildAssetBundles()
        {
            HTTPManager.Setup();

            m_client = new FTPClient(CredentialsManager.FTP_HOST_IP, CredentialsManager.FTP_HOST_USERNAME, CredentialsManager.FTP_HOST_PASSWORD);

            GenerateAssetBundles();
            RetrieveVersionManifest();

            m_client = null;
        }

        private static void GenerateAssetBundles()
        {
            string outputPath = GetOutputDirectory();
            BuildPipeline.BuildAssetBundles("Assets/../AssetBundles", BuildAssetBundleOptions.None, BuildTarget.Android);

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            m_bundlesToBuild = Directory.GetFiles(outputPath, "*.ab");
            
            for (int i = 0, count = m_bundlesToBuild.Length; i < count; i++)
            {
                m_bundlesToBuild[i] = m_bundlesToBuild[i].Replace(@"\", "/");

                string[] _split = m_bundlesToBuild[i].Split('/');
                m_bundlesToBuild[i] = _split[_split.Length - 1];
            }

            // Debug.LogFormat("AssetBundlesPipeline: generated asset bundles '{0}'.", string.Join(", ", m_bundlesToBuild));
        }

        private static async void RetrieveVersionManifest()
        {
            string _uri = string.Concat(CredentialsManager.MANIFEST_BASE_URL, MANIFEST_PATH);
            
            HTTPRequest _request = new HTTPRequest(new Uri(_uri), HTTPMethods.Get);
            _request.DisableCache = true;
            _request.OnProgress += (req, downloaded, length) => {
                EditorUtility.DisplayProgressBar("Asset Bundles Pipeline", "Retrieving version manifest from server...", downloaded/length);
            };

            string _rawData = await _request.GetAsStringAsync();

            VersionManifest _manifest = JsonUtility.FromJson<VersionManifest>(_rawData);
            int _maxValue = 0;

            for (int i = 0, count = _manifest.versions.Length; i < count; i++)
            {
                VersionDefinition _version = _manifest.versions[i];

                int _value = int.Parse(_version.folder);

                if (_value > _maxValue)
                {
                    _maxValue = _value;
                }
            }

            DefineNewVersionInManifest(_manifest, _maxValue);

            EditorUtility.ClearProgressBar();

            await Task.Run(UploadAssetBundles);
            
            EditorUtility.DisplayProgressBar("", "", 0.9f);

            UploadAssetBundlesManifest();

            Directory.Delete(GetOutputDirectory(), true);
        }

        private static void UploadAssetBundles()
        {
            m_uploadedBundles = 0;
            EditorApplication.update += OnUploadProgress;

            List<string> _files = m_client.FetchDirectoryInfo(FTP_BASE_PATH).ToList();

            if (!_files.Contains(m_versionFolder))
            {
                string _newFolder = GetRepositoryDirectory();
                m_client.CreateDirectory(_newFolder);
            }

            for (int i = 0, count = m_bundlesToBuild.Length; i < count; i++)
            {
                m_uploadedBundles++;

                string _localPath = string.Concat(GetOutputDirectory(), "/", m_bundlesToBuild[i]);
                
                string[] _split = _localPath.Split('/');
                string _remotePath = string.Concat(GetRepositoryDirectory(), "/", _split[_split.Length - 1]);

                // Debug.LogFormat("AssetBundlesPipeline: uploading asset bundle '{0}' for version '{1}'.", m_bundlesToBuild[i], m_versionFolder);
                
                m_client.UploadFile(_remotePath, _localPath);
            }

            EditorApplication.update -= OnUploadProgress;
        }

        private static void DefineNewVersionInManifest(VersionManifest pManifest, int pNewFolderNumber)
        {
            pNewFolderNumber++;

            List<VersionDefinition> _versionsList = pManifest.versions.ToList();
            bool _found = false;
            
            for (int i = 0, count = _versionsList.Count; i < count; i++)
            {
                if (_versionsList[i].version.Equals(Application.version))
                {
                    _found = true;
                    _versionsList[i].folder = pNewFolderNumber.ToString();
                    break;
                }
            }

            if (!_found)
            {
                _versionsList.Add(new VersionDefinition {
                    version = Application.version,
                    folder = pNewFolderNumber.ToString()
                });
            }

            pManifest.versions = _versionsList.ToArray();

            byte[] _manifestData = Encoding.ASCII.GetBytes(JsonUtility.ToJson(pManifest));
            string _remotePath = string.Concat(FTP_BASE_PATH, "version.json");

            Debug.LogFormat("AssetBundlesPipeline: adding new version definition '{0}' for version '{1}'.", pNewFolderNumber, Application.version);
            
            m_versionFolder = pNewFolderNumber.ToString();

            m_client.UploadData(_remotePath, _manifestData);
        }

        private static async void UploadAssetBundlesManifest()
        {
            EditorUtility.DisplayProgressBar("Asset Bundles Pipeline", "Uploading asset bundles manifest to server...", 0.25f);

            LoadingManifest _manifest = new LoadingManifest() {
                bundles = m_bundlesToBuild
            };

            byte[] _manifestData = Encoding.ASCII.GetBytes(JsonUtility.ToJson(_manifest));
        
            
            string _remotePath = string.Concat(FTP_BASE_PATH, m_versionFolder, "/manifest.json");

            await Task.Delay(500);

            EditorUtility.DisplayProgressBar("Asset Bundles Pipeline", "Uploading asset bundles manifest to server...", 1f);

            await Task.Run(() => {
                m_client.UploadDataAsync(_remotePath, _manifestData);
                
            });

            EditorUtility.ClearProgressBar();
        }

        private static void OnUploadProgress()
        {
            if (m_uploadedBundles == m_bundlesToBuild.Length)
            {
                EditorUtility.ClearProgressBar();
            }
            else
            {
                string _message = string.Format("Uploading bundle '{0}' to server...", m_bundlesToBuild[m_uploadedBundles]);
                EditorUtility.DisplayProgressBar("Asset Bundles Pipeline", _message, ((float)m_uploadedBundles)/m_bundlesToBuild.Length);
            }
        }

        private static string GetOutputDirectory()
        {
            List<string> _splitList = Application.streamingAssetsPath.Replace(@"\", "/").Split('/').ToList();
            _splitList.RemoveRange(_splitList.Count - 2, 2);

            string _path = string.Concat(string.Join("/", _splitList), "/AssetBundles");

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
                AssetDatabase.Refresh();
            }

            return _path.Replace(@"\", "/");
        }

        private static string GetRepositoryDirectory()
        {
            return string.Concat(FTP_BASE_PATH, m_versionFolder);
        }
    }
}